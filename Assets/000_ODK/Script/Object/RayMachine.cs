using UnityEngine;

public class RayMachine : AbstractObjectScript
{
    [Header("Ray Settings")]
    public Vector2Int dir = Vector2Int.right;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private LayerMask blockRayLayer;

    private LineRenderer lineRenderer;
    [SerializeField] private bool layenable = true;

    // 🔹 현재 레이저에 맞고 있는 오브젝트를 추적하기 위한 변수
    private AbstractObjectScript lastHitObject;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null) lineRenderer.positionCount = 2;
    }

    public void RayEnable() => layenable = true;
    public void RayDisable() => layenable = false;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (lineRenderer == null) return;

        if (Activated && layenable)
        {
            lineRenderer.enabled = true;
            UpdateRay();
        }
        else
        {
            // 기계가 꺼질 때도 기존에 맞던 오브젝트가 있다면 Trigger2 실행
            if (lineRenderer.enabled)
            {
                ClearLastHit();
            }
            lineRenderer.enabled = false;
        }
    }

    private void UpdateRay()
    {
        Vector3 startPos = transform.position;
        Vector3 direction = new Vector3(dir.x, dir.y, 0).normalized;
        Vector3 rayStartPoint = startPos + (direction * 0.6f);
        lineRenderer.SetPosition(0, startPos);

        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false;

        RaycastHit2D hit = Physics2D.Raycast(rayStartPoint, direction, maxDistance, blockRayLayer);

        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;

        Vector3 endPos;
        AbstractObjectScript currentHitObject = null;

        if (hit.collider != null)
        {
            endPos = hit.point;
            if (hit.collider.TryGetComponent<AbstractObjectScript>(out var objScript))
            {
                currentHitObject = objScript;
            }
        }
        else
        {
            endPos = startPos + (direction * maxDistance);
        }

        // 🔹 벗어남 감지 로직
        HandleObjectChange(currentHitObject);

        lineRenderer.SetPosition(1, endPos);
    }

    private void HandleObjectChange(AbstractObjectScript currentHit)
    {
        // 1. 이전 대상과 현재 대상이 다를 때
        if (lastHitObject != currentHit)
        {
            // 이전에 맞고 있던 애가 있었다면 -> 레이저에서 벗어남(Trigger2)
            if (lastHitObject != null)
            {
                Debug.Log($"[RayMachine] {lastHitObject.name}이 레이저에서 벗어남.");
                lastHitObject.Trigger2(); // 👈 여기에 새로 만든 Trigger2 메서드 호출
            }

            // 새로운 대상이 생겼다면 -> 레이저에 들어옴(Trigger)
            if (currentHit != null && currentHit.Triggerable)
            {
                Debug.Log($"[RayMachine] {currentHit.name}이 레이저에 맞음.");
                currentHit.Trigger();
            }

            // 현재 대상을 저장
            lastHitObject = currentHit;
        }
        else
        {
            // 2. 같은 대상에게 계속 레이저를 쏘고 있을 때 (필요 시 지속 처리 가능)
            if (lastHitObject != null && lastHitObject.Triggerable)
            {
                // lastHitObject.Trigger(); // 매 프레임 실행하고 싶다면 주석 해제
            }
        }
    }

    // 레이저가 꺼질 때를 대비한 정리 함수
    private void ClearLastHit()
    {
        if (lastHitObject != null)
        {
            lastHitObject.Trigger2();
            lastHitObject = null;
        }
    }

    public void RotateLeft() => dir = new Vector2Int(-dir.y, dir.x);
    public void RotateRight() => dir = new Vector2Int(dir.y, -dir.x);
}