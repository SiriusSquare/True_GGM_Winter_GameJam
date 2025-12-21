using DG.Tweening;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public abstract class AbstractObjectScript : MonoBehaviour
{
    [field: SerializeField] public bool Activated { get; protected set; }
    [field: SerializeField] public Color ObjectColor { get; protected set; }

    [field: SerializeField] public bool isGrabable { get; protected set; }
    [field: SerializeField] public bool Grabed { get; protected set; }

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected SpriteRenderer selectRenderer;
    protected Collider2D objectCollider;
    protected Rigidbody2D objectRigidbody;
    [SerializeField] protected float grabDistance = 0.5f;
    protected Transform grabTarget;
    [SerializeField] private float gridSize = 1f;

    
    protected virtual void Awake()
    {
        if (ObjectColor == null)
        {
            ObjectColor = spriteRenderer.color;
        }
        objectCollider = GetComponent<Collider2D>();
        objectRigidbody = GetComponent<Rigidbody2D>();
    }

    public virtual void MouseEnter()
    {
        if (!Activated) return;

        selectRenderer.DOColor(Color.yellow, 0.2f);
    }
    public virtual void MouseDown()
    {
        if (!Activated || Grabed) return;

        Interact();
    }
    public virtual void MouseExit()
    {
        if (!Activated || Grabed) return;

        selectRenderer.DOColor(ObjectColor, 0.2f);
    }

    public virtual void Active()
    {
        Activated = true;
        spriteRenderer.DOFade(1f, 0.5f);
        objectCollider.enabled = true;
    }

    public virtual void Deactive()
    {
        spriteRenderer.DOFade(0.5f, 0.5f);
        objectCollider.enabled = false;
    }
    public virtual void Interact() 
    {
        if (Activated) 
        { 
        } 
    }
    public virtual void Grab(Transform target)
    {
        if (!isGrabable || !Activated) return;

        Grabed = true;
        grabTarget = target;

        if (objectRigidbody != null)
        {
            objectRigidbody.linearVelocity = Vector2.zero;
            
        }
    }

    public virtual void Down(Transform target)
    {
        if (!isGrabable || !Activated) return;

        Grabed = false;

        Vector3 worldPos = target.position;

        float snapX = Mathf.Round(worldPos.x / gridSize) * gridSize;
        float snapY = Mathf.Round(worldPos.y / gridSize) * gridSize;

        transform.position = new Vector3(snapX, snapY, transform.position.z);

        if (objectRigidbody != null)
        {
            objectRigidbody.isKinematic = false;
        }
    }
}
