using UnityEngine;
using UnityEngine.InputSystem;

public class SceneChangeTest : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            SceneChangeManager.Instance.ChangeScene(0);
        }
    }
}
