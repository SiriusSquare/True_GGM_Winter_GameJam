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

        UpdateVisuals();
    }

    private void OnMouseDown()
    {
        Interact();
    }

    public override void Interact()
    {
        switched = !switched;

        AbstractObjectScript[] objects =
            FindObjectsByType<AbstractObjectScript>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        string myColorHex = ColorUtility.ToHtmlStringRGB(ObjectColor);

        foreach (AbstractObjectScript obj in objects)
        {
            if (obj == this) continue;
            if (obj.NoActiveChange) continue;

            string targetColorHex =
                ColorUtility.ToHtmlStringRGB(obj.ObjectColor);

            if (myColorHex != targetColorHex) continue;

            if (obj.Activated)
                obj.Disable();
            else
                obj.Active();
        }

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
        if (switchedOnSpriteRenderer != null)
            switchedOnSpriteRenderer.color = ObjectColor;

        if (switchedOffSpriteRenderer != null)
            switchedOffSpriteRenderer.color =
                ObjectColor * new Color(0.7f, 0.7f, 0.7f);
    }
#endif
}
