using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class UsedItemEvent
{
    public string type;
    public UnityEvent onEvent;
}

public abstract class AbstractObjectScript : MonoBehaviour
{
    [field: SerializeField] public bool NoActiveChange { get; protected set; }
    [field: SerializeField] public string[] ObjectType { get; protected set; }
    [field: SerializeField] public bool Activated { get; protected set; } = true;
    [field: SerializeField] public bool isGrabable { get; protected set; }
    [field: SerializeField] public Color ObjectColor { get; protected set; } = Color.white;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteRenderer selectRenderer;

    protected Collider2D objectCollider;
    protected Rigidbody2D objectRigidbody;

    [field: SerializeField] public bool Grabed { get; protected set; }
    [field: SerializeField] public string[] UseableObjectType { get; protected set; }
    [field: SerializeField] public Transform GrabTarget { get; set; }

    [SerializeField] protected float grabDistance = 0.5f;
    [SerializeField] protected float followSpeed = 15f;
    [SerializeField] protected float gridSize = 1f;
    private Ease ease = Ease.OutQuad;
    protected GameObject player;
    [SerializeField] private LayerMask blockLayer;

    [SerializeField] protected UnityEvent onInteract;
    [SerializeField] protected UnityEvent onGrab;
    [SerializeField] protected UnityEvent onDrop;
    [SerializeField] protected UnityEvent onActive;
    [SerializeField] protected UnityEvent onDisable;
    [SerializeField] protected UsedItemEvent[] useItemEvent;
    protected virtual void Awake()
    {
        player = FindAnyObjectByType<PlayerMovement>().gameObject;

        objectCollider = GetComponent<Collider2D>();
        objectRigidbody = GetComponent<Rigidbody2D>();
       
        if (selectRenderer != null)
            selectRenderer.color = new Color(1, 1, 1, 0);
    }

    /* =========================
       Mouse Events (Container)
       ========================= */

    public virtual void Active()
    {
        Activated = true;
        objectCollider.isTrigger = false;
        Debug.Log("Activated");
        spriteRenderer.DOFade(1f, 0.2f).SetEase(ease);
        onActive?.Invoke();
    }
    public virtual void Disable()
    {
        Activated = false;
        objectCollider.isTrigger = true;
        Debug.Log("Disabled");
        spriteRenderer.DOFade(0.4f, 0.2f).SetEase(ease);
        onDisable?.Invoke();
    }

    public virtual void MouseEnter()
    {
        if (!Activated || Grabed || selectRenderer == null) return;

        selectRenderer.DOFade(0.3f, 0.15f).SetEase(ease);
    }

    public virtual void MouseExit()
    {
        if (!Activated || Grabed || selectRenderer == null) return;

        selectRenderer.DOFade(0f, 0.15f).SetEase(ease);
    }

    public virtual void MouseDown()
    {
        if (!Activated || Grabed) return;

        Interact();
    }

    /* =========================
       Core Logic
       ========================= */

    public virtual void Interact()
    {
        onInteract?.Invoke();
    }
    public virtual void Use(string type)
    {
        foreach (UsedItemEvent a in useItemEvent)
        {
            if (a.type == type)
            {
                a.onEvent?.Invoke();
            }
        }
    }

    public virtual void Grab()
    {
        if (!isGrabable || !Activated) return;

        Grabed = true;
        
        objectCollider.enabled = false;
        if (selectRenderer != null)
            selectRenderer.DOFade(0f, 0.1f).SetEase(ease);

        if (objectRigidbody != null)
        {
            objectRigidbody.linearVelocity = Vector2.zero;
        }
        if (player.TryGetComponent<GrabObjectContainer>(out var detectTrigger))
        {
            if (detectTrigger.GrabArray.Count > 0)
            {
                GrabTarget = detectTrigger.GetLastObject().transform;
            }
            else
            {
                GrabTarget = player.transform;
            }

        }
        player.GetComponent<GrabObjectContainer>().ArrayAdd(this);
        
        onGrab?.Invoke();
    }

    protected virtual void FixedUpdate()
    {
        if (!Grabed || GrabTarget == null) return;

        Vector2 targetPos = GrabTarget.position;

        Vector2 dir = ((Vector2)transform.position - targetPos).normalized;
        targetPos += dir * grabDistance;

        if (objectRigidbody != null)
        {
            objectRigidbody.MovePosition(
                Vector2.Lerp(
                    objectRigidbody.position,
                    targetPos,
                    followSpeed * Time.fixedDeltaTime
                )
            );
        }
        else
        {
            transform.position = targetPos;
        }
    }

    public virtual void Down(Vector3 Pos)
    {
        if (!Grabed) return;

        Grabed = false;

        float snapX = Mathf.Round(Pos.x / gridSize) * gridSize;
        float snapY = Mathf.Round(Pos.y / gridSize) * gridSize;

        Vector3 dropPos = new Vector3(snapX, snapY, transform.position.z);

        transform.DOMove(dropPos, 0.1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                // 🔍 콜라이더 크기로 겹침 검사
                bool blocked = Physics2D.OverlapBox(
                    dropPos,
                    objectCollider.bounds.size,
                    0f,
                    blockLayer
                );

                if (blocked)
                {
                    // ❌ 겹치면 다시 들고 있게 처리
                    Grabed = true;
                    GrabTarget = player.transform;
                    return;
                }

                // ✅ 안전하면 콜라이더 ON
                objectCollider.enabled = true;

                if (objectRigidbody != null)
                    objectRigidbody.linearVelocity = Vector2.zero;
                onDrop?.Invoke();
            });

        player.GetComponent<GrabObjectContainer>().ArrayRemove(this);
        
    }
}
