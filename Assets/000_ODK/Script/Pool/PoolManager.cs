using Code.Core;
using Code.Core.Pooling;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 완전 자동 풀링 매니저.
/// prefab 또는 이름(string) 기반으로 자동 생성/관리.
/// 초기 세팅 SO로 미리 프리로드 가능.
/// </summary>
public class PoolManager : MonoSingleton<PoolManager>
{
    [Header("초기 풀 셋업 리스트 (선택사항)")]
    [SerializeField] private PoolSetupListSO initialPoolSetup;

    private readonly Dictionary<string, Pool> _pools = new();
    private readonly Dictionary<string, GameObject> _prefabCache = new();

    private void Awake()
    {
        if (initialPoolSetup != null)
        {
            foreach (var setup in initialPoolSetup.poolList)
            {
                if (setup == null || setup.prefab == null) continue;

                string key = setup.poolName;
                if (_pools.ContainsKey(key)) continue;

                Debug.Log($"[PoolManager] 초기 풀 생성: {key} (Count: {setup.count})");
                _pools[key] = new Pool(setup.prefab, transform, setup.count);
                _prefabCache[key] = setup.prefab;
            }
        }
    }
    

    public GameObject Pop(GameObject prefab, int initCount = 1)
    {
        if (prefab == null)
        {
            Debug.LogError("[PoolManager] Tried to Pop null prefab.");
            return null;
        }

        string key = prefab.name;
        Debug.Log("Pop" + key);
        if (!_pools.ContainsKey(key))
        {
            Debug.Log($"[PoolManager] Auto-creating pool for {key}");
            _pools[key] = new Pool(prefab, transform, initCount);
            _prefabCache[key] = prefab;
        }

        var pool = _pools[key];
        var item = pool.Pop();
        item?.ResetItem();
        return item?.GameObject;
    }
    public void Remove(IPoolable item)
    {
        if (_pools.TryGetValue(item.ItemName, out var pool))
        {
            pool.Remove(item);
        }
    }
    public GameObject Pop(string prefabName, int initCount = 0)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("[PoolManager] Tried to Pop with empty _name.");
            return null;
        }
        if (!_prefabCache.TryGetValue(prefabName, out var prefab))
        {
            prefab = Resources.Load<GameObject>(prefabName);

            if (prefab == null)
            {
                Debug.LogError($"[PoolManager] Cannot find prefab named '{prefabName}' in cache or Resources.");
                return null;
            }

            _prefabCache[prefabName] = prefab;
        }

        return Pop(prefab, initCount);
    }

    public void Push(GameObject obj)
    {
        if (obj == null) return;

        var poolable = obj.GetComponent<IPoolable>();
        if (poolable == null)
        {
            Debug.LogWarning($"[PoolManager] {obj.name} has no IPoolable, destroying instead.");
            Destroy(obj);
            return;
        }

        string key = poolable.ItemName;
        if (!_pools.ContainsKey(key))
        {
            Debug.Log($"[PoolManager] Auto-creating pool for {key}");
            _pools[key] = new Pool(obj, transform, 0);
            _prefabCache[key] = obj;
        }

        _pools[key].Push(poolable);
    }
}
