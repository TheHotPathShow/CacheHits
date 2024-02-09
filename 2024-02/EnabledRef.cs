namespace TMG.CacheHits
{
	public partial struct PlayerInputSystem : ISystem
    {
		public void OnUpdate(ref SystemState state)
		{
			var inputActionMap = SystemAPI.ManagedAPI.GetComponent<Singleton>(state.SystemHandle).Value.DefaultMap;
			var moveInput = inputActionMap.PlayerMove.ReadValue<Vector2>();
			var jumpInput = inputActionMap.PlayerJump.WasPressedThisFrame();
			var sprintInput = inputActionMap.PlayerSprint.IsPressed();
	
			foreach (var (playerMoveInput, playerSprintMultiplier, entity) in SystemAPI.Query<RefRW<PlayerMoveInput>,
							EnabledRefRW<PlayerSprintMultiplier>>()
						.WithPresent<PlayerJumpProperties, PlayerSprintMultiplier>().WithEntityAccess())
			{
				playerMoveInput.ValueRW.Value = moveInput;
				SystemAPI.SetComponentEnabled<PlayerJumpProperties>(entity, jumpInput);
	
				// Set enabled state of a component
				playerSprintMultiplier.ValueRW = sprintInput;
			}
		}
    }
	
    [BurstCompile]
    // Ensure enabled state is ignored
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct PlayerMoveJob : IJobEntity
    {
        public float3 CameraForward;
        public float3 CameraRight;
        public float DeltaTime;

        // Need both EnabledRefRO & the actual component to read enabled status and values
        private void Execute(ref LocalTransform transform, in PlayerMoveInput moveInput, in PlayerMoveSpeed moveSpeed, 
            EnabledRefRO<PlayerSprintMultiplier> playerSprintEnabled, in PlayerSprintMultiplier playerSprint)
        {
            var finalSpeed = moveSpeed.Value;
            // Check enabled state of a component
            if (playerSprintEnabled.ValueRO)
            {
                var multiplier = playerSprint.Value;
                finalSpeed *= multiplier;
            }
            var moveVector = (CameraForward * moveInput.Value.y + CameraRight * moveInput.Value.x) *
                             finalSpeed * DeltaTime;
            transform = transform.Translate(moveVector);
            if (math.lengthsq(moveVector) <= float.Epsilon) return;
            transform.Rotation = quaternion.LookRotationSafe(moveVector, math.up());
        }
    }
}