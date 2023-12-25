using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct CameraSystem : ISystem
{
    Entity target;
    Unity.Mathematics.Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Camera>();
        random = new Unity.Mathematics.Random(123);
    }
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (target == Entity.Null || Input.GetKeyDown(KeyCode.Space))
        {
            var tankQuery = SystemAPI.QueryBuilder().WithAll<Tank>().Build();
            var tanks = tankQuery.ToEntityArray(Allocator.Temp);
            if (tanks.Length == 0)
                return;

            target = tanks[random.NextInt(tanks.Length)];
        }

        var cameraTrf = CameraSingleton.Ins.transform;
        var tankTrf = SystemAPI.GetComponent<LocalToWorld>(target);
        cameraTrf.position = tankTrf.Position;
        cameraTrf.position -= 10f * (Vector3)tankTrf.Forward;
        cameraTrf.position += new Vector3(0f, 5f, 0f);
        cameraTrf.LookAt(tankTrf.Position);
    }
}
