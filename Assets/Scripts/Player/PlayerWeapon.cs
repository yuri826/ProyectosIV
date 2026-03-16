using System;
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
    }

    public void Shoot(Vector3 dir)
    {
        if (bulletAmount <= 0 || !canShoot) return;

        StartCoroutine(ShootCd());

        bulletText.text = bulletAmount.ToString();
        
        PlayerBullet b = Instantiate(playerBullet, this.transform.position + shootOffset, Quaternion.identity).GetComponent<PlayerBullet>();
        b.shootDir = dir;
        
        bulletAmount--;
    }

    private IEnumerator ShootCd()
    {
        canShoot = false;
        yield return new WaitForSeconds(0.2f);
        canShoot = true;
    }
}
