using UnityEngine;

public enum WeaponReloadType
{
    Default,
    OneBullet
}

public class Weapon : MonoBehaviour
{
    [Header("Weapon Type")]
    public WeaponReloadType reloadType;

    [Header("Characteristics")]
    public string weaponName = "";
    public float bulletSpeed = 0f;
    public float damage = 20f;
    public float fireRate = 0f;
    public float baseSpreadAngle = 0.5f; // Degree

    [Header("Visual")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public ParticleSystem muzzleFlash;

    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("Ammo Settings")]
    public int magazineSize = 30;
    public int currentAmmo = 30;
    public int maxReserveAmmo = 120;
    public int currentReserveAmmo = 90;

    public void ReloadAll()
    {
        int ammoNeeded = magazineSize - currentAmmo;
        if (ammoNeeded > 0 && currentReserveAmmo > 0)
        {
            int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);
            currentAmmo += ammoToReload;
            currentReserveAmmo -= ammoToReload;
        }
    }

    public bool ReloadOneBullet()
    {
        if (currentAmmo < magazineSize && currentReserveAmmo > 0)
        {
            currentAmmo++;
            currentReserveAmmo--;
            return true;
        }
        return false;
    }
}