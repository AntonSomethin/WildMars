using UnityEngine;

public static class BallisticCalculator
{    public static float GetAngle(float distance, float speed, float gravity, float yOffset)
    {
        float v = speed;
        float g = gravity;
        float x = distance;
        float y = yOffset;

        float v2 = v * v;
        float root = v2 * v2 - g * (g * x * x + 2 * y * v2);

        if (root < 0) return 0;

        return Mathf.Atan((v2 - Mathf.Sqrt(root)) / (g * x)) * Mathf.Rad2Deg;
    }

    public static Vector3 ApplySpread(Vector3 direction, float spreadFactor)
    {
        if (spreadFactor <= 0.01f) return direction;
        return Vector3.Slerp(direction, Random.insideUnitSphere, spreadFactor * 0.1f).normalized;
    }
}