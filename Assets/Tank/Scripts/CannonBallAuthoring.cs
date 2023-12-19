using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class CannonBallAuthoring : MonoBehaviour
{
    class Baker : Baker<CannonBallAuthoring>
    {
        public override void Bake(CannonBallAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<Cannon_Ball>(entity);

            //AddComponent<URPMaterialPropertyBaseColor>(entity);
        }
    }
}

public struct Cannon_Ball : IComponentData
{
    public float3 Velocity;
}
