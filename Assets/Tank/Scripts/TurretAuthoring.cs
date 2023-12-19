using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


class TurretAuthoring : MonoBehaviour
{
    public GameObject CannonBallPrefab;
    public GameObject CanonBallSpawn;

    class Baker : Baker<TurretAuthoring>
    {
        public override void Bake(TurretAuthoring authoring)
        {
            var  entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Turret
            {
                CannonBallPrefab = GetEntity(authoring.CannonBallPrefab, TransformUsageFlags.Dynamic),
                CannonBallSpawn = GetEntity(authoring.CanonBallSpawn, TransformUsageFlags.Dynamic)
            });

            AddComponent<Shooting>(entity);
        }
    }
}
public struct Turret : IComponentData
{
    public Entity CannonBallPrefab;
    public Entity CannonBallSpawn;
}
public struct Shooting : IComponentData, IEnableableComponent { }
