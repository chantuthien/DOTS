using Kickball;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kickball
{
    [UpdateBefore(typeof(BallMovementSystem))]
    public partial struct BallCarrySystem : ISystem
    {
        static readonly float3 CarryOffset = new float3(0, 2, 0);

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallCarry>();
            state.RequireForUpdate<Config>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config  = SystemAPI.GetSingleton<Config>();

            foreach (var (ballTrf, carry) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Carry>>()
                                             .WithAll<Ball>())
            {
                var playerTrf = state.EntityManager.GetComponentData<LocalTransform>(carry.ValueRO.Target);
                ballTrf.ValueRW.Position = playerTrf.Position + CarryOffset;
            }

            if (!Input.GetKeyDown(KeyCode.C))
                return;

            foreach (var (playerTrf, playerEntity) in SystemAPI.Query<RefRW<LocalTransform>>()
                                                     .WithAll<Player>()
                                                     .WithEntityAccess())
            {
                if (state.EntityManager.IsComponentEnabled<Carry>(playerEntity))
                {
                    // put down ball
                    var carried = state.EntityManager.GetComponentData<Carry>(playerEntity);

                    var ballTransform = state.EntityManager.GetComponentData<LocalTransform>(carried.Target);
                    ballTransform.Position = playerTrf.ValueRO.Position;
                    state.EntityManager.SetComponentData(carried.Target, ballTransform);

                    state.EntityManager.SetComponentEnabled<Carry>(carried.Target, false);
                    state.EntityManager.SetComponentEnabled<Carry>(playerEntity, false);

                    state.EntityManager.SetComponentData(carried.Target, new Carry());
                    state.EntityManager.SetComponentData(playerEntity, new Carry());
                }
                else
                {
                    // pick up first ball in range
                    foreach (var (ballTrf, ballEntity) in SystemAPI.Query<RefRO<LocalTransform>>()
                                                         .WithAll<Ball>()
                                                         .WithDisabled<Carry>()
                                                         .WithEntityAccess())
                    {
                        float distSQ = math.distancesq(playerTrf.ValueRO.Position, ballTrf.ValueRO.Position);
                        if (distSQ <= config.BallKickingRangeSQ)
                        {
                            state.EntityManager.SetComponentData(ballEntity, new Velocity());

                            state.EntityManager.SetComponentData(playerEntity, new Carry { Target = ballEntity });
                            state.EntityManager.SetComponentData(ballEntity, new Carry { Target = playerEntity });

                            state.EntityManager.SetComponentEnabled<Carry>(playerEntity, true);
                            state.EntityManager.SetComponentEnabled<Carry>(ballEntity, true);

                            break;
                        }
                    }
                }
            }
        }
    }
}
