using DG.Tweening;
using UnityEngine;

public abstract class AbstractObjectScript : MonoBehaviour
{
    [field: SerializeField] public string[] ObjectType { get; protected set; }
    [field: SerializeField] public bool Activated { get; protected set; } = true;
    [field: SerializeField] public bool isGrabable { get; protected set; }

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteRenderer selectRenderer;

    protected Collider2D objectCollider;
    protected Rigidbody2D objectRigidbody;

    protected bool Grabed;
    protected Transform grabTarget;

    [SerializeField] protected float grabDistance = 0.5f;
    [SerializeField] protected float followSpeed = 15f;
    [SerializeField] protected float gridSize = 1f;

    protected GameObject player;

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

    public virtual void MouseEnter()
    {
        if (!Activated || Grabed || selectRenderer == null) return;

        selectRenderer.DOFade(0.3f, 0.15f);
    }

    public virtual void MouseExit()
    {
        if (!Activated || Grabed || selectRenderer == null) return;

        selectRenderer.DOFade(0f, 0.15f);
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

        if (selectRenderer != null)
            selectRenderer.DOFade(0f, 0.1f);

        if (objectRigidbody != null)
        {
            objectRigidbody.linearVelocity = Vector2.zero;
            objectRigidbody.isKinematic = true;
        }
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

        Vector3 worldPos = Pos;

        float snapX = Mathf.Round(worldPos.x / gridSize) * gridSize;
        float snapY = Mathf.Round(worldPos.y / gridSize) * gridSize;

        transform.position = new Vector3(snapX, snapY, transform.position.z);

        if (objectRigidbody != null)
            objectRigidbody.isKinematic = false;
    }
}
