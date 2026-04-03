using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private Vector3 shootOffset;

    [SerializeField] private int initBulletAmount;

    private bool canShoot = true;
    private int bulletAmount;

    [SerializeField] private TextMeshProUGUI bulletText;

    private void Start()
    {
        bulletAmount = initBulletAmount;

        if (bulletText != null)
        {
            bulletText.text = bulletAmount.ToString();
        }
        else
        {
            Debug.LogWarning($"[{name}] PlayerWeapon: bulletText no está asignado en el inspector.");
        }
    }

    public void Shoot(Vector3 dir)
    {
        if (bulletAmount <= 0 || !canShoot)
        {
            return;
        }

        if (playerBullet == null)
        {
            Debug.LogError($"[{name}] PlayerWeapon: playerBullet no está asignado en el inspector.");
            return;
        }

        StartCoroutine(ShootCd());

        if (bulletText != null)
        {
            bulletText.text = bulletAmount.ToString();
        }

        GameObject bulletObject = Instantiate(
            playerBullet,
            transform.position + shootOffset,
            Quaternion.identity
        );

        Bullet b = bulletObject.GetComponent<Bullet>();

        if (b == null)
        {
            Debug.LogError($"[{name}] PlayerWeapon: el prefab de bala no tiene componente Bullet.");
            Destroy(bulletObject);
            return;
        }

        b.Init(dir, gameObject);

        bulletAmount--;
    }

    private IEnumerator ShootCd()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.2f);
        canShoot = true;
    }
}
