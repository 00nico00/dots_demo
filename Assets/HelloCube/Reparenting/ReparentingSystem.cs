using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace HelloCube.Reparenting
{
    public partial struct ReparentingSystem : ISystem
    {
        private bool attched;
        private float timer;
        private const float interval = 0.7f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            timer = interval;
            attched = true;
            state.RequireForUpdate<Execute.Reparenting>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            timer -= SystemAPI.Time.DeltaTime;
            if (timer > 0)
            {
                return;
            }

            timer = interval;

            var rotatorEntity = SystemAPI.GetSingletonEntity<RotationSpeed>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            if (attched)
            {
                DynamicBuffer<Child> children = SystemAPI.GetBuffer<Child>(rotatorEntity);
                for (int i = 0; i < children.Length; i++)
                {
                    ecb.RemoveComponent<Parent>(children[i].Value);
                }
            } else
            {
                foreach (var (transform, entity) in SystemAPI.Query<RefRO<LocalTransform>>()
                             .WithNone<RotationSpeed>()
                             .WithEntityAccess())
                {
                    ecb.AddComponent(entity, new Parent() { Value = rotatorEntity });
                }
            }
            
            ecb.Playback(state.EntityManager);

            attched = !attched;
        }
    }
}
