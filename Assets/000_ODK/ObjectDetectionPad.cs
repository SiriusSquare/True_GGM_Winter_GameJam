using UnityEngine;
using System.ComponentModel;

#if UNITY_EDITOR
using UnityEditor; // MonoScript 사용
#endif
using System.Linq;

public class ObjectDetectionPad : MonoBehaviour
{
    [Header("Condition")]
#if UNITY_EDITOR
    public MonoScript targetScript; // 에셋에서 직접 스크립트 지정 가능
#endif
    public bool needTypeCheck = false;  // 타입 검사 여부
    public string needType;

    public bool needColorCheck = false; // 색상 검사 여부
    public Color needColor;

    [field:SerializeField, _JJM.Script.CustomEditor.ReadOnly] public bool isClear { get; private set; }

    private void OnTriggerStay2D(Collider2D other)
    {
        var obj = other.GetComponent<AbstractObjectScript>();
        if (obj == null) return;

        // 🔽 기본 무시 조건
        if (!obj.Activated) return;
        if (obj.Grabed) return;

        // 🔽 조건 검사
        if (!IsTypeMatch(obj)) return;
        if (!IsColorMatch(obj)) return;
        if (!IsSameConcreteType(obj)) return;

        isClear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var obj = other.GetComponent<AbstractObjectScript>();
        if (obj == null) return;

        if (IsSameConcreteType(obj))
            isClear = false;
    }

    /* =========================
       Condition Checks
       ========================= */

    bool IsTypeMatch(AbstractObjectScript obj)
    {
        if (!needTypeCheck || string.IsNullOrEmpty(needType)) return true;

        return obj.ObjectType != null &&
               obj.ObjectType.Contains(needType);
    }

    bool IsColorMatch(AbstractObjectScript obj)
    {
        if (!needColorCheck) return true;

        if (obj.TryGetComponent<SpriteRenderer>(out var sr))
        {
            return sr.color == needColor;
        }
        return false;
    }

    bool IsSameConcreteType(AbstractObjectScript obj)
    {
#if UNITY_EDITOR
        if (targetScript == null) return true;

        // 🔥 핵심: MonoScript로 타입 비교
        return obj.GetType() == targetScript.GetClass();
#else
        return true;
#endif
    }
}
