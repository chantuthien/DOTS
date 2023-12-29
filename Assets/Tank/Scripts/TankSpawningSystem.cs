using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using Random = Unity.Mathematics.Random;

partial struct TankSpawningSystem : ISystem
{
    [BurstCompile]
    private void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TankSpawning>();
        state.RequireForUpdate<Config>();
    }
    [BurstCompile]
    private void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var config = SystemAPI.GetSingleton<Config>();

        var random = new Random(123);

        var query = SystemAPI.QueryBuilder().WithAll<URPMaterialPropertyBaseColor>().Build();
        var queryMask = query.GetEntityQueryMask();

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var tanks = new NativeArray<Entity>(config.TankCount, Allocator.Temp);
        ecb.Instantiate(config.TankPrefab, tanks);

        foreach (var tank in tanks)
        {
            ecb.SetComponentForLinkedEntityGroup<URPMaterialPropertyBaseColor>(tank, queryMask, new URPMaterialPropertyBaseColor
            {
                Value = RandomColor(ref random)
            });
        }

        ecb.Playback(state.EntityManager);
    }

    private float4 RandomColor(ref Random random)
    {
        var hue = (random.NextFloat() + 0.618034005f) % 1;
        return (Vector4)Color.HSVToRGB(hue, 1f, 1f);
    }
}
