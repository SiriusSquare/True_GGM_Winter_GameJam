using UnityEngine;

public class PlaySoundMethod : MonoBehaviour
{
    public void PlaySound(int num)
    {
        SoundManager.Instance.PlaySFX(num);
    }
}
