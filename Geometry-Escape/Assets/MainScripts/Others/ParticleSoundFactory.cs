using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;


//This class deals with sound element generation and recycling.
namespace GeometryEscape
{
    public struct SoundInfo
    {
        public float lifeTime, x, y, z;
        public short branchAmount, colorIndex;
        public byte radius, width;
        public bool bounce;
    }
    [CreateAssetMenu]
    public class ParticleSoundFactory : ScriptableObject
    {
        [SerializeField]
        private Color[] colors = null;
        [SerializeField]
        private UnityEngine.Material[] materials = null;
        [SerializeField]
        private SoundOrigin soundOriginPrefab = null;
        Timer timerReset;
        SoundProperties soundPropertiesReset;
        RigidBodyComponents rigidBodyComponentsReset;

        public EntityArchetype soundOriginEntityArchetype;

        public List<SoundOrigin> soundPool;
        Scene poolScene;

        public EntityManager entityManager;

        public void Start()
        {
            entityManager = World.Active.EntityManager;
            soundOriginEntityArchetype = entityManager.CreateArchetype(
            typeof(Timer),
            typeof(Translation),
            typeof(SoundProperties));


            timerReset = new Timer
            {
                isOn = true,
                maxT = 0,
                T = 0
            };

            soundPropertiesReset = new SoundProperties
            {
                syncMap = false,
                branchCount = 0,
                baseColor = Color.white,
                actualColor = Color.white
            };

            rigidBodyComponentsReset = new RigidBodyComponents
            {
                position = Vector3.zero,
                rotation = Quaternion.identity
            };

            soundPool = new List<SoundOrigin>();

            if (Application.isEditor)
            {
                poolScene = SceneManager.GetSceneByName(name);
                if (poolScene.isLoaded)
                {
                    GameObject[] rootObjects = poolScene.GetRootGameObjects();
                    for (int i = 0; i < rootObjects.Length; i++)
                    {
                        
                        SoundOrigin pooledSound = rootObjects[i].GetComponent<SoundOrigin>();
                        if (pooledSound != null && !pooledSound.gameObject.activeSelf)
                        {
                            soundPool.Add(pooledSound);
                        }
                    }
                    return;
                }
            }
            poolScene = SceneManager.CreateScene(name);
        }
        public void CreateSound(bool syncMap, Vector2 originPosition, int branchAmount, int colorMode = 0, int radius = 6, float lifeTime = -1, int width = 1, bool bounce = true)
        {
            if (lifeTime == -1) lifeTime = branchAmount * 0.015f;
            SoundOrigin instance;
            int lastIndex = soundPool.Count - 1;
            if (lastIndex >= 0)
            {
                instance = soundPool[lastIndex];
                soundPool.RemoveAt(lastIndex);
                ResetSound(syncMap, instance, branchAmount, originPosition, radius, colorMode, lifeTime, width, bounce);
            }
            else
            {
                instance = NewSound();
                ResetSound(syncMap, instance, branchAmount, originPosition, radius, colorMode, lifeTime, width, bounce);
            }
        }
        void ResetSound(bool syncMap, SoundOrigin instance, int loudNess, Vector2 originPosition, int radius, int colorIndex, float lifeTime, int width, bool bounce)
        {
            timerReset.maxT = lifeTime;
            timerReset.T = 0;
            timerReset.isOn = true;
            soundPropertiesReset.colorIndex = colorIndex;
            soundPropertiesReset.branchCount = loudNess;
            soundPropertiesReset.radius = radius;
            soundPropertiesReset.width = width;
            soundPropertiesReset.bounce = bounce;
            soundPropertiesReset.syncMap = syncMap;
            if (colorIndex >= 0 && colorIndex < colors.Length)
            {
                soundPropertiesReset.baseColor = colors[colorIndex];
            }
            else
            {
                soundPropertiesReset.baseColor = Color.white;
            }
            rigidBodyComponentsReset.position = originPosition;
            instance.SoundOriginStart(ref rigidBodyComponentsReset, ref soundPropertiesReset, ref timerReset);
        }
        SoundOrigin NewSound()
        {
            SoundOrigin sound = Instantiate(soundOriginPrefab);
            SceneManager.MoveGameObjectToScene(sound.gameObject, poolScene);
            sound.particleSoundFactory = this;
            sound.entity = entityManager.CreateEntity(soundOriginEntityArchetype);
            return sound;
        }
        public void ReclaimSound(SoundOrigin soundToRecycle)
        {
            soundPool.Add(soundToRecycle);
        }

    }
    
}