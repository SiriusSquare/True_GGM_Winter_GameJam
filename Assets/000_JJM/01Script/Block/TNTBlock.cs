using UnityEngine;

public class TNTBlock : MonoBehaviour
{
    [SerializeField] private GameObject _tntPrefab;

    public void ExplosionPlay()
    {
        GameObject ex = PoolManager.Instance.Pop(_tntPrefab);
        ex.transform.position = transform.position;
        gameObject.SetActive(false);
    }
}
