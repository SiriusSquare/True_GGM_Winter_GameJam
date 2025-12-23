using Unity.VisualScripting;
using UnityEngine;

public class SwichActive : AbstractObjectScript
{
    [SerializeField] private bool switched = false;
    [SerializeField] private SpriteRenderer switchedOnSpriteRenderer;
    [SerializeField] private SpriteRenderer switchedOffSpriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        switchedOnSpriteRenderer.color = ObjectColor;
        switchedOffSpriteRenderer.color = ObjectColor * new Color(0.7f, 0.7f, 0.7f);
        
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
        SoundManager.Instance.PlaySFX(1);
    }


    public override void Interact()
    {
        AbstractObjectScript[] objects = FindObjectsByType<AbstractObjectScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        switched = !switched;

        // 1. 현재 스위치의 색상을 Hex 코드로 변환 (예: "FF0000")
        string myColorHex = UnityEngine.ColorUtility.ToHtmlStringRGB(ObjectColor);

        foreach (AbstractObjectScript obj in objects)
        {
            if (obj == this) continue;

            // 2. 대상 오브젝트의 색상을 Hex 코드로 변환
            string targetColorHex = UnityEngine.ColorUtility.ToHtmlStringRGB(obj.ObjectColor);

            // 3. 문자열 비교 (대소문자 구분 없이 비교됨)
            if (myColorHex == targetColorHex && !obj.NoActiveChange)
            {
                if (obj.Activated) obj.Disable();
                else obj.Active();
            }
        }

        // 스위치 외형 업데이트 로직
        UpdateVisuals();

        base.Interact();
    }

    private void UpdateVisuals()
    {
        switchedOnSpriteRenderer.gameObject.SetActive(switched);
        switchedOffSpriteRenderer.gameObject.SetActive(!switched);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        switchedOnSpriteRenderer.color = ObjectColor;
        switchedOffSpriteRenderer.color = ObjectColor * new Color(0.7f, 0.7f, 0.7f);
    }
#endif

}
