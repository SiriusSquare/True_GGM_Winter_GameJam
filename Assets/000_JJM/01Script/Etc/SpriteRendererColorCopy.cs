using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpriteRendererColorCopy : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private SpriteRenderer _sprtieRenderer;
    [SerializeField] private SpriteRenderer _sprtieRenderer2;

    void Update()
    {
        Color a = _sprtieRenderer.color;
        if (_light != null)
        {
            Color la = _light.color;
            _light.color = new Color(la.r, la.r, la.b, a.a);
        }

        if (_sprtieRenderer2 != null)
        {
            Color la = _sprtieRenderer2.color;
            _sprtieRenderer2.color = new Color(la.r, la.r, la.b, a.a);
        }
    }
}
