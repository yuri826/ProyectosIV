using UnityEngine;

public class GunObj : ToolObj
{
    [SerializeField] private GameObject playerBullet;
    [SerializeField] private Vector3 shootOffset;
    
    public override void Throw(Vector3 throwDir, out bool dropObj)
    {
        base.Throw(throwDir, out dropObj);
        
        print("Shoot");
        PlayerBullet b = Instantiate(playerBullet, this.transform.position + shootOffset, Quaternion.identity).GetComponent<PlayerBullet>();
        b.shootDir = throwDir;
    }
}
