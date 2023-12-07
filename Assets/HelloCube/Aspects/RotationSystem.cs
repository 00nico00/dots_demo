using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace HelloCube.Aspects
{
    public partial struct RotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.Aspects>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var rotate in SystemAPI.Query<RotateAspect>())
            {
                rotate.Rotate(deltaTime);
            }
        }
    }

    readonly partial struct RotateAspect : IAspect
    {
        readonly RefRW<LocalTransform> m_Transform;
        readonly RefRO<RotationSpeed> m_Speed;

        public void Rotate(float deltaTime)
        {
            m_Transform.ValueRW = m_Transform.ValueRO.RotateY(m_Speed.ValueRO.RadiansPerSecond * deltaTime);
        }
    }
}
