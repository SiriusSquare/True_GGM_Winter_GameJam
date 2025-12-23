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

    private void Update()
    {
        bool upHit = Physics2D.OverlapBox(
            transform.position + (Vector3)_upOffset,
            _upSize,
            0,
            _layerMask
        );

        bool downHit = Physics2D.OverlapBox(
            transform.position + (Vector3)_downOffset,
            _downSize,
            0,
            _layerMask
        );

        if (upHit)
            upPassed = true;

        if (downHit)
            downPassed = true;

        if (upPassed && downPassed)
        {
            open = true;
        }

        _spriteRenderer.sprite = open ? _openImage : _closeImage;
        _collider.isTrigger = !open;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + (Vector3)_upOffset, _upSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position + (Vector3)_downOffset, _downSize);
    }
}
