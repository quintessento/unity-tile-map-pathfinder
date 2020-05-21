using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple pooling mechanism that uses a generic list. Add scene items to disable and stash them away. 
/// Get items to re-enable and use them without the need of instantiating.
/// </summary>
/// <typeparam name="T">Type of the objects stored in the pool.</typeparam>
public class ListPool<T> where T : MonoBehaviour
{
    private readonly List<T> _pooledObjects = new List<T>();

    /// <summary>
    /// Number of items currently in the pool.
    /// </summary>
    public int Count => _pooledObjects.Count;

    /// <summary>
    /// Stashes the object by adding it to the pool and disabling it in the scene.
    /// </summary>
    /// <param name="obj">Stashed object.</param>
    public void Add(T obj)
    {
        _pooledObjects.Add(obj);
        obj.gameObject.SetActive(false);
    }

    /// <summary>
    /// Picks an object from the pool and enables it, thus removing it from the pool.
    /// </summary>
    /// <returns>Object from the pool, if any; otherwise null.</returns>
    public T Get()
    {
        if (_pooledObjects.Count > 0)
        {
            int index = 0;
            T obj = _pooledObjects[index];
            _pooledObjects.RemoveAt(index);
            obj.gameObject.SetActive(true);
            return obj;
        }

        return null;
    }
}
