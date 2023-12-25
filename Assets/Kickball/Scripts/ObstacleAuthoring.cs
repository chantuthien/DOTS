using Unity.Entities;
using UnityEngine;

namespace Kickball
{
    public class ObstacleAuthoring : MonoBehaviour
    {
        class Baker : Baker<ObstacleAuthoring>
        {
            public override void Bake(ObstacleAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Obstacel>(entity);
            }
        }
    }
    public struct Obstacel : IComponentData { }
}
