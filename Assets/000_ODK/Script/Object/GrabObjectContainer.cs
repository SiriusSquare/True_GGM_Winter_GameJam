using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabObjectContainer : MonoBehaviour
{
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask noDropLayer;
    [SerializeField] private Vector2 gridSize = Vector2.one; // 타일 1칸 크기
    [SerializeField] private int maxCapacity = 1;

    private Vector2Int mouseVector;
    [SerializeField] private Transform mouseEffect;

    private AbstractObjectScript currentHover;
    [field:SerializeField] public List<AbstractObjectScript> GrabArray { get; private set; }

    void Update()
    {
        UpdateHover();
        HandleClick();
    }

    public float fadeDistance = 3f; // 마우스와의 거리 기준
    public float fadeDuration = 0.2f;

    public void FixedUpdate()
    {
        if (Camera.main == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 월드 위치 → 그리드 정수 좌표(Vector2Int)로 변환
        Vector2Int snappedGrid = new Vector2Int(
            Mathf.RoundToInt(mouseWorldPos.x / gridSize.x),
            Mathf.RoundToInt(mouseWorldPos.y / gridSize.y)
        );

        // 위치 변경 시만 이동
        if (snappedGrid != mouseVector)
        {
            mouseVector = snappedGrid;

            Vector3 snappedPos = new Vector3(
                mouseVector.x * gridSize.x,
                mouseVector.y * gridSize.y,
                0f
            );

            if (mouseEffect != null)
            {
                mouseEffect.DOMove(snappedPos, 0.1f).SetEase(Ease.OutQuad);
            }
        }

        if (mouseEffect != null)
        {
            // 1. 실제 마우스 월드 위치와 효과의 현재 위치 사이의 거리 계산
            float distance = Vector3.Distance(transform.position, mouseEffect.position);

            // 2. 거리 조건에 따른 Fade 처리 (1.5 이상이면 투명하게, 미만이면 불투명하게)
            // SpriteRenderer 또는 CanvasGroup 등 컴포넌트에 따라 DOFade를 호출합니다.
            // 여기서는 SpriteRenderer 기준으로 예시를 작성합니다.
            float targetAlpha = !IsInside3x3(snappedGrid) ? 0f : 1f;

            // GetComponent를 매번 호출하는 것보다 캐싱해두는 것이 좋지만, 
            // 일단 구조에 맞춰 작성합니다.
            SpriteRenderer sr = mouseEffect.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 현재 알파값과 목표값이 다를 때만 트윈 실행 (최적화)
                if (!Mathf.Approximately(sr.color.a, targetAlpha))
                {
                    sr.DOKill(); // 기존 트윈 중단
                    sr.DOFade(targetAlpha, 0.1f).SetEase(Ease.OutQuad);
                }
            }
        }

    }




    public void ArrayAdd(AbstractObjectScript abstractObjectScript)
    {
        GrabArray.Add(abstractObjectScript);
    }
    public void ArrayRemove(AbstractObjectScript abstractObjectScript)
    {
        if (!GrabArray.Contains(abstractObjectScript)) return;

        GrabArray.Remove(abstractObjectScript);

        // 리스트 재정렬: 남은 오브젝트들의 타겟을 다시 설정
        for (int i = 0; i < GrabArray.Count; i++)
        {
            if (i == 0)
            {
                GrabArray[i].GrabTarget = this.transform; // 첫 번째는 컨테이너(플레이어)를 따라감
            }
            else
            {
                GrabArray[i].GrabTarget = GrabArray[i - 1].transform; // 나머지는 앞의 오브젝트를 따라감
            }
        }
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
            if (currentHover != null && currentHover.isGrabable)
            {
                // 최대 개수 초과 시 가장 먼저 Grab한 오브젝트 내려놓기
                if (GrabArray.Count >= maxCapacity)
                {
                    var first = GrabArray[0];
                    if (first != null)
                    {
                        first.Down(currentHover.transform.position);
                    }
                }

                // 새 오브젝트 Grab
                currentHover.Grab();
            }
            else if (currentHover != null)
            {
                currentHover.MouseDown();
            }
        }

        // 우클릭 : 사거리 내 가장 가까운 오브젝트 내려놓기
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

        if (!IsInside3x3(mouseWorldPos)) return;

        // 🔹 수정된 부분: OverlapPoint 대신 사용 (트리거 포함 감지)
        // true를 인자로 넣으면 트리거 콜라이더도 감지합니다.
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, noDropLayer);

        // 만약 일반 OverlapPoint가 트리거를 못 잡는다면 아래 방식을 사용하세요
        /*
        Collider2D hit = null;
        Collider2D[] results = Physics2D.OverlapPointAll(mouseWorldPos, noDropLayer);
        if (results.Length > 0) hit = results[0];
        */

        if (hit != null) return;

        AbstractObjectScript target = GetLastObject();
        if (target != null)
        {
            target.Down(mouseWorldPos);
        }
    }

    void Swap()
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

        AbstractObjectScript target = GrabArray.First();
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
