using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HelloCube.Prefabs
{
    public partial struct SpawnSystem : ISystem
    {
        private uint updateCounter;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Spawner>();
            state.RequireForUpdate<Execute.Prefabs>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // 创建一个 query 来查找所有含有 RotationSpeed 的 entity
            // query 会被缓冲到 source generation，因此不会每次 update 都创建
            var spinningCubeQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeed>().Build();

            if (spinningCubeQuery.IsEmpty)
            {
                var prefab = SystemAPI.GetSingleton<Spawner>().Prefab;
                var instances = state.EntityManager.Instantiate(prefab, 500, Allocator.Temp);
                var random = Random.CreateFromIndex(updateCounter++);
                
                foreach (var entity in instances)
                {
                    var transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                    transform.ValueRW.Position = (random.NextFloat3() - new float3(0.5f, 0.0f, 0.5f)) * 20;
                }
            }
        }
    }
}

