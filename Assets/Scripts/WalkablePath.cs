using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class WalkablePath : MonoBehaviour
{
    #region Variables
    
    public enum PlayerDirection //Show this in the inspector
    {
        Right,
        Left,
        Forward,
        Backward
    }
    
    
    [Space]
    public List<WalkPath> possiblePaths = new List<WalkPath>();

    [Space] 
    public Transform previousBlock;
    private PlayerMovement player;

    [Space] 
    [Header("Offsets")]
    public float walkPointOffset = .5f;
    public float stairOffset = .4f;

    [Space] [Header("Speeds")] 
    public float walkPointSpeed = 1f;
    public float stairSpeed = 1.5f;

    [Space] [Header("Booleans")] 
    public bool isButton;
    public bool isStair;
    public bool dontRotate;

    [Space] [Header("Colors")] 
    private readonly Color selectedColor = Color.green;
    private readonly Color inactiveColor = Color.gray;

    #endregion
    
    //playerDirection 
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }
    
    

    public Vector3 GetWalkPoint()
    {
        var t = transform;
        float stair = isStair ? stairOffset : 0;
        return t.position+ t.up * walkPointOffset - t.up * stair;
    }

    private void OnDrawGizmos()
    {
        if (player == null) player = FindObjectOfType<PlayerMovement>();
        
        
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(GetWalkPoint(), .1f);

        if (!player.isSearchComplete) return;
        foreach (var p in player.pathNodes)
        {
            if (p == player.pathNodes.First())
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(player.pathNodes.First().GetComponent<WalkablePath>().GetWalkPoint(), new Vector3(0.25f, 0.25f, 0.25f));
            }
            else if (p == player.clickedCube)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(player.clickedCube.GetComponent<WalkablePath>().GetWalkPoint(), new Vector3(0.25f, 0.25f, 0.25f));
            }
            else
            {
                Gizmos.color = selectedColor;
                Gizmos.DrawCube(p.GetComponent<WalkablePath>().GetWalkPoint(), new Vector3(0.15f, 0.15f, 0.15f));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (possiblePaths == null)
            return;
        
        Gizmos.color = selectedColor;
        Gizmos.DrawSphere(GetWalkPoint(), .1f);

        foreach (WalkPath p in possiblePaths)
        {
            if (p.target != null)
            {
                Gizmos.color = p.active ? selectedColor : inactiveColor;
                Gizmos.DrawLine(GetWalkPoint(), p.target.GetComponent<WalkablePath>().GetWalkPoint());
            }
        }
    }
}


[System.Serializable] 
public class WalkPath
{
    public Transform target;
    public bool active = true;
}   
