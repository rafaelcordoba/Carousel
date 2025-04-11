using UnityEngine;

namespace Pooling
{
    public class PoolObject
    {
        public readonly GameObject Instance;
        public readonly GameObject Prefab;

        public PoolObject(GameObject prefab, GameObject instance)
        {
            Prefab = prefab;
            Instance = instance;
        }
    }
}