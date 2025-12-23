using UnityEngine;

public class RayMachine : AbstractObjectScript
{
    [Header("Ray Settings")]
    public Vector2Int dir = Vector2Int.right; // 발사 방향
    [SerializeField] private float maxDistance = 50f; // 최대 거리
    [SerializeField] private LayerMask blockRayLayer; // 레이저를 막을 레이어

    private LineRenderer lineRenderer;

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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (lineRenderer == null) return;

        // 활성화 상태일 때만 레이저 발사
        if (Activated)
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
        // 1. 시작 위치 설정
        Vector3 startPos = transform.position;
        lineRenderer.SetPosition(0, startPos);

        // 2. 방향 벡터 계산 (Vector2Int를 Vector3로 변환)
        Vector3 direction = new Vector3(dir.x, dir.y, 0).normalized;

        // 3. 레이캐스트 발사
        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, blockRayLayer);

        Vector3 endPos;

        if (hit.collider != null)
        {
            endPos = hit.point;
            if (endPos != lineRenderer.GetPosition(1))
            {
                if (hit.collider.gameObject.GetComponent<AbstractObjectScript>().Triggerable)
                {
                    hit.collider.gameObject.GetComponent<AbstractObjectScript>().Trigger();

                }
                
            }

        }
        else
        {
            endPos = startPos + (direction * maxDistance);
        }
        
        lineRenderer.SetPosition(1, endPos);
    }
}