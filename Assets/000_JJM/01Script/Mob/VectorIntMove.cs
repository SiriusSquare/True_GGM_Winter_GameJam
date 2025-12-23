using UnityEngine;

public class VectorIntMove : MonoBehaviour
{
    [SerializeField] private GameObject _object;

    private void Awake()
    {
        _object = transform.parent.gameObject;
    }

    void LateUpdate()
    {
        Vector3 pos = _object.transform.position;

        transform.position = new Vector3(
            Mathf.Round(pos.x),
            Mathf.Round(pos.y),
            pos.z
        );
    }
}
