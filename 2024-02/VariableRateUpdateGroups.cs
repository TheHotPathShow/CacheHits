using Unity.Entities;
using UnityEngine;

namespace THPS.VariableRateUpdate
{
    public partial class TurboRateUpdateGroup : ComponentSystemGroup
    {
        public TurboRateUpdateGroup()
        {
            // uint - Time in ms for how frequently this should update
            //        - Updates max 1x per frame
            // bool - Do systems in this group need Time?
            //        - Minor perf gain if false
            RateManager = new RateUtils.VariableRateManager(5000, true);
        }
    }
    
    [UpdateInGroup(typeof(TurboRateUpdateGroup))]
    public partial struct ImportantTurboRateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            Debug.Log($"Updating important things at: {SystemAPI.Time.ElapsedTime}");
        }
    }
    
    [UpdateInGroup(typeof(TurboRateUpdateGroup))]
    public partial struct NotImportantTurboRateSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            var rateMan = new RateUtils.VariableRateManager(1000, true);
            var rateGroup = state.World.GetExistingSystemManaged<TurboRateUpdateGroup>();
            rateGroup.RateManager = rateMan;
        }

        public void OnUpdate(ref SystemState state)
        {
            Debug.Log($"Updating NOT important things at: {SystemAPI.Time.ElapsedTime}");
        }
    }
}