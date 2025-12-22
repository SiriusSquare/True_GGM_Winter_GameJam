using System.Collections.Generic;
using UnityEngine;

public class GrabObjectContainer : MonoBehaviour
{
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private Vector2 gridSize = Vector2.one; // 타일 1칸 크기

    private AbstractObjectScript currentHover;
    [field:SerializeField] public List<AbstractObjectScript> GrabArray { get; private set; }
    void Update()
    {
        UpdateHover();
        HandleClick();
    }

    public void ArrayAdd(AbstractObjectScript abstractObjectScript)
    {
        GrabArray.Add(abstractObjectScript);
    }
    public void ArrayRemove(AbstractObjectScript abstractObjectScript)
    {
        GrabArray.Remove(abstractObjectScript);
    }
    public AbstractObjectScript GetObjectByType(string type)
    {
        foreach (var obj in GrabArray)
        {
            foreach (var objType in obj.ObjectType)
            {
                if (objType == type)
                    return obj;
            }
        }
        return null;
    }
    void UpdateHover()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mousePos, objectLayer);

        AbstractObjectScript nextHover = null;

        if (hit != null)
        {
            AbstractObjectScript obj = hit.GetComponent<AbstractObjectScript>();

            if (obj != null && IsInside3x3(obj.transform.position))
            {
                nextHover = obj;
            }
        }

        if (nextHover == currentHover) return;

        if (currentHover != null)
            currentHover.MouseExit();

        currentHover = nextHover;

        if (currentHover != null)
            currentHover.MouseEnter();
    }

    bool IsInside3x3(Vector2 worldPos)
    {
        Vector2 center = transform.position;
        Vector2 halfSize = gridSize * 1.5f; // 3칸 / 2

        return Mathf.Abs(worldPos.x - center.x) <= halfSize.x &&
               Mathf.Abs(worldPos.y - center.y) <= halfSize.y;
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
