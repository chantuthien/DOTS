using Kickball;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kickball
{
    [UpdateAfter(typeof(PlayerSpawnerSystem))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovement>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float3 input = new float3(-horizontal, 0f, -vertical) * SystemAPI.Time.DeltaTime * config.PlayerSpeed;
            if (input.Equals(float3.zero))
                return;

            float minDist = config.ObstacleRadius + 0.5f;
            float minDistSQ = minDist * minDist;

            foreach (var playerTrf in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<Player>())
            {
                float3 newPos = playerTrf.ValueRO.Position + input;
                foreach (var obstacleTrf in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Obstacel>())
                {
                    if (math.distancesq(newPos, obstacleTrf.ValueRO.Position) <= minDistSQ)
                    {
                        newPos = playerTrf.ValueRO.Position;
                        break;
                    }
                }

                playerTrf.ValueRW.Position = newPos;
            }
        }
    }
}
