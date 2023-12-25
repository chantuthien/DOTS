using Kickball;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Kickball
{
    [UpdateAfter(typeof(ObstacleSpawnerSystem))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct PlayerSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerSpawner>();
            state.RequireForUpdate<Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var config = SystemAPI.GetSingleton<Config>();

            foreach (var trf in SystemAPI.Query<RefRO<LocalTransform>>()
                                .WithAll<Obstacel>())
            {
                var player = state.EntityManager.Instantiate(config.PlayerPrefab);

                state.EntityManager.SetComponentData(player, new LocalTransform
                {
                    Position = new float3
                    {
                        x = trf.ValueRO.Position.x + config.PlayerOffset,
                        y = 1,
                        z = trf.ValueRO.Position.z + config.PlayerOffset
                    },
                    Scale = 1,
                    Rotation = quaternion.identity
                });
                ;
            }
        }
    }
}
