using UnityEngine;

public class AimController : MonoBehaviour
{
    public enum AimZone { Body, Head, Legs }
    public AimZone currentZone = AimZone.Body;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            currentZone = (AimZone)(((int)currentZone + 1) % 3);
            Debug.Log($"Targetting {currentZone} zone");
        }
    }

    public float GetVerticalOffset()
    {
        switch (currentZone)
        {
            case AimZone.Head: return 0.4f;
            case AimZone.Legs: return -0.6f;
            default: return 0f;
        }
    }
}