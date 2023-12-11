using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace HelloCube.MainThread
{
    public partial struct PostTransformMatrixSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.MainThread>();
            state.RequireForUpdate<PostTransformMatrix>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float elapsedTime = (float)SystemAPI.Time.ElapsedTime;

            foreach (var (postTransform, speed) in SystemAPI
                         .Query<RefRW<PostTransformMatrix>, RefRO<RotationSpeed>>())
            {
                postTransform.ValueRW.Value = float4x4.Scale(1, math.sin(elapsedTime), 1);
            }
        }
    }
}