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
    
    public bool isStair;
    public bool dontRotatePlayerInParticularBlock;
    
    
    #endregion

    public Vector3 GetWalkPoint()
    {
        var t = transform;
        float stair = isStair ? stairOffset : 0; 
        return t.position + t.up  * walkPointOffset - t.up * stair; //return the walk point
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        var stair = isStair ? .4f : 0;
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
