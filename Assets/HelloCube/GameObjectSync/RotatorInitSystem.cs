using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace HelloCube.GameObjectSync
{
    public partial struct RotatorInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.GameObjectSync>();
            state.RequireForUpdate<DirectoryManaged>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var directory = SystemAPI.ManagedAPI.GetSingleton<DirectoryManaged>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (rotationSpeed, entity) in SystemAPI.Query<RotationSpeed>().WithNone<RotatorGO>()
                         .WithEntityAccess())
            {
                var go = GameObject.Instantiate(directory.RotatorPrefab);
                ecb.AddComponent(entity, new RotatorGO(go));
            }
            
            ecb.Playback(state.EntityManager);
        }
    }

    public class RotatorGO : IComponentData
    {
        public GameObject Value;

        public RotatorGO(GameObject value)
        {
            Value = value;
        }

        public RotatorGO()
        {
            
        }
    }
}
