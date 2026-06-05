using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public Shoot shootScript;

    [Header("3D Models in Hand")]
    public GameObject[] weaponModels;

    [Header("Fire Points with Weapon Script")]
    public Weapon[] weaponScripts;

    private int _currentWeaponIndex = 0;

    void Start()
    {
        SelectWeapon(_currentWeaponIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
    }

    void SelectWeapon(int index)
    {
        if (index < 0 || index >= 2) 
        {
            Debug.Log("Wrong Index");
            return;
        }        

        _currentWeaponIndex = index;

        bool isRevolverActive = (index == 0);

        if (weaponModels[0] != null) weaponModels[0].SetActive(isRevolverActive);
        if (weaponModels[1] != null) weaponModels[1].SetActive(!isRevolverActive);

        if (weaponScripts[0] != null) weaponScripts[0].gameObject.SetActive(isRevolverActive);
        if (weaponScripts[1] != null) weaponScripts[1].gameObject.SetActive(!isRevolverActive);

        if (shootScript != null && weaponScripts[index] != null)
        {
            shootScript.currentWeapon = weaponScripts[index];
            shootScript.StopAllCoroutines();
            shootScript.UpdateAmmoDisplay();
        }
    }
}