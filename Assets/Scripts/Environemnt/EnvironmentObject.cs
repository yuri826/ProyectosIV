using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    //Se mueve en la dirección que le diga su manager. CUando llega a cierto umbral vuelve al spawn
    public void Move(float speed, int minPos, int spawnPos)
    {
        this.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);

        if (transform.position.x < minPos)
        {
            this.transform.position = new Vector3(spawnPos, this.transform.position.y, this.transform.position.z);
        }
    }
}
