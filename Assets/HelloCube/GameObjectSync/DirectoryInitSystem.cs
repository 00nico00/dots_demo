using System;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace HelloCube.GameObjectSync
{
    public partial struct DirectoryInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Execute.GameObjectSync>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var go = GameObject.Find("Directory");
            if (go == null)
            {
                throw new Exception("GameObject 'Directory' not found");
            }

            var directory = go.GetComponent<Directory>();

            var directoryManaged = new DirectoryManaged
            {
                RotatorPrefab = directory.RotatorPrefab,
                RotationToggle = directory.RotationToggle
            };

            var entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, directoryManaged);
        }
    }

    public class DirectoryManaged : IComponentData
    {
        public GameObject RotatorPrefab;
        public Toggle RotationToggle;

        public DirectoryManaged()
        {
            
        }
    }
}
