using UnityEngine;

public class LaserPointer : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public LineRenderer lineRenderer;

    [Header("Settings")]
    public float maxDistance = 40f;
    public LayerMask enemyLayers;

    [Header("Data Output")]
    public bool isTargetFound;
    public float hitDistance;
    public int hitLayer;

    public bool isTargetingEnemy;
    public Transform targetEnemy;
    public Vector3 lastHitPoint;

    public Collider lastHitCollider;

    [HideInInspector] public Vector3 currentDirection;

    void Update()
    {
        if (firePoint == null || lineRenderer == null) return;

        UpdateLaser();
    }

    void UpdateLaser()
    {
        Vector3 startPoint = firePoint.position;

        Plane laserPlane = new Plane(Vector3.up, startPoint);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 targetWorldPos = startPoint + firePoint.forward * maxDistance;

        if (laserPlane.Raycast(mouseRay, out float enter))
        {
            targetWorldPos = mouseRay.GetPoint(enter);
        }

        Vector3 direction = (targetWorldPos - startPoint).normalized;
        currentDirection = direction;
        Vector3 endPoint = startPoint + direction * maxDistance;

        RaycastHit hit;
        if (Physics.Raycast(startPoint, direction, out hit, maxDistance, enemyLayers))
        {
            endPoint = hit.point;
            isTargetFound = true;
            hitDistance = hit.distance;
            hitLayer = hit.collider.gameObject.layer;

            lastHitPoint = hit.point;
            targetEnemy = hit.transform;

            lastHitCollider = hit.collider;

            isTargetingEnemy = true;
        }
        else
        {
            isTargetFound = false;
            hitDistance = maxDistance;
            hitLayer = -1;

            lastHitPoint = endPoint;
            targetEnemy = null;

            lastHitCollider = null;

            isTargetingEnemy = false;
        }

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        lineRenderer.startColor = isTargetFound ? Color.green : Color.red;
        lineRenderer.endColor = lineRenderer.startColor;
    }
}