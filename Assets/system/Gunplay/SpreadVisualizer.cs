using UnityEngine;

public class SpreadVisualizer : MonoBehaviour
{
    public Shoot shootScript;
    public LaserPointer laser;

    [Header("Lines")]
    public LineRenderer targetCircle;
    public LineRenderer startCircle;
    public LineRenderer[] sideLines;

    [Header("Settings")]
    public float focusDistance = 1.0f;
    public int segments = 32;

    void Update()
    {
        if (laser != null && laser.isTargetingEnemy && shootScript != null && shootScript.currentWeapon != null)
        {
            ToggleLines(true);
            DrawTruncatedCone();
        }
        else
        {
            ToggleLines(false);
        }
    }

    void ToggleLines(bool state)
    {
        if (targetCircle != null) targetCircle.enabled = state;
        if (startCircle != null) startCircle.enabled = state;

        if (sideLines != null)
        {
            foreach (var l in sideLines)
            {
                if (l != null) l.enabled = state;
            }
        }
    }

    void DrawTruncatedCone()
    {
        if (shootScript == null || shootScript.currentWeapon == null || shootScript.aim == null) return;

        float yOffset = shootScript.aim.GetVerticalOffset();
        Vector3 hitPoint = laser.lastHitPoint + Vector3.up * yOffset;
        Vector3 firePoint = shootScript.currentWeapon.firePoint.position;

        Vector3 shootDir = (hitPoint - firePoint).normalized;

        Vector3 startCenter = hitPoint - shootDir * focusDistance;
        Vector3 endCenter = hitPoint + shootDir * focusDistance;

        float spreadAngle = shootScript.currentUserSpread;

        float distToStart = Vector3.Distance(firePoint, startCenter);
        float distToEnd = Vector3.Distance(firePoint, endCenter);

        float radiusStart = distToStart * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        float radiusEnd = distToEnd * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);

        DrawCircle(startCircle, startCenter, radiusStart);
        DrawCircle(targetCircle, endCenter, radiusEnd);

        if (sideLines != null)
        {
            for (int i = 0; i < sideLines.Length; i++)
            {
                if (sideLines[i] == null) continue;

                float angle = i * 2 * Mathf.PI / sideLines.Length;
                Vector3 pStart = GetPointOnCircle(startCenter, radiusStart, angle);
                Vector3 pEnd = GetPointOnCircle(endCenter, radiusEnd, angle);

                sideLines[i].SetPosition(0, pStart);
                sideLines[i].SetPosition(1, pEnd);
            }
        }
    }

    void DrawCircle(LineRenderer line, Vector3 center, float radius)
    {
        if (line == null) return;

        line.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * 2 * Mathf.PI / (segments - 1);
            line.SetPosition(i, GetPointOnCircle(center, radius, angle));
        }
    }

    Vector3 GetPointOnCircle(Vector3 center, float radius, float angle)
    {
        if (shootScript == null || shootScript.currentWeapon == null) return center;

        Vector3 direction = (center - shootScript.currentWeapon.firePoint.position).normalized;
        if (direction == Vector3.zero) direction = Vector3.forward;

        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 localPoint = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

        return center + (rotation * localPoint);
    }
}