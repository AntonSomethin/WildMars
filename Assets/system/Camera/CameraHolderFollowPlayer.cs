using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    private Vector3 offset;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            rb.MovePosition(targetPosition);
        }
    }
}