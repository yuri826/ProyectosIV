using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootCooldown = 0.2f;

    [Header("Ammo")]
    [SerializeField] private int maxChamberAmmo = 6;
    [SerializeField] private int maxBeltAmmo = 18;
    [SerializeField] private float reloadTimePerBullet = 0.5f;

    [Header("Debug UI")]
    [SerializeField] private TextMeshProUGUI beltAmmoText;
    [SerializeField] private RevolverAmmoHUD revolverAmmoHUD;

    private bool canShoot = true;
    private bool isReloading = false;

    private int currentChamberAmmo;
    private int currentBeltAmmo;

    private Coroutine reloadRoutine;

    private void Start()
    {
        currentChamberAmmo = maxChamberAmmo;
        currentBeltAmmo = 12;

        UpdateBeltAmmoText();
        revolverAmmoHUD.Initialize(currentChamberAmmo);
    }

    public void Shoot(Vector3 dir)
    {
        if (isReloading)
        {
            return;
        }

        if (!canShoot)
        {
            return;
        }

        if (currentChamberAmmo > 0)
        {
            FireBullet(dir);
            return;
        }

        if (currentBeltAmmo > 0)
        {
            StartReload();
        }
    }

    private void FireBullet(Vector3 dir)
    {
        GameObject bulletObject = Instantiate(
            playerBullet,
            shootPoint.position,
            Quaternion.identity
        );

        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.Init(dir, gameObject);

        currentChamberAmmo--;
        UpdateBeltAmmoText();
        revolverAmmoHUD.OnShot(currentChamberAmmo, shootCooldown);

        StartCoroutine(ShootCd());
    }

    private void StartReload()
    {
        if (isReloading)
        {
            return;
        }

        if (currentBeltAmmo <= 0)
        {
            return;
        }

        if (currentChamberAmmo >= maxChamberAmmo)
        {
            return;
        }

        reloadRoutine = StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        UpdateBeltAmmoText();

        while (currentChamberAmmo < maxChamberAmmo && currentBeltAmmo > 0)
        {
            yield return new WaitForSeconds(reloadTimePerBullet);

            currentChamberAmmo++;
            currentBeltAmmo--;

            UpdateBeltAmmoText();
            revolverAmmoHUD.OnReloadBulletInserted(currentChamberAmmo);
        }

        isReloading = false;
        reloadRoutine = null;

        UpdateBeltAmmoText();
        if (currentChamberAmmo == maxChamberAmmo)
        {
            revolverAmmoHUD.OnReloadComplete();
        }
    }

    private IEnumerator ShootCd()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private void UpdateBeltAmmoText()
    {
        beltAmmoText.text = $"{currentBeltAmmo}/{maxBeltAmmo}";
    }

    public int AddBeltAmmo(int amount)
    {
        if (amount <= 0)
        {
            return 0;
        }

        int freeSpace = maxBeltAmmo - currentBeltAmmo;
        int addedAmount = Mathf.Clamp(amount, 0, freeSpace);

        currentBeltAmmo += addedAmount;
        UpdateBeltAmmoText();

        return addedAmount;
    }

    public bool IsReloading()
    {
        return isReloading;
    }

    public int GetCurrentChamberAmmo()
    {
        return currentChamberAmmo;
    }

    public int GetCurrentBeltAmmo()
    {
        return currentBeltAmmo;
    }

    public int GetMaxChamberAmmo()
    {
        return maxChamberAmmo;
    }

    public int GetMaxBeltAmmo()
    {
        return maxBeltAmmo;
    }

    public void CancelReload()
    {
        if (!isReloading)
        {
            return;
        }

        if (reloadRoutine != null)
        {
            StopCoroutine(reloadRoutine);
            reloadRoutine = null;
        }

        isReloading = false;
        UpdateBeltAmmoText();
    }
}
