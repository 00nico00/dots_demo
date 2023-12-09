using Unity.Assertions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace HelloCube.IJobChunk
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.IJobChunk>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spinningCubeQuery = SystemAPI.QueryBuilder().WithAll<RotationSpeed, LocalTransform>().Build();

            var job = new RotationJob
            {
                TransformTypeHandle = SystemAPI.GetComponentTypeHandle<LocalTransform>(),
                RotationSpeedTypeHandle = SystemAPI.GetComponentTypeHandle<RotationSpeed>(true),
                DeltaTime = SystemAPI.Time.DeltaTime
            };

            // 与 IJobEntity 不同，IJobChunk 需要手动传递查询。
            // 此外，IJobChunk 不会隐式传递和分配 state.Dependency JobHandle。
            // （通过传递和分配 state.Dependency JobHandle 的模式，确保在不同系统中调度的实体作业将按需要相互依赖。）
            state.Dependency = job.ScheduleParallel(spinningCubeQuery, state.Dependency);
        }
    }

    [BurstCompile]
    struct RotationJob : Unity.Entities.IJobChunk
    {
        public ComponentTypeHandle<LocalTransform> TransformTypeHandle;
        [ReadOnly] public ComponentTypeHandle<RotationSpeed> RotationSpeedTypeHandle;
        public float DeltaTime;
        
        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex,
                            bool useEnabledMask, in v128 chunkEnabledMask)
        {
            // useEnableMask 参数为 true 表示在块中有一个或多个实体的查询组件被禁用。
            // 如果查询组件类型都没有实现 IEnableableComponent，我们可以假设 useEnabledMask 将始终为 false。
            // 但是，为了以防万一，最好添加此保护检查，以防将来有人更改查询或组件类型。
            Assert.IsFalse(useEnabledMask);

            var transforms = chunk.GetNativeArray(ref TransformTypeHandle);
            var rotationSpeeds = chunk.GetNativeArray(ref RotationSpeedTypeHandle);
            
            for (int i = 0, chunkEntityCount = chunk.Count; i < chunkEntityCount; i++)
            {
                transforms[i] = transforms[i].RotateY(rotationSpeeds[i].RadiansPerSecond * DeltaTime);
            }
        }
    }
}