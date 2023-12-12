using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace HelloCube.EnableableComponents
{
    public partial struct RotationSystem : ISystem
    {
        private float timer;
        private const float interval = 1.3f;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            timer = interval;
            state.RequireForUpdate<Execute.EnableableComponents>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            timer -= deltaTime;

            // 切换 RotationSpeed 组件的状态
            if (timer < 0)
            {
                foreach (var rotationSpeedEnabled in SystemAPI.Query<EnabledRefRW<RotationSpeed>>()
                             .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                {
                    rotationSpeedEnabled.ValueRW = !rotationSpeedEnabled.ValueRO;
                }

                timer = interval;
            }
            
            foreach (var (transform, speed) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotationSpeed>>())
            {
                transform.ValueRW = transform.ValueRO.RotateY(speed.ValueRO.RadiansPerSecond * deltaTime);
            }
        }
    }
}