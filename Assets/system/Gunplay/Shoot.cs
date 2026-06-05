using UnityEngine;
using System.Collections;
using TMPro;

public class Shoot : MonoBehaviour
{
    public LaserPointer laser;
    public AimController aim;
    public Weapon currentWeapon;

    [Header("UI Settings")]
    public TextMeshProUGUI ammoTextUI;

    [Header("Reload Settings")]
    public float timePerBullet = 0.8f;
    private Coroutine _reloadCoroutine;
    private bool _isReloading = false;

    [Header("Focus Triangle Settings")]
    public float focusZoneWidth = 1.0f;
    public float penaltyMultiplier = 5.0f;
    public float currentUserSpread;

    private float _bulletGravity;
    private float _nextFireTime;

    void Start()
    {
        UpdateGravityFromWeapon();
        UpdateAmmoDisplay();
    }

    void Update()
    {
        if (currentWeapon != null)
        {
            currentUserSpread = currentWeapon.baseSpreadAngle;
        }

        if (Input.GetMouseButton(0) && Time.time >= _nextFireTime)
        {
            if (currentWeapon != null && laser != null && aim != null)
            {
                if (_isReloading)
                {
                    StopReloading();
                    _nextFireTime = Time.time + currentWeapon.fireRate;
                    return;
                }

                if (currentWeapon.currentAmmo > 0)
                {
                    ExecuteShoot();
                    _nextFireTime = Time.time + currentWeapon.fireRate;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !_isReloading && currentWeapon != null)
        {
            TryReload();
        }
    }

    void TryReload()
    {
        if (currentWeapon.currentAmmo >= currentWeapon.magazineSize || currentWeapon.currentReserveAmmo <= 0)
            return;

        if (currentWeapon.reloadType == WeaponReloadType.OneBullet)
        {
            _reloadCoroutine = StartCoroutine(OneBulletReloadRoutine());
        }
        else
        {
            if (currentWeapon.reloadSound != null)
            {
                AudioSource.PlayClipAtPoint(currentWeapon.reloadSound, currentWeapon.firePoint.position);
            }

            currentWeapon.ReloadAll();
            UpdateAmmoDisplay();
        }
    }

    IEnumerator OneBulletReloadRoutine()
    {
        _isReloading = true;

        while (currentWeapon.ReloadOneBullet())
        {
            UpdateAmmoDisplay();

            if (currentWeapon.reloadSound != null)
            {
                AudioSource.PlayClipAtPoint(currentWeapon.reloadSound, currentWeapon.firePoint.position);
            }

            yield return new WaitForSeconds(timePerBullet);
        }

        _isReloading = false;
    }

    void StopReloading()
    {
        if (_reloadCoroutine != null)
        {
            StopCoroutine(_reloadCoroutine);
        }
        _isReloading = false;
    }

    void ExecuteShoot()
    {
        currentWeapon.currentAmmo--;
        UpdateAmmoDisplay();

        if (currentWeapon.muzzleFlash != null)
            currentWeapon.muzzleFlash.Play();

        if (currentWeapon.shootSound != null)
        {
            AudioSource.PlayClipAtPoint(currentWeapon.shootSound, currentWeapon.firePoint.position);
        }

        if (_bulletGravity == 0 && currentWeapon.bulletPrefab != null)
        {
            _bulletGravity = currentWeapon.bulletPrefab.GetComponent<BallisticBullet>().gravity;
        }

        float distance = laser.hitDistance;
        float speed = currentWeapon.bulletSpeed;
        float yOffset = aim.GetVerticalOffset();

        float angle = BallisticCalculator.GetAngle(distance, speed, _bulletGravity, yOffset);
        Vector3 baseDir = Quaternion.AngleAxis(-angle, transform.right) * laser.currentDirection;

        float totalSpread = currentWeapon.baseSpreadAngle;

        Vector2 randomPoint = Random.insideUnitCircle * totalSpread;
        Quaternion spreadRot = Quaternion.Euler(randomPoint.x, randomPoint.y, 0);
        Vector3 finalDir = spreadRot * baseDir;

        GameObject bulletObj = Instantiate(currentWeapon.bulletPrefab, currentWeapon.firePoint.position, Quaternion.LookRotation(finalDir));
        bulletObj.GetComponent<BallisticBullet>().Initialize(currentWeapon.firePoint.position, finalDir, speed, currentWeapon.damage);
    }

    public void UpdateAmmoDisplay()
    {
        if (ammoTextUI != null && currentWeapon != null)
        {
            ammoTextUI.text = $"{currentWeapon.currentAmmo} / {currentWeapon.currentReserveAmmo}";
        }
    }

    void UpdateGravityFromWeapon()
    {
        if (currentWeapon != null && currentWeapon.bulletPrefab != null)
        {
            var bulletScript = currentWeapon.bulletPrefab.GetComponent<BallisticBullet>();
            if (bulletScript != null)
            {
                _bulletGravity = bulletScript.gravity;
            }
        }
    }
}