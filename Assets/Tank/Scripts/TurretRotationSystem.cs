using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct TurretRotationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TurretRotaion>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var spin = quaternion.RotateY(SystemAPI.Time.DeltaTime * math.PI);

        foreach (var trf in SystemAPI.Query<RefRW<LocalTransform>>()
                           .WithAll<Turret>())
        {
            trf.ValueRW.Rotation = math.mul(spin, trf.ValueRO.Rotation);
        }
    }
}
