using UnityEngine;

public class ToolObj : PickableObj
{
    public override void Throw(Vector3 throwDir, out bool dropObj)
    {
        dropObj = false;
    }
}
