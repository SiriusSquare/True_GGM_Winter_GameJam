using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabObjectContainer : MonoBehaviour
{
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask noDropLayer;
    [SerializeField] private Vector2 gridSize = Vector2.one;
    [SerializeField] private float grabRange = 3f;
    [SerializeField] private int maxCapacity = 1;

    private Vector2Int mouseVector;
    [SerializeField] private Transform mouseEffect;
    [SerializeField] private Transform ditectionEffect;
    private AbstractObjectScript currentHover;
    [field: SerializeField] public List<AbstractObjectScript> GrabArray { get; private set; }

    public float fadeDistance = 3f;
    public float fadeDuration = 0.2f;

    void Update()
    {
        UpdateHover();
        HandleClick();
    }

    public void PlusMaxCapacity()
    {
        maxCapacity++;
    }

    public void PlusGrabRange()
    {
        grabRange += 2;
    }

    public void FixedUpdate()
    {
        if (Camera.main == null) return;
        if (ditectionEffect != null)
            ditectionEffect.localScale = new Vector3(grabRange * gridSize.x, grabRange * gridSize.y, 1f) * 1.32f;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector2Int snappedGrid = new Vector2Int(
            Mathf.RoundToInt(mouseWorldPos.x / gridSize.x),
            Mathf.RoundToInt(mouseWorldPos.y / gridSize.y)
        );

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
            float targetAlpha = !IsInside3x3(snappedGrid) ? 0f : 1f;
            SpriteRenderer sr = mouseEffect.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (!Mathf.Approximately(sr.color.a, targetAlpha))
                {
                    sr.DOKill();
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

        for (int i = 0; i < GrabArray.Count; i++)
        {
            if (i == 0)
                GrabArray[i].GrabTarget = this.transform;
            else
                GrabArray[i].GrabTarget = GrabArray[i - 1].transform;
        }
    }

    public AbstractObjectScript GetObjectByType(string type)
    {
        return GrabArray.FirstOrDefault(obj => obj.ObjectType.Contains(type));
    }

    public AbstractObjectScript GetLastObject()
    {
        if (GrabArray.Count == 0) return null;
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

        if (currentHover != null) currentHover.MouseExit();
        currentHover = nextHover;
        if (currentHover != null) currentHover.MouseEnter();
    }

    bool IsInside3x3(Vector2 worldPos)
    {
        Vector2 center = transform.position;
        Vector2 halfSize = gridSize * (grabRange / 2f);

        return Mathf.Abs(worldPos.x - center.x) <= halfSize.x &&
               Mathf.Abs(worldPos.y - center.y) <= halfSize.y;
    }

    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentHover != null)
            {
                if (currentHover.isGrabable)
                {
                    if (GrabArray.Count >= maxCapacity)
                    {
                        var first = GrabArray[0];
                        if (first != null)
                        {
                            first.Down(currentHover.transform.position);
                            SoundManager.Instance.PlaySFX(first.soundIndex);
                        }
                    }
                    currentHover.Grab();
                    SoundManager.Instance.PlaySFX(currentHover.soundIndex);
                }
                else
                {
                    HandleRightClick();
                }
            }
            else
            {
                HandleRightClick();
            }
        }
    }

    void HandleRightClick()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        if (!IsInside3x3(mouseWorldPos)) return;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, noDropLayer | objectLayer);

        if (hit != null)
        {
            
            if (hit.TryGetComponent<AbstractObjectScript>(out AbstractObjectScript interactiveObj))
            {

                if ( interactiveObj.isInteractable)
                {
                    interactiveObj.Interact();
                }
                if (interactiveObj.UseableObjectType.Length > 0)
                {
                    foreach (var useType in interactiveObj.UseableObjectType)
                    {
                        foreach (var grabObj in GrabArray)
                        {
                            if (grabObj.ObjectType.Contains(useType))
                            {
                                interactiveObj.Use(useType);
                                SoundManager.Instance.PlaySFX(grabObj.soundIndex);

                                if (grabObj.IsConsumerble)
                                    grabObj.Consum(mouseWorldPos);
                                else
                                    grabObj.UseNoConsume(mouseWorldPos);

                                return;
                            }
                        }
                    }
                }

            }

            if (((1 << hit.gameObject.layer) & noDropLayer) != 0)
            {
                return;
            }
        }

        AbstractObjectScript target = GetLastObject();
        if (target != null)
        {
            target.Down(mouseWorldPos);
            SoundManager.Instance.PlaySFX(target.soundIndex);
        }
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / gridSize.x),
            Mathf.RoundToInt(mouseVector.y / gridSize.y)
        );
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, gridSize * grabRange);
    }
#endif
}