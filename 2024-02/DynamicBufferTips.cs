namespace TMG.Live
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    struct PlayerState : IComponentData
    {
        public float3 Position;
        public Entity Entity;
        public bool GameplayState;
    }

    struct TargetState : IComponentData
    {
        public bool GameplayState;
    }

    struct BossTargets : IBufferElementData
    {
        public Entity Entity;
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    partial class DynamicBufferExampleSystem : SystemBase
    {
        protected override void OnCreate()
        {
            EntityManager.CreateSingletonBuffer<BossTargets>();
            EntityManager.CreateSingleton<PlayerState>();
            var dummy = EntityManager.CreateSingleton<TargetState>();

            var buffer = SystemAPI.GetSingletonBuffer<BossTargets>(false);
            buffer.Add(new BossTargets { Entity = dummy });
            buffer.Add(new BossTargets { Entity = dummy });
        }

        protected override void OnUpdate()
        {
            var targets = SystemAPI.GetSingletonBuffer<BossTargets>(true);

            // var targets = SystemAPI.GetSingletonBuffer<BossTargets>(true).ToNativeArray(Allocator.Temp);

            // var targets = SystemAPI.GetSingletonBuffer<BossTargets>(true).Reinterpret<Entity>().ToNativeArray(Allocator.Temp);

            Entities.ForEach((Entity entity, ref PlayerState state) =>
            {
                state.GameplayState = true;

                for (int i = 0; i < targets.Length; i++)
                {
                    SystemAPI.SetComponent(targets[i].Entity, new TargetState { GameplayState = true });
                }
                if (targets.Length > 0)
                {
                    state.Entity = targets[0].Entity;
                    // ObjectDisposedException: Attempted to access BufferTypeHandle<BossTargets> which has been invalidated by a structural change.
                }
            }).Run();

            targets.Reinterpret<BossTargets>();
        }
    }
}