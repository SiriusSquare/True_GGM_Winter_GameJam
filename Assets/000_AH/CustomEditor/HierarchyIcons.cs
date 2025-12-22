#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyIcons
{
    const int MAX_ICONS = 6;

    static HierarchyIcons()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        Component[] components = obj.GetComponents<Component>();
        if (components == null || components.Length == 0) return;

        float iconSize = 16f;
        float padding = 2f;

        Rect rect = new Rect(
            selectionRect.xMax - iconSize,
            selectionRect.y,
            iconSize,
            iconSize
        );

        HashSet<Texture> drawnIcons = new HashSet<Texture>();
        int iconCount = 0;
        bool hasScript = false;

        for (int i = components.Length - 1; i >= 0; i--)
        {
            if (iconCount >= MAX_ICONS) break;

            Component comp = components[i];
            if (comp == null) continue;

            if (comp is Transform) continue;

            if (comp is RectTransform) continue;

            if(comp is CanvasRenderer) continue;
            if (comp is MonoBehaviour)
            {
                if (hasScript) continue;
                hasScript = true;
            }

            Texture icon = EditorGUIUtility
                .ObjectContent(null, comp.GetType())
                .image;

            if (icon == null) continue;

            if (drawnIcons.Contains(icon)) continue;

            drawnIcons.Add(icon);

            GUI.DrawTexture(rect, icon);
            rect.x -= iconSize + padding;
            iconCount++;
        }
    }
}
#endif
