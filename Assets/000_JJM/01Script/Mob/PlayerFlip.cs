using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlip : MonoBehaviour
{
    private Vector2 _moveDir;

    private void Update()
    {
        if (_moveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (_moveDir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        _moveDir = value.Get<Vector2>();
    }
}
