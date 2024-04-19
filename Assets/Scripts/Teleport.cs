using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject TpPosition;
    PlayerMovement player;
    private bool isPlayerInside;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector3 spherePosition;
    [SerializeField] private float sphereSize = 0.5f;
    
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    

    private void Update()
    {
       
        isPlayerInside = Physics.CheckSphere(transform.position + spherePosition, sphereSize, playerLayer);
        
        if(isPlayerInside)
        {
            player.transform.position = TpPosition.transform.position;
        }
        
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + spherePosition, sphereSize);
    }

}
