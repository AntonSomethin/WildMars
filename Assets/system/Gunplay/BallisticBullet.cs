using UnityEngine;

public class BallisticBullet : MonoBehaviour
{
    private Vector3 _velocity;
    public float gravity = 3.71f;
    private float _damage;
    private bool _isInitialized = false;

    public void Initialize(Vector3 startPos, Vector3 direction, float speed, float damage)
    {
        transform.position = startPos;
        _velocity = direction * speed;
        _damage = damage;
        _isInitialized = true;

        Destroy(gameObject, 10f);
    }

    void Update()
    {
        if (!_isInitialized) return;

        Vector3 previousPosition = transform.position;
        _velocity.y -= gravity * Time.deltaTime;
        Vector3 step = _velocity * Time.deltaTime;
        transform.position += step;

        Vector3 movementThisFrame = transform.position - previousPosition;
        if (Physics.Raycast(previousPosition, movementThisFrame.normalized, out RaycastHit hit, movementThisFrame.magnitude))
        {
            HitTarget(hit);
        }

        transform.forward = _velocity.normalized;
    }

    void HitTarget(RaycastHit hit)
    {
        BodyPart bodyPart = hit.collider.GetComponent<BodyPart>();

        if (bodyPart != null)
        {
            bodyPart.TakePartDamage(_damage);
        }
        else
        {
            Health globalHealth = hit.collider.GetComponentInParent<Health>();
            if (globalHealth != null)
            {
                globalHealth.TakeDamage(_damage);
            }
        }

        Debug.Log($"Bullet hit target collider: {hit.collider.name}");

        Destroy(gameObject);
    }
}