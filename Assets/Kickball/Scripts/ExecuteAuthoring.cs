using Unity.Entities;
using UnityEngine;

namespace Kickball
{
    public class ExecuteAuthoring : MonoBehaviour
    {
        [Header("Step 1")]
        public bool ObstacleSpawner;

        [Header("Step 2")]
        public bool PlayerSpawner;
        public bool PlayerMovement;

        [Header("Step 3")]
        public bool BallSpawner;
        public bool BallMovement;

        [Header("Step 4")]
        public bool NewPlayerMovement;
        public bool NewBallMovement;

        [Header("Step 5")]
        public bool BallCarry;
        public bool BallKicking;

        class Baker : Baker<ExecuteAuthoring>
        {
            public override void Bake(ExecuteAuthoring auth)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                if (auth.ObstacleSpawner)
                    AddComponent<ObstacleSpawner>(entity);
                if (auth.PlayerMovement)
                    AddComponent<PlayerMovement>(entity);
                if (auth.PlayerSpawner)
                    AddComponent<PlayerSpawner>(entity);
                if (auth.BallSpawner)
                    AddComponent<BallSpawner>(entity);
                if (auth.BallMovement)
                    AddComponent<BallMovement>(entity);
                if (auth.NewPlayerMovement)
                    AddComponent<NewPlayerMovement>(entity);
                if (auth.NewBallMovement)
                    AddComponent<NewBallMovement>(entity);
                if (auth.BallCarry)
                    AddComponent<BallCarry>(entity);
                if (auth.BallKicking)
                    AddComponent<BallKicking>(entity);
            }
        }
    }
    public struct ObstacleSpawner : IComponentData { }
    public struct PlayerMovement : IComponentData { }
    public struct PlayerSpawner : IComponentData { }
    public struct BallSpawner : IComponentData { }
    public struct BallMovement : IComponentData { }
    public struct NewPlayerMovement : IComponentData { }
    public struct NewBallMovement : IComponentData { }
    public struct BallCarry : IComponentData { }
    public struct BallKicking : IComponentData { }
}
