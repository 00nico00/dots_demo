using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace HelloCube.JobEntity
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.IJobEntity>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new RotationJob() { deltaTime = SystemAPI.Time.DeltaTime };
            job.Schedule();
            // state.Dependency = job.Schedule(state.Dependency); 此处与 job.Schedule(); 等价
        }
    }

    [BurstCompile]
    partial struct RotationJob : IJobEntity
    {
        public float deltaTime;

        private void Execute(ref LocalTransform transform, in RotationSpeed speed)
        {
            transform = transform.RotateY(speed.RadiansPerSecond * deltaTime);
        }
    }
}
