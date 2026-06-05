using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30;
    public string weaponTargetNameKey = "revolver";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Shoot playerShoot = other.GetComponent<Shoot>();
            if (playerShoot == null)
            {
                playerShoot = other.GetComponentInChildren<Shoot>();
            }

            WeaponSwitcher switcher = other.GetComponent<WeaponSwitcher>();
            if (switcher == null)
            {
                switcher = other.GetComponentInChildren<WeaponSwitcher>();
            }

            if (switcher != null && playerShoot != null)
            {
                Weapon matchedWeapon = null;

                foreach (Weapon w in switcher.weaponScripts)
                {
                    if (w != null && w.weaponName.ToLower().Contains(weaponTargetNameKey.ToLower()))
                    {
                        matchedWeapon = w;
                        break;
                    }
                }

                if (matchedWeapon != null)
                {
                    matchedWeapon.currentReserveAmmo = Mathf.Min(
                        matchedWeapon.currentReserveAmmo + ammoAmount,
                        matchedWeapon.maxReserveAmmo
                    );

                    if (playerShoot.currentWeapon == matchedWeapon)
                    {
                        playerShoot.UpdateAmmoDisplay();
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}