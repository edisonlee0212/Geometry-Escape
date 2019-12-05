using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;

namespace GeometryEscape
{
    public struct RigidBodyComponents
    {
        public Vector3 position;
        public Quaternion rotation;
    }
    public class OnlineObject : MonoBehaviour
    {
        public Entity entity;
        public Translation position;
        public Rotation rotation;
        public Rigidbody2D rigidBody;


        //Set the gameobject position and sync with the ECS.
        public void OnlineObjectsStart(ref RigidBodyComponents rigidBodyComponents)
        {
            rigidBody = GetComponent<Rigidbody2D>();
            transform.SetPositionAndRotation(rigidBodyComponents.position, rigidBodyComponents.rotation);
            position.Value = rigidBodyComponents.position;
            rotation.Value = rigidBodyComponents.rotation;
            UploadPosition();
            Spawn();
        }
        
        protected virtual void SyncRigidBody()
        {
            Vector3 position = World.Active.EntityManager.GetComponentData<Translation>(entity).Value;
            rigidBody.position = position;
            Quaternion rotation = World.Active.EntityManager.GetComponentData<Rotation>(entity).Value;
            rigidBody.transform.rotation = rotation;
        }

        protected virtual void UploadRigidBody()
        {
            UploadPosition();
            UploadRotation();
        }

        protected virtual void UploadPosition()
        {
            position.Value = transform.position;
            World.Active.EntityManager.SetComponentData(entity,
                    position);
        }

        protected virtual void UploadRotation()
        {
            rotation.Value = transform.rotation;
            World.Active.EntityManager.SetComponentData(entity,
                    rotation);
        }

        public virtual void Spawn()
        {
            gameObject.SetActive(true);
        }

        public virtual void Despawn()
        {
            gameObject.SetActive(false);
        }
    }
}
