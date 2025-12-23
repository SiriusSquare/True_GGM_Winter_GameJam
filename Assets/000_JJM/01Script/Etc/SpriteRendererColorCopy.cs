using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpriteRendererColorCopy : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private SpriteRenderer _sprtieRenderer;

    void Update()
    {
        Color a = _sprtieRenderer.color;
        Color la = _light.color;

        _light.color = new Color(la.r, la.r, la.b, a.a);
    }
}
