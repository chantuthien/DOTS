using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kickball
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BallMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallMovement>();
            state.RequireForUpdate<Config>();
        }
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<Config>();

            var dt = SystemAPI.Time.DeltaTime;

            var decayFactor = config.BallVelocityDecay * dt;
            var minDist = config.ObstacleRadius + 0.5f;
            var minDistSQ = minDist * minDist;

            foreach (var (ballTransform, velocity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Velocity>>()
                                                      .WithAll<Ball>()
                                                      .WithDisabled<Carry>())
            {
                if (velocity.ValueRO.Value.Equals(float2.zero))
                    continue;

                var magnitude = math.length(velocity.ValueRO.Value);
                var newPostition = ballTransform.ValueRW.Position + new float3(velocity.ValueRO.Value.x, 0f, velocity.ValueRO.Value.y) * dt;

                foreach (var obstacelTrf in SystemAPI.Query<RefRO<LocalTransform>>()
                                            .WithAll<Obstacel>())
                {
                    if (math.distancesq(newPostition, obstacelTrf.ValueRO.Position) <= minDistSQ)
                    {
                        newPostition = DeflecBall(ballTransform.ValueRO.Position, obstacelTrf.ValueRO.Position, velocity, magnitude, dt);
                        break;
                    }
                }

                ballTransform.ValueRW.Position = newPostition;
                var newMagnitude = math.max(magnitude - decayFactor, 0f);
                velocity.ValueRW.Value = math.normalizesafe(velocity.ValueRO.Value) * newMagnitude;
            }
        }
        private float3 DeflecBall(float3 ballPos, float3 obstaclePos, RefRW<Velocity> velocity, float magnitude, float dt)
        {
            var obstacelToBallVector = math.normalize((ballPos - obstaclePos).xz);
            velocity.ValueRW.Value = math.reflect(math.normalize(velocity.ValueRO.Value), obstacelToBallVector) * magnitude;
            return ballPos + new float3(velocity.ValueRO.Value.x, 0, velocity.ValueRO.Value.y) * dt;
        }
    }
}
