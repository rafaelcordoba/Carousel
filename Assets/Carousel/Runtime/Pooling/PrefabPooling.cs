using System.Collections.Generic;
using UnityEngine;

namespace Carousel.Runtime.Pooling
{
    public class PrefabPooling
    {
        private const int MaxPoolSize = 3;
        private readonly Dictionary<GameObject, Transform> _containers = new();
        private readonly Dictionary<GameObject, Queue<PoolObject>> _pools = new();

        public PoolObject Get(GameObject prefab)
        {
            if (!_containers.ContainsKey(prefab))
            {
                var go = new GameObject($"Pool ({prefab.name})");
                _containers.Add(prefab, go.transform);
            }

            var container = _containers[prefab];

            if (!_pools.ContainsKey(prefab))
                _pools.Add(prefab, new Queue<PoolObject>(MaxPoolSize));

            var pool = _pools[prefab];
            if (pool.Count > 0)
                return pool.Dequeue();

            var newInstance = Object.Instantiate(prefab, container);
            var poolObject = new PoolObject(prefab, newInstance);
            return poolObject;
        }

        public void Return(PoolObject poolObject)
        {
            var pool = _pools[poolObject.Prefab];

            if (pool.Count >= MaxPoolSize)
            {
                Object.Destroy(poolObject.Instance);
                return;
            }

            var container = _containers[poolObject.Prefab];
            pool.Enqueue(poolObject);
            poolObject.Instance.transform.SetParent(container);
            poolObject.Instance.SetActive(false);
        }
    }
}