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
    private TrainCarZone currentTrainCarZone;
    private CartEnemyManager cartManager => TrainGameMode.instance.GetCartManager();

    [Header("Movement")] 
    [SerializeField] private float appearSpeed;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveAmplitude;

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
                
                this.transform.position = Vector3.Lerp(this.transform.position,
                    new Vector3(Mathf.Sin(Time.time * moveSpeed) * moveAmplitude 
                                + currentCartPoint.position.x,this.transform.position.y, this.transform.position.z),
                    Time.deltaTime * moveSpeed);

                break;
            
            case CartState.Return:
                
                this.transform.position = Vector3.Lerp(this.transform.position, currentCartPoint.position, Time.deltaTime * moveSpeed);

                if (Vector3.Distance(this.transform.position, currentCartPoint.position) < 0.1f)
                {
                    cartState = CartState.InCart;
                }

                break;
        }
    }

    private void GetRandomCart()
    {
        List<CartPoint> possibleLocations = new List<CartPoint>();

        for (int i = 0; i < cartManager.cartPoints.Length; i++)
        {
            if (!cartManager.cartPoints[i].IsTaken)
            {
                possibleLocations.Add(cartManager.cartPoints[i]);
            }
        }

        if (possibleLocations.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        
        int cartN = Random.Range(0, possibleLocations.Count);
        
        currentCartPoint = possibleLocations[cartN].TransformPoint;
        currentTrainCarZone = possibleLocations[cartN].TrainCarZone;
        possibleLocations[cartN].IsTaken = true;
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
        
        currentTrainCarZone.GetRandomFreeSabotagePoint().BreakPoint();
        
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
                    cartState = CartState.Return;
                    shootState = CartShootState.LookLeftToSide;
                    break;
            }
        }
    }
}
