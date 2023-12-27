using Kickball;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Kickball
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallSpawner>();
            state.RequireForUpdate<Config>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            if (!Input.GetKeyDown(KeyCode.Return))
                return;

            var rand = new Random(123);

            foreach (var playerTrf in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player>())
            {
                var ball = state.EntityManager.Instantiate(config.BallPrefab);
                state.EntityManager.SetComponentData(ball, new LocalTransform
                {
                    Position = playerTrf.ValueRO.Position,
                    Rotation = quaternion.identity,
                    Scale = 1
                });

                state.EntityManager.SetComponentData(ball, new Velocity
                {
                    Value = rand.NextFloat2Direction() * config.BallStartVelocity
                });
            }
        }
    }
}
