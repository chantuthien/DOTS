using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TankMovementSystem : ISystem
{
    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TankMovement>();
    }
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        var dt = SystemAPI.Time.DeltaTime;
        foreach (var (trf, entity) in SystemAPI.Query<RefRW<LocalTransform>>()
                                     .WithAll<Tank>()
                                     .WithEntityAccess())
        {
            var pos = trf.ValueRO.Position;
            pos.y = entity.Index;
            var angle = (0.5f + noise.snoise(pos/10f)) * 4f * math.PI;
            var dir = float3.zero;
            math.sincos(angle, out dir.x, out dir.z);

            trf.ValueRW.Position += dir * dt * 5f;
            trf.ValueRW.Rotation = quaternion.RotateY(angle);

        }
    }
}
