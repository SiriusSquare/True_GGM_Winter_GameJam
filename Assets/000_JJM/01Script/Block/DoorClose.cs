using _JJM.Script.CustomEditor;
using UnityEngine;

public class DoorClose : MonoBehaviour
{
    [SerializeField] private Sprite _openImage;
    [SerializeField] private Sprite _closeImage;

    [SerializeField] private Vector2 _upOffset;
    [SerializeField] private Vector2 _upSize;
    [SerializeField] private Vector2 _downOffset;
    [SerializeField] private Vector2 _downSize;

    [SerializeField, ReadOnly] private bool open = false;

    private SpriteRenderer _spriteRenderer;

    bool upPassed;
    bool downPassed;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        bool upHit = Physics2D.OverlapBox(
            transform.position + (Vector3)_upOffset,
            _upSize,
            0
        );

        bool downHit = Physics2D.OverlapBox(
            transform.position + (Vector3)_downOffset,
            _downSize,
            0
        );

        if (upHit)
            upPassed = true;

        if (downHit)
            downPassed = true;

        if (upPassed && downPassed)
        {
            open = true;
        }

        if (open)
        {

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + (Vector3)_upOffset, _upSize);

        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(transform.position + (Vector3)_downOffset, _downSize);
    }
}
