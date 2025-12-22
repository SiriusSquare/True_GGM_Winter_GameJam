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
        AbstractObjectScript[] objects = FindObjectsByType<AbstractObjectScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        switched = !switched;

        foreach (AbstractObjectScript obj in objects)
        {
            // 2. 자기 자신 제외 로직 확인
            if (obj == this) continue;

            // 3. 색상 비교 값 확인

            if (obj.ObjectColor == ObjectColor && !obj.NoActiveChange)
            {
                if (obj.Activated) obj.Disable();
                else obj.Active();
            }
        }
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

}
