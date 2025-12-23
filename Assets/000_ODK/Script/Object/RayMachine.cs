using UnityEngine;

public class RayMachine : AbstractObjectScript
{
    [Header("Ray Settings")]
    public Vector2Int dir = Vector2Int.right; // 발사 방향
    [SerializeField] private float maxDistance = 50f; // 최대 거리
    [SerializeField] private LayerMask blockRayLayer; // 레이저를 막을 레이어

    private LineRenderer lineRenderer;
    private bool layenable = true;
    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();

        // 라인렌더러 초기 설정 (Position Count가 2개 필요: 시작점, 끝점)
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
        }
    }
    public void RayEnable()
    {
        layenable = true;
    }
    public void RayDisable()
    {
        layenable = false;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (lineRenderer == null) return;

        // 활성화 상태일 때만 레이저 발사
        if (Activated && layenable)
        {
            lineRenderer.enabled = true;
            UpdateRay();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
    private void UpdateRay()
    {
        Vector3 startPos = transform.position;
        Vector3 direction = new Vector3(dir.x, dir.y, 0).normalized;

        // 시작 지점 오프셋 (자기 자신 통과용)
        Vector3 rayStartPoint = startPos + (direction * 0.6f);
        lineRenderer.SetPosition(0, startPos);

        // 🔹 방법 1: 현재 레이캐스트가 트리거를 무시하도록 전역 설정 잠시 변경
        bool oldQueriesHitTriggers = Physics2D.queriesHitTriggers;
        Physics2D.queriesHitTriggers = false; // 트리거 무시 설정

        // 레이캐스트 발사
        RaycastHit2D hit = Physics2D.Raycast(rayStartPoint, direction, maxDistance, blockRayLayer);

        // 🔹 설정을 다시 원래대로 복구 (다른 로직에 영향 주지 않기 위함)
        Physics2D.queriesHitTriggers = oldQueriesHitTriggers;

        Vector3 endPos;

        if (hit.collider != null)
        {
            endPos = hit.point;

            // 트리거가 아닌 콜라이더만 감지되었으므로 바로 로직 수행
            if (hit.collider.TryGetComponent<AbstractObjectScript>(out var objScript))
            {
                if (objScript.Triggerable)
                {
                    objScript.Trigger();
                }
            }
        }
        else
        {
            endPos = startPos + (direction * maxDistance);
        }

        lineRenderer.SetPosition(1, endPos);
    }

    // 왼쪽으로 90도 회전 (반시계 방향)
    public void RotateLeft()
    {
        // (x, y) -> (-y, x)
        // 예: (1, 0) [우] -> (0, 1) [상] -> (-1, 0) [좌] -> (0, -1) [하]
        dir = new Vector2Int(-dir.y, dir.x);
        Debug.Log($"[RayMachine] 왼쪽 회전: {dir}");
    }

    // 오른쪽으로 90도 회전 (시계 방향)
    public void RotateRight()
    {
        // (x, y) -> (y, -x)
        // 예: (1, 0) [우] -> (0, -1) [하] -> (-1, 0) [좌] -> (0, 1) [상]
        dir = new Vector2Int(dir.y, -dir.x);
        Debug.Log($"[RayMachine] 오른쪽 회전: {dir}");
    }
}