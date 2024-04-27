using Shapes;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace TMG.Shapes
{
    public struct NodeTag : IComponentData {}

    public struct Line : IComponentData
    {
        public float MinSize;
        public float MaxSize;
        public Entity NodeA;
        public Entity NodeB;
        public bool IsExpanding;
    }

    public partial struct NodeMoverSystem : ISystem, ISystemStartStop
    {
        private EntityArchetype _nodeEntityArchetype;
        private Random _random;
        
        public void OnCreate(ref SystemState state)
        {
            _nodeEntityArchetype = state.EntityManager.CreateArchetype(typeof(LocalTransform), typeof(LocalToWorld), typeof(NodeTag));
            _random = Random.CreateFromIndex(100u);
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var line in SystemAPI.Query<RefRW<Line>>())
            {
                var aToB = SystemAPI.GetComponent<LocalTransform>(line.ValueRO.NodeB).Position -
                           SystemAPI.GetComponent<LocalTransform>(line.ValueRO.NodeA).Position;
                if (math.length(aToB) > line.ValueRO.MaxSize)
                {
                    line.ValueRW.IsExpanding = false;
                }
                else if (math.length(aToB) < line.ValueRO.MinSize)
                {
                    line.ValueRW.IsExpanding = true;
                }
                
                var nodeAPos = SystemAPI.GetComponent<LocalTransform>(line.ValueRO.NodeA).Position;
                var nodeBPos = SystemAPI.GetComponent<LocalTransform>(line.ValueRO.NodeB).Position;
               
                var direction = math.normalize(aToB);
                if (line.ValueRO.IsExpanding)
                {
                    nodeAPos -= (direction * deltaTime);
                    nodeBPos += (direction * deltaTime);
                }
                else
                {
                    nodeAPos += (direction * deltaTime);
                    nodeBPos -= (direction * deltaTime);
                }

                SystemAPI.SetComponent(line.ValueRO.NodeA, LocalTransform.FromPosition(nodeAPos));
                SystemAPI.SetComponent(line.ValueRO.NodeB, LocalTransform.FromPosition(nodeBPos));
            }
        }

        public void OnStartRunning(ref SystemState state)
        {
            for (var i = 0; i < 10; i++)
            {
                var nodeA = state.EntityManager.CreateEntity(_nodeEntityArchetype);
                var nodeB = state.EntityManager.CreateEntity(_nodeEntityArchetype);

                var pos1 = _random.NextFloat3(new float3(-10f, -10f, -10f), new float3(10f, 10f, 10f));
                var pos2 = _random.NextFloat3(new float3(-10f, -10f, -10f), new float3(10f, 10f, 10f));

                state.EntityManager.SetComponentData(nodeA, LocalTransform.FromPosition(pos1));
                state.EntityManager.SetComponentData(nodeB, LocalTransform.FromPosition(pos2));

                var newLine = state.EntityManager.CreateEntity();
                state.EntityManager.AddComponentData(newLine, new Line
                {
                    MinSize = 3f,
                    MaxSize = 10f,
                    NodeA = nodeA,
                    NodeB = nodeB,
                    IsExpanding = false
                });
            }
        }

        public void OnStopRunning(ref SystemState state)
        {
        }
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class LineDrawerSystem : SystemBaseDraw
    {
        protected override void OnUpdate()
        {
        }

        protected override void OnDraw(Camera cam)
        {
            using (Draw.Command(cam))
            {
                foreach (var line in SystemAPI.Query<RefRO<Line>>())
                {
                    var nodeAPos = SystemAPI.GetComponent<LocalTransform>(line.ValueRO.NodeA).Position;
                    var nodeBPos = SystemAPI.GetComponent<LocalTransform>(line.ValueRO.NodeB).Position;

                    Draw.Line(nodeAPos, nodeBPos, 1f, Color.red);
                }
            }
        }
    }
}