using Unity.Entities;
using UnityEngine;

namespace Kickball
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int ObstaclesNumRows;
        public int ObstaclesNumColumns;
        public float ObstacleGridCellSize;
        public float ObstacleRadius;
        public float ObstacleOffset;
        public float PlayerOffset;
        public float PlayerSpeed;
        public float BallStartVelocity;
        public float BallVelocityDecay;
        public float BallKickingRange;
        public float BallKickForce;
        public GameObject ObstaclePrefab;
        public GameObject PlayerPrefab;
        public GameObject BallPrefab;

        class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring auth)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Config
                {
                    NumRows = auth.ObstaclesNumRows,
                    NumColumns = auth.ObstaclesNumColumns,
                    ObstacleGridCellSize = auth.ObstacleGridCellSize,
                    ObstacleRadius = auth.ObstacleRadius,
                    ObstacleOffset = auth.ObstacleOffset,
                    PlayerOffset = auth.PlayerOffset,
                    PlayerSpeed = auth.PlayerSpeed,
                    BallStartVelocity = auth.BallStartVelocity,
                    BallVelocityDecay = auth.BallVelocityDecay,
                    BallKickingRangeSQ = auth.BallKickingRange * auth.BallKickingRange,
                    BallKickForce = auth.BallKickForce,

                    ObstaclePrefab = GetEntity(auth.ObstaclePrefab, TransformUsageFlags.Dynamic),
                    PlayerPrefab = GetEntity(auth.PlayerPrefab, TransformUsageFlags.Dynamic),
                    BallPrefab = GetEntity(auth.BallPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
    public struct Config : IComponentData
    {
        public int NumRows; // obstacles and players spawns in a grid, one obstacle and player per cell
        public int NumColumns;
        public float ObstacleGridCellSize;
        public float ObstacleRadius;
        public float ObstacleOffset;
        public float PlayerOffset;
        public float PlayerSpeed; // meters per second
        public float BallStartVelocity;
        public float BallVelocityDecay;
        public float BallKickingRangeSQ; // square distance of how close a player must be to a ball to kick it
        public float BallKickForce;
        public Entity ObstaclePrefab;
        public Entity PlayerPrefab;
        public Entity BallPrefab;
    }
}
