using UnityEngine;
using System.Linq;

public class DropTriggerItem : AbstractObjectScript
{
    [Header("Sensor Settings")]
    [SerializeField] private bool triggerOnce = false;

    [Header("Stage Settings")]
    [SerializeField] private int targetStageIndex = 0;

    private bool hasTriggered = false;
    private AbstractObjectScript currentDetectedItem = null;

    protected override void Awake()
    {
        base.Awake();
        isGrabable = false;
    }

    protected override void FixedUpdate()
    {
        if (triggerOnce && hasTriggered) return;
        DetectItemOnMe();
    }

    private void DetectItemOnMe()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, objectCollider.bounds.size * 0.8f, 0f);
        AbstractObjectScript foundItem = null;

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;
            AbstractObjectScript item = hit.GetComponent<AbstractObjectScript>();
            if (item != null && !item.Grabed)
            {
                foundItem = item;
                break;
            }
        }

        if (foundItem == null)
        {
            currentDetectedItem = null;
            return;
        }

        if (foundItem == currentDetectedItem) return;

        currentDetectedItem = foundItem;

        if (AH_StageManager.Instance != null)
        {
            if (AH_StageManager.Instance.IsStageLocked(targetStageIndex))
            {
                Debug.Log($"이 놈 {targetStageIndex} 안열리죠 ㅋ{AH_StageManager.Instance.currentStageIndex})");
                return;
            }
        }

        bool anyActionExecuted = false;

        foreach (string itemType in foundItem.ObjectType)
        {
            if (!string.IsNullOrEmpty(itemType))
            {
                Use(itemType);
                anyActionExecuted = true;
            }
        }

        if (anyActionExecuted && triggerOnce)
        {
            hasTriggered = true;
        }
    }

    public override void Use(string type)
    {
        base.Use(type);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (GetComponent<BoxCollider2D>() != null)
            Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().bounds.size * 0.8f);
    }
}