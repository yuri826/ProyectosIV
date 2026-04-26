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
    private CartPoint currentCartPointData;
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
        //Se adjudica un carro en el que ponerse
        GetRandomCart();
    }

    private void Update()
    {
        switch (cartState)
        {
            case CartState.MoveToCart:

                transform.position = Vector3.MoveTowards(
                    transform.position,
                    currentCartPoint.position,
                    appearSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, currentCartPoint.position) < 0.05f)
                {
                    transform.position = currentCartPoint.position;
                    cartState = CartState.InCart;
                    shootRoutine = StartCoroutine(ShootRoutine());
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
        //Busca un punto posible
        List<CartPoint> possibleLocations = new List<CartPoint>();
        
        foreach (var cart in cartManager.cartPoints)
        {
            if (!cart.IsTaken)
            {
                possibleLocations.Add(cart);
            }
        }
        
        //Si todos están cogidos se destruyen
        if (possibleLocations.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        
        //Se setean las variables tras coger un carro aleatorio entre los posibles
        
        int cartN = Random.Range(0, possibleLocations.Count);

        currentCartPointData = possibleLocations[cartN];
        currentCartPoint = currentCartPointData.TransformPoint;
        currentTrainCarZone = currentCartPointData.TrainCarZone;
        currentCartPointData.IsTaken = true;

        transform.position = currentCartPointData.AppearPoint.position;
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
        
        SabotagePoint sabotagePoint = currentTrainCarZone.GetRandomFreeSabotagePoint();

        if (sabotagePoint is not null)
        {
            sabotagePoint.BreakPoint();
            TrainGameMode.instance.TakeDamage(sabotagePoint.damageAmount);
        }

        shootRoutine = StartCoroutine(ShootRoutine());
    }

    //Dispara en abanico
    private void Shoot()
    {
        currentBulletNumber++;
        
        Bullet b = Instantiate(bulletPrefab, shootPoint).GetComponent<Bullet>();
        b.transform.parent = null;
        b.Init(shootDir, this.gameObject);
        
        int angleMult;
        
        if (shootState is CartShootState.LookLeftToSide or CartShootState.LookLeftToCenter)
            angleMult = -1;
        else 
            angleMult = 1;

        shootDir = Quaternion.AngleAxis(bulletAngleIncrement * angleMult, Vector3.up) * shootDir;

        /*Dispara en cuatro estados
            Barre izquierda
            Barre derecha
            Barre más a la derecha
            Barre a la izquierda hasta el centro*/
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
