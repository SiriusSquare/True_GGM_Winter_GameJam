using UnityEngine;
using System.Collections;

public class ExplosionObject : MonoBehaviour, IPoolable
{
    [SerializeField] private Vector2 _size;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private Color _color;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private ParticleSystem _explosionParticle;

    public string ItemName => "Explosion";

    public GameObject GameObject => gameObject;

    private void OnEnable()
    {
        _explosionParticle.Play();
        StartCoroutine(EndTime());
    }

    private void Update()
    {
        Collider2D[] hit = Physics2D.OverlapBoxAll(transform.position + (Vector3)_offset, _size, 0, _layerMask);
        
        foreach (Collider2D col in hit)
        {
            ParticleManager.Instance.ParticlePlay(ParticleEnum.BlockBreakParticle, col.transform.position);
            col.gameObject.SetActive(false);
        }
    }

    private IEnumerator EndTime()
    {
        yield return new WaitForSeconds(_explosionParticle.main.startLifetime.constant);
        PoolManager.Instance.Push(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        
        Gizmos.DrawCube(transform.position + (Vector3)_offset, _size);
    }

    public void ResetItem()
    {
        
    }
}
