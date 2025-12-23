using DG.Tweening;
using UnityEngine;

public class MagneticPoint : AbstractObjectScript
{
    private void Update()
    {
        if (Grabed)
        {
            if (player.TryGetComponent<GrabObjectContainer>(out var container))
            {
                container.PlusGrabRange();
            }

            Destroy(gameObject);
        }
    }

    public override void Interact()
    {
        if (DOTween.IsTweening(transform)) return;

        if (Grabed)
        {
            Consum(player.transform.position);
            return;
        }

        base.Interact();

        objectCollider.enabled = false;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(player.transform.position, 0.1f).SetEase(ease));
        seq.Append(spriteRenderer.DOFade(0f, 0.1f).SetEase(ease));
        seq.OnComplete(() =>
        {
            if (player.TryGetComponent<GrabObjectContainer>(out var container))
            {
                container.PlusGrabRange();
            }

            Destroy(gameObject);
        });
    }
}
