using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private Vector3 shootOffset;
    [SerializeField] private float shootCooldown = 0.2f;

    [Header("Ammo")]
    [SerializeField] private int maxChamberAmmo = 6;
    [SerializeField] private int maxBeltAmmo = 18;
    [SerializeField] private float reloadTimePerBullet = 0.5f;

    [Header("Debug UI")]
    [SerializeField] private TextMeshProUGUI bulletText;

    private bool canShoot = true;
    private bool isReloading = false;

    private int currentChamberAmmo;
    private int currentBeltAmmo;

    private Coroutine reloadRoutine;

    private void Start()
    {
        currentChamberAmmo = maxChamberAmmo;
        currentBeltAmmo = 3;

        UpdateBulletText();
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
        if (playerBullet == null)
        {
            Debug.LogError($"[{name}] PlayerWeapon: playerBullet no está asignado en el inspector.");
            return;
        }

        GameObject bulletObject = Instantiate(
            playerBullet,
            transform.position + shootOffset,
            Quaternion.identity
        );

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        if (bullet == null)
        {
            Debug.LogError($"[{name}] PlayerWeapon: el prefab de bala no tiene componente Bullet.");
            Destroy(bulletObject);
            return;
        }

        bullet.Init(dir, gameObject);

        currentChamberAmmo--;
        UpdateBulletText();

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
        UpdateBulletText();

        while (currentChamberAmmo < maxChamberAmmo && currentBeltAmmo > 0)
        {
            yield return new WaitForSeconds(reloadTimePerBullet);

            currentChamberAmmo++;
            currentBeltAmmo--;

            UpdateBulletText();
        }

        isReloading = false;
        reloadRoutine = null;

        UpdateBulletText();
    }

    private IEnumerator ShootCd()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    private void UpdateBulletText()
    {
        if (bulletText == null)
        {
            return;
        }

        string reloadText = isReloading ? " RELOADING" : "";
        bulletText.text = $"T:{currentChamberAmmo}/{maxChamberAmmo}  C:{currentBeltAmmo}/{maxBeltAmmo}{reloadText}";
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
        UpdateBulletText();

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
        UpdateBulletText();
    }
}
