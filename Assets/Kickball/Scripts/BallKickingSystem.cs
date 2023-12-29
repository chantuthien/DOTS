using Kickball;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kickball
{
    [UpdateBefore(typeof(BallMovementSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallKickingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallKicking>();
            state.RequireForUpdate<Config>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            if (!Input.GetKeyDown(KeyCode.Space))
                return;

            foreach (var playerTrf in SystemAPI.Query<RefRO<LocalTransform>>()
                                               .WithAll<Player>())
            {
                foreach (var (ballTrf, velocity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<Velocity>>()
                                                   .WithAll<Ball>())
                {
                    float minDistSQ = math.distancesq(playerTrf.ValueRO.Position, ballTrf.ValueRO.Position);

                    if (minDistSQ <= config.BallKickingRangeSQ)
                    {
                        var playerToBall = playerTrf.ValueRO.Position.xz - ballTrf.ValueRO.Position.xz;
                        velocity.ValueRW.Value += math.normalizesafe(playerToBall) * config.BallKickForce;
                    }
                }
            }
        }
    }
}
