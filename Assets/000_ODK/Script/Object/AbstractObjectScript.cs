using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public abstract class AbstractObjectScript : MonoBehaviour
{
    [field: SerializeField] public string[] ObjectType { get; protected set; }
    [field: SerializeField] public bool Activated { get; protected set; } = true;
    [field: SerializeField] public bool isGrabable { get; protected set; }

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteRenderer selectRenderer;

    protected Collider2D objectCollider;
    protected Rigidbody2D objectRigidbody;

    [field: SerializeField] public bool Grabed { get; protected set; }
    protected Transform grabTarget;

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
        spriteRenderer.DOFade(1f, 0.2f).SetEase(ease);
        onActive.Invoke();
    }
    public virtual void Disable()
    {
        Activated = false;
        spriteRenderer.DOFade(1f, 0.2f).SetEase(ease);
        onActive.Invoke();
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
        Grab();
    }

    public virtual void Grab()
    {
        if (!isGrabable || !Activated) return;

        Grabed = true;
        grabTarget = player.transform;
        objectCollider.enabled = false;
        if (selectRenderer != null)
            selectRenderer.DOFade(0f, 0.1f).SetEase(ease);

        if (objectRigidbody != null)
        {
            objectRigidbody.linearVelocity = Vector2.zero;
        }
        player.GetComponent<GrabObjectContainer>().ArrayAdd(this);
    }

    protected virtual void FixedUpdate()
    {
        if (!Grabed || grabTarget == null) return;

        Vector2 targetPos = grabTarget.position;

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
                    grabTarget = player.transform;
                    return;
                }

                // ✅ 안전하면 콜라이더 ON
                objectCollider.enabled = true;

                if (objectRigidbody != null)
                    objectRigidbody.linearVelocity = Vector2.zero;
            });

        player.GetComponent<GrabObjectContainer>().ArrayRemove(this);
    }
}
