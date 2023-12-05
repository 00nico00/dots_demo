using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

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
