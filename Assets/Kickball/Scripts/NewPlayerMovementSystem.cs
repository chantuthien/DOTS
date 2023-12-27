using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kickball
{
    [UpdateAfter(typeof(PlayerSpawnerSystem))]
    public partial struct NewPlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NewPlayerMovement>();
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

            var obstacleQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacel>().Build();

            var job = new PlayerMovementJob
            {
                ObstacleTrf = obstacleQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                MinDistSQ = minDistSQ,
                PlayerInput = input
            };
            job.ScheduleParallel();
        }

    }

    [WithAll(typeof(Player))]
    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<LocalTransform> ObstacleTrf;
        public float MinDistSQ;
        public float3 PlayerInput;

        public void Execute(ref LocalTransform playerTrf)
        {
            float3 newPos = playerTrf.Position + PlayerInput;
            foreach (var obstacleTrf in ObstacleTrf)
            {
                if (math.distancesq(newPos, obstacleTrf.Position) <= MinDistSQ)
                {
                    newPos = playerTrf.Position;
                    break;
                }
            }

            playerTrf.Position = newPos;
        }
    }
}
