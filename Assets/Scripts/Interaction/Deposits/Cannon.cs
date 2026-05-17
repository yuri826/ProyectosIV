using FMODUnity;
using UnityEngine;

public class Cannon : DepositObj
{
    [SerializeField] private GameObject cannonBullet;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private ParticleSystem particles;

    [SerializeField] private StudioEventEmitter sfxShoot;
    
    protected override void Completed()
    {
        repairBar.SetActive(false);
        repairBarImage.fillAmount = 0f;
        
        particles.Play();
        sfxShoot.Play();

        Bullet b = Instantiate(cannonBullet, shootPoint).GetComponent<Bullet>();
        b.Init(shootPoint.forward, this.gameObject);
        b.damage = 99;
        b.transform.parent = null;
    }
}
