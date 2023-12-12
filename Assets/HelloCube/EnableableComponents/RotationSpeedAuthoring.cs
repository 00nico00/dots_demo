using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace HelloCube.EnableableComponents
{
    public class RotationSpeedAuthoring : MonoBehaviour
    {
        public bool StartEnable;
        public float DegreesPerSecond = 360.0f;

        public class Baker : Baker<RotationSpeedAuthoring>
        {
            public override void Bake(RotationSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.NonUniformScale);
                
                AddComponent(entity, new RotationSpeed
                {
                    RadiansPerSecond = math.radians(authoring.DegreesPerSecond)
                });
                SetComponentEnabled<RotationSpeed>(entity, authoring.StartEnable);
            }
        }
    }

    struct RotationSpeed : IComponentData, IEnableableComponent
    {
        public float RadiansPerSecond;
    }
}
