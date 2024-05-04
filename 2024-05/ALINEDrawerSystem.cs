using Drawing;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace TMG.Shapes
{
    public partial struct ALINEDrawerSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var builder = DrawingManager.GetBuilder(true);
            
            state.Dependency = new ALINEDrawJob
            {
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(true),
                Builder = builder
            }.ScheduleParallel(state.Dependency);

            builder.DisposeAfter(state.Dependency);
            // return;
            
            foreach (var line in SystemAPI.Query<Line>())
            {
                var nodeAPos = SystemAPI.GetComponent<LocalTransform>(line.NodeA).Position;
                var nodeBPos = SystemAPI.GetComponent<LocalTransform>(line.NodeB).Position;

                Draw.Line(nodeAPos, nodeBPos, Color.red);
                Draw.ingame.Line(nodeAPos, nodeBPos, Color.blue);
            }
        }     
    }
    
    [BurstCompile]
    public partial struct ALINEDrawJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> TransformLookup;

        public CommandBuilder Builder;
        public void Execute(in Line line)
        {
            var nodeAPos = TransformLookup[line.NodeA].Position;
            var nodeBPos = TransformLookup[line.NodeB].Position;

            Builder.Line(nodeAPos, nodeBPos, Color.green);
        }
    } 
}