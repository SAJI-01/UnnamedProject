using System.Collections.Generic;
using UnityEngine;



public class WalkablePath : MonoBehaviour
{
    #region Variables
    public List<WalkPath> possiblePaths = new List<WalkPath>();
    
    [Space]
    public Transform previousBlock;
    
    [Space]
    [Header("Offsets")]
    public float walkPointOffset = .5f;
    public float stairOffset = .4f;
    
    [Space]
    [Header("Speeds")]
    public float walkPointSpeed = 1f;
    public float stairSpeed = 1.5f;
    
    [Space]
    [Header("Booleans")]
    public bool isStair = false;
    public bool movingGround = false;
    //public bool isButton;
    
    
    #endregion

    public Vector3 GetWalkPoint()
    {
        float stair = isStair ? stairOffset : 0; // change the offset for vector3 position to illusion
        return transform.position + transform.up  * walkPointOffset - transform.up * stair; //return the walk point
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        float stair = isStair ? .4f : 0;
        Gizmos.DrawSphere(GetWalkPoint(), .1f);
        if(possiblePaths == null) return;
        foreach (WalkPath p in possiblePaths)
        {
            if(p.target == null) return;
            Gizmos.color = p.active ? Color.black : Color.clear;
            Gizmos.DrawLine(GetWalkPoint(), p.target.GetComponent<WalkablePath>().GetWalkPoint());
        }
        

    }
}

[System.Serializable] 
public class WalkPath
{
    public Transform target;
    public bool active = true;
}   
