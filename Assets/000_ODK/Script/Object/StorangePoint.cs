using DG.Tweening;
using UnityEngine;

public class StorangePoint : AbstractObjectScript
{
    public override void Interact()
    {
        if (DOTween.IsTweening(transform)) return;

        base.Interact();
        objectCollider.enabled = false; // 충돌 비활성화
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(player.transform.position, 0.1f).SetEase(ease));
        seq.Append(spriteRenderer.DOFade(0f, 0.1f).SetEase(ease));
        seq.OnComplete(() =>
        {
            if (player.TryGetComponent<GrabObjectContainer>(out var container))
            {
                container.PlusMaxCapacity();
            }

            // 오브젝트 파괴
            Destroy(gameObject);
        });
    }
}