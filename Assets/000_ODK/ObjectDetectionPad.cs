using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectDetectionPad : MonoBehaviour
{
    [Header("Condition")]
#if UNITY_EDITOR
    public MonoScript targetScript;
#endif
    public bool needTypeCheck = false;
    public string needType;

    public bool needColorCheck = false;
    public Color needColor; // 인스펙터에서 찍는 색상

    [field: SerializeField] public bool isClear { get; private set; }

    private void OnTriggerStay2D(Collider2D other)
    {
        var obj = other.GetComponent<AbstractObjectScript>();
        if (obj == null) return;

        // 기본 무시 조건
        if (!obj.Activated) { isClear = false; return; }
        if (obj.Grabed) { isClear = false; return; }

        // 조건 검사
        bool match = IsTypeMatch(obj) && IsColorMatch(obj) && IsSameConcreteType(obj);

        isClear = match;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var obj = other.GetComponent<AbstractObjectScript>();
        if (obj == null) return;

        // 나가면 무조건 클리어 해제 (혹은 타입 체크 후 해제)
        isClear = false;
    }

    /* =========================
       Condition Checks
       ========================= */

    bool IsTypeMatch(AbstractObjectScript obj)
    {
        if (!needTypeCheck || string.IsNullOrEmpty(needType)) return true;
        return obj.ObjectType != null && obj.ObjectType.Contains(needType);
    }

    bool IsColorMatch(AbstractObjectScript obj)
    {
        if (!needColorCheck) return true;

        // 🔹 색상 코드로 비교 (RGB만 추출하여 비교)
        string targetHex = ColorUtility.ToHtmlStringRGB(needColor);
        string objHex = ColorUtility.ToHtmlStringRGB(obj.ObjectColor);

        return targetHex == objHex;
    }

    bool IsSameConcreteType(AbstractObjectScript obj)
    {
#if UNITY_EDITOR
        if (targetScript == null) return true;
        return obj.GetType() == targetScript.GetClass();
#else
        return true;
#endif
    }
}