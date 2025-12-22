using UnityEngine;

public class SwichActive : AbstractObjectScript
{
    [SerializeField] private bool switched = false;
    [SerializeField] private SpriteRenderer switchedOnSpriteRenderer;
    [SerializeField] private SpriteRenderer switchedOffSpriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        if (switched)
        {
            switchedOnSpriteRenderer.gameObject.SetActive(true);
            switchedOffSpriteRenderer.gameObject.SetActive(false);
        }
        else
        {
            switchedOnSpriteRenderer.gameObject.SetActive(false);
            switchedOffSpriteRenderer.gameObject.SetActive(true);
        }
    }
    public override void Interact()
    {
        // 1. 전체 오브젝트 몇 개 찾았는지 확인
        AbstractObjectScript[] objects = FindObjectsByType<AbstractObjectScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"[Switch] 총 {objects.Length}개의 오브젝트를 찾았습니다.");

        switched = !switched;

        foreach (AbstractObjectScript obj in objects)
        {
            // 2. 자기 자신 제외 로직 확인
            if (obj == this) continue;

            // 3. 색상 비교 값 확인
            Debug.Log($"[Switch] 체크 중: {obj.name} | 내 색상: {ObjectColor} | 대상 색상: {obj.ObjectColor}");

            if (obj.ObjectColor == ObjectColor)
            {
                Debug.Log($"[Switch] {obj.name} 일치! 상태 변경 시도");
                if (obj.Activated) obj.Disable();
                else obj.Active();
            }
        }
        // ... 비주얼 업데이트 코드
    }

}
