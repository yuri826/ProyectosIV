using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    private bool isActive = true;
    
    public void Move(float speed, int minPos, int spawnPos)
    {
        if (!isActive) return;
        
        this.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);

        if (transform.position.x < minPos)
        {
            this.transform.position = new Vector3(spawnPos, this.transform.position.y, this.transform.position.z);
        }
    }

    public void Activate()
    {
        isActive = true;
    }
}
