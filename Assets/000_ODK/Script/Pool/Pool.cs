using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private Stack<IPoolable> _pool = new();
    private Transform _parent;
    private GameObject _prefab;

    public string PoolName { get; private set; }

    public Pool(GameObject prefab, Transform parent, int initCount = 0)
    {
        _parent = parent;
        _prefab = prefab;
        PoolName = prefab.name;

        for (int i = 0; i < initCount; i++)
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.name = PoolName;
            obj.SetActive(false);

            var item = obj.GetComponent<IPoolable>();
            if (item == null)
            {
                Debug.LogError($"[Pool] {_prefab.name} must have IPoolable.");
                Object.Destroy(obj);
                continue;
            }

            _pool.Push(item);
        }
    }

    public IPoolable Pop()
    {
        IPoolable item;
        if (_pool.Count > 0)
        {
            item = _pool.Pop();
            item.GameObject.SetActive(true);
        }
        else
        {
            var obj = Object.Instantiate(_prefab, _parent);
            obj.name = PoolName;
            item = obj.GetComponent<IPoolable>();
        }

        return item;
    }
    public void Remove(IPoolable item)
    {
        var tempList = new List<IPoolable>(_pool);
        if (tempList.Remove(item))
        {
            _pool = new Stack<IPoolable>(tempList);
        }
    }
    public void Push(IPoolable item)
    {
        if (item == null || item.GameObject == null)
            return;

        item.GameObject.SetActive(false);
        _pool.Push(item);
    }
}
