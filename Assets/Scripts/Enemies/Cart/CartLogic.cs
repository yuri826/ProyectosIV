using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class CartLogic : MonoBehaviour
{
    private CartState cartState = CartState.MoveToCart;
    private CartShootState shootState = CartShootState.LookLeftToSide;
    private Transform currentCartPoint;
    CartEnemyManager cartManager => CartEnemyManager.Instance;

    [Header("Movement")] 
    [SerializeField] private float appearSpeed;

    [Header("Shoot")] 
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shootWaitTime;
    [SerializeField] private float bulletTime;

    [Tooltip("Multiplo de 4 OBLIGATORIO")][SerializeField] private int bulletNumber = 32;
    private int currentBulletNumber;
    private int bulletNumberPerMovement => bulletNumber / 4;
    [SerializeField] private int maxSideAngle = 45;
    private float bulletAngleIncrement => maxSideAngle / bulletNumberPerMovement;
    [SerializeField] private Vector3 shootDir;
    [SerializeField] private Transform shootPoint;

    private Coroutine shootRoutine;
    
    private void Start()
    {
        GetRandomCart();
    }

    private void Update()
    {
        switch (cartState)
        {
            case CartState.MoveToCart:
                
                this.transform.position = Vector3.Lerp(this.transform.position, currentCartPoint.position, Time.deltaTime * appearSpeed);

                if (Vector3.Distance(this.transform.position, currentCartPoint.position) < 0.2f)
                {
                    shootRoutine = StartCoroutine(ShootRoutine());
                    cartState = CartState.InCart;
                }

                break;
            
            case CartState.InCart:

                break;
        }
    }

    private void GetRandomCart()
    {
        List<Transform> possibleLocations = new List<Transform>();

        for (int i = 0; i < cartManager.cartPoints.Length; i++)
        {
            if (!cartManager.cartPoints[i].IsTaken)
            {
                possibleLocations.Add(cartManager.cartPoints[i].TransformPoint);
            }
        }

        if (possibleLocations.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        
        int cartN = Random.Range(0, possibleLocations.Count);
        
        currentCartPoint = possibleLocations[cartN];
    }

    private IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(shootWaitTime);
        
        cartState = CartState.Shoot;
        currentBulletNumber = 0;

        while (cartState == CartState.Shoot)
        {
            Shoot();
            yield return new WaitForSeconds(bulletTime);
        }
        
        shootRoutine = StartCoroutine(ShootRoutine());
    }

    private void Shoot()
    {
        currentBulletNumber++;
        
        Bullet b = Instantiate(bulletPrefab, shootPoint).GetComponent<Bullet>();
        b.transform.parent = null;
        b.Init(shootDir, this.gameObject);
        
        int angleMult;
        
        if (shootState == CartShootState.LookLeftToSide || shootState == CartShootState.LookLeftToCenter)
            angleMult = -1;
        else 
            angleMult = 1;

        shootDir = Quaternion.AngleAxis(bulletAngleIncrement * angleMult, Vector3.up) * shootDir;

        if (currentBulletNumber >= bulletNumberPerMovement)
        {
            currentBulletNumber = 0;
            switch (shootState)
            {
                case CartShootState.LookLeftToSide: shootState = CartShootState.LookRightToCenter;
                    break;
                case CartShootState.LookRightToCenter: shootState = CartShootState.LookRightToSide;
                    break;
                case CartShootState.LookRightToSide: shootState = CartShootState.LookLeftToCenter;
                    break;
                case CartShootState.LookLeftToCenter: 
                    cartState = CartState.InCart;
                    shootState = CartShootState.LookLeftToSide;
                    break;
            }
        }
    }
}
