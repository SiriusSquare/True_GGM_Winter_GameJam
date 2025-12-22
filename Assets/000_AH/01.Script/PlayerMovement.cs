using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 _moveDir;
    private Rigidbody2D _rigid;
    [SerializeField] private float _speed = 5f;
    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    public void SetMove(Vector2 dir)
    {
        _moveDir = dir;
    }
    public void Move()
    {
        _rigid.linearVelocity = _moveDir * _speed;
    }
    public void OnMove(InputValue value)
    {
        _moveDir = value.Get<Vector2>();
    }
    private void FixedUpdate()
    {
        Move();
    }
}
