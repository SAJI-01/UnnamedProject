using UnityEngine;
using UnityEngine.Serialization;

public class Teleport : MonoBehaviour
{ 
    public GameObject teleportPosition;
    [SerializeField] private float sphereSize = 0.5f;
    [SerializeField] private Vector3 spherePosition;
    [SerializeField] private LayerMask playerLayer;
    private bool isPlayerInside;
    private PlayerMovement player;
    
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }
    
    private void Update()
    {
       
        isPlayerInside = Physics.CheckSphere(transform.position + spherePosition, sphereSize, playerLayer);
        
        if(isPlayerInside)
        {
            player.transform.position = teleportPosition.transform.position;
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + spherePosition, sphereSize);
    }

}
