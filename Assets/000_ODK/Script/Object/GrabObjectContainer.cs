using UnityEngine;
using System.Collections.Generic;

public class GrabObjectContainer : MonoBehaviour
{
    [SerializeField] private Vector2 gridSize = Vector2.one; // 타일 1칸 크기
    [SerializeField] private LayerMask objectLayer;

    private List<AbstractObjectScript> cachedObjects = new();
    private AbstractObjectScript currentHover;

    void Update()
    {
        UpdateHover();
        HandleClick();
    }

    void UpdateHover()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        cachedObjects.Clear();

        // 3x3 타일 범위
        Vector2 boxSize = gridSize * 3f;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            transform.position,
            boxSize,
            0f,
            objectLayer
        );

        foreach (var hit in hits)
        {
            AbstractObjectScript obj =
                hit.GetComponent<AbstractObjectScript>();

            if (obj != null && obj.Activated)
            {
                cachedObjects.Add(obj);
            }
        }

        // 마우스 위치 기준 가장 가까운 오브젝트 선택
        AbstractObjectScript nearest = null;
        float minDist = float.MaxValue;

        foreach (var obj in cachedObjects)
        {
            float dist = Vector2.Distance(mousePos, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = obj;
            }
        }

        if (nearest != currentHover)
        {
            if (currentHover != null)
                currentHover.MouseExit();

            currentHover = nearest;

            if (currentHover != null)
                currentHover.MouseEnter();
        }
    }

    void HandleClick()
    {
        if (currentHover == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            currentHover.MouseDown();
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            transform.position,
            gridSize * 3f
        );
    }
#endif
}
