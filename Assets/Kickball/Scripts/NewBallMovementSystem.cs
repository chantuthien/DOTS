using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Kickball
{

    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct NewBallMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NewBallMovement>();
            state.RequireForUpdate<Config>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();
            var obstacleQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, Obstacel>().Build();
            var minDist = config.ObstacleRadius + 0.5f;

            var job = new BallMovementJob
            {
                ObstacleTransforms = obstacleQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator),
                DecayFactor = config.ObstacleRadius + 0.5f,
                DeltaTime = SystemAPI.Time.DeltaTime,
                MinDistToObstacleSQ = minDist * minDist
            };
            job.ScheduleParallel();
        }
    }

    [WithAll(typeof(Ball))]
    [WithDisabled(typeof(Carry))]
    [BurstCompile]
    public partial struct BallMovementJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<LocalTransform> ObstacleTransforms;
        public float DecayFactor;
        public float DeltaTime;
        public float MinDistToObstacleSQ;
        public void Execute(ref LocalTransform ballTransform, ref Velocity velocity)
        {
            if (velocity.Value.Equals(float2.zero))
                return;

            var magnitude = math.length(velocity.Value);
            var newPostition = ballTransform.Position + new float3(velocity.Value.x, 0f, velocity.Value.y) * DeltaTime;

            foreach (var obstacelTrf in ObstacleTransforms)
            {
                if (math.distancesq(newPostition, obstacelTrf.Position) <= MinDistToObstacleSQ)
                {
                    newPostition = DeflecBall(ballTransform.Position, obstacelTrf.Position, velocity, magnitude, DeltaTime);
                    break;
                }
            }

            ballTransform.Position = newPostition;

            var newMagnitude = math.max(magnitude - DecayFactor, 0f);
            velocity.Value = math.normalizesafe(velocity.Value) * newMagnitude;
        }
        private float3 DeflecBall(float3 ballPos, float3 obstaclePos, Velocity velocity, float magnitude, float dt)
        {
            var obstacelToBallVector = math.normalize((ballPos - obstaclePos).xz);
            velocity.Value = math.reflect(math.normalize(velocity.Value), obstacelToBallVector) * magnitude;
            return ballPos + new float3(velocity.Value.x, 0, velocity.Value.y) * dt;
        }
    }

}
