using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] private int audioIndex;

    public void OnEnable()
    {
        SoundManager.Instance.PlaySFX(audioIndex);
    }
}
