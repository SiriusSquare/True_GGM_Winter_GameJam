using _JJM.Script.CustomEditor;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [SerializeField] private Sprite _openImage;
    [SerializeField] private Sprite _closeImage;
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private Vector2 _upOffset;
    [SerializeField] private Vector2 _upSize;
    [SerializeField] private Vector2 _downOffset;
    [SerializeField] private Vector2 _downSize;
    [SerializeField] private Vector2 _allOffset;
    [SerializeField] private Vector2 _allSize;

    [SerializeField, ReadOnly] private bool open = false;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private bool upPassed;
    private bool downPassed;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private bool downHit = false;
    private bool upHit = false;

    private bool s = false;

    private void Update()
    {
        if (Physics2D.OverlapBox((Vector2)transform.position + _upOffset, _upSize, 0, _layerMask))
        {
            upHit = true;
        }
        if (Physics2D.OverlapBox((Vector2)transform.position + _downOffset, _downSize, 0, _layerMask))
        {
            downHit = true;
        }
        bool allHit = Physics2D.OverlapBox((Vector2)transform.position + _allOffset, _allSize, 0, _layerMask);

        if (!allHit)
        {
            upHit = false;
            downHit = false;
        }

        if (upHit && downHit && allHit)
        {
            if (!s)
            {
                SoundManager.Instance.PlaySFX(4);
                Debug.Log("d");
                s = true;
            }
            open = true;
        }
        

        _spriteRenderer.sprite = open ? _openImage : _closeImage;
        _collider.isTrigger = !open;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.4f);
        Gizmos.DrawCube((Vector2)transform.position + _upOffset, _upSize);
        Gizmos.color = new Color(0, 1, 1, 0.4f);
        Gizmos.DrawCube((Vector2)transform.position + _downOffset, _downSize);
        Gizmos.color = new Color(1, 1, 0, 0.4f);
        Gizmos.DrawCube((Vector2)transform.position + _allOffset, _allSize);
    }
}