using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace GeometryEscape
{
    public class SoundOrigin : OnlineObject
    {
        public ParticleSoundFactory particleSoundFactory;

        private ParticleSystem soundParticleSystem;

        private void Awake()
        {
            soundParticleSystem = GetComponent<ParticleSystem>();
        }


        public void SoundOriginStart(ref RigidBodyComponents rigidBodyComponents, ref SoundProperties soundProperties, ref Timer timer)
        {
            OnlineObjectsStart(ref rigidBodyComponents);
            World.Active.EntityManager.SetComponentData(entity, soundProperties);
            World.Active.EntityManager.SetComponentData(entity, timer);
            transform.position = rigidBodyComponents.position;
            float radius = soundProperties.radius - 0.5f;
            var shape = soundParticleSystem.shape;
            shape.radius = radius;
            

            var trails = soundParticleSystem.trails;
            trails.widthOverTrail = soundProperties.width;

            var main = soundParticleSystem.main;
            main.startLifetime = timer.maxT;
            var color = main.startColor = soundProperties.baseColor;
            

            soundParticleSystem.Emit(soundProperties.branchCount);
        }


        public override void Despawn()
        {
            base.Despawn();
            particleSoundFactory.ReclaimSound(this);
        }


        public void SetMaterial(Material material, int materialId)
        {
        }

        public void SetSoundComponentsColor(Color color)
        {

        }

        void FixedUpdate()
        {
            if (!World.Active.EntityManager.GetComponentData<Timer>(entity).isOn)
            {
                Despawn();
            }
        }
    }
}
