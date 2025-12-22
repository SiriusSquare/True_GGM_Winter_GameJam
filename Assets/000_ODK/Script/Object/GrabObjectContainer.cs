using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabObjectContainer : MonoBehaviour
{
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask noDropLayer;
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
    public AbstractObjectScript GetLastObject()
    {
        if (GrabArray.Count == 0)
            return null;
        return GrabArray[GrabArray.Count - 1];
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
        // 좌클릭 : 선택된 오브젝트
        if (Input.GetMouseButtonDown(0))
        {
            if (currentHover != null)
            {
                currentHover.MouseDown();
            }
        }

        // 우클릭 : 사거리 내 가장 가까운 오브젝트
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }
    void HandleRightClick()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 mp = Input.mousePosition;
        mp.z = -cam.transform.position.z;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(mp);

        // ① 사거리 체크
        if (!IsInside3x3(mouseWorldPos))
            return;

        // ② 해당 위치에 NoDrop 오브젝트가 있으면 드롭 금지
        if (Physics2D.OverlapPoint(mouseWorldPos, noDropLayer) != null)
            return;

        AbstractObjectScript target = GetLastObject();
        if (target != null)
        {
            target.Down(mouseWorldPos);
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
