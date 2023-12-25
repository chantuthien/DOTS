using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Player>(entity);

            AddComponent<Carry>(entity);
            SetComponentEnabled<Carry>(entity, false);
        }
    }
}
public struct Player : IComponentData { }
public struct Carry : IComponentData, IEnableableComponent
{
    public Entity target;
}
