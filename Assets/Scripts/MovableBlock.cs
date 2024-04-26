using UnityEngine;
using UnityEngine.Splines;


public class MovableBlock : MonoBehaviour
{
    private bool isPlayer;
    public bool isLaserHit;
    public bool normalBlock;
    [SerializeField] private Transform hitBox;


    [SerializeField] private Vector3 _wireBoxSize = new Vector3(0.200000003f, 1.03499997f, 1f);
    [SerializeField] private Vector3 _wireBoxPosition = new Vector3(0.400000006f, 0f, 0f);
    
    public float time = 3f;
    
    public SplineAnimate splineAnimate;
    private PlayerMovement _player;
    
    private void Awake()
    {
        splineAnimate.ElapsedTime = 3f;
        _player = FindObjectOfType<PlayerMovement>();
        if (normalBlock)
        {
            hitBox.GetComponent<BoxCollider>().enabled = false;hitBox.GetComponent<BoxCollider>().enabled = false;
            hitBox.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            hitBox.transform.localPosition = new Vector3(0, 0.27f, 0);
        }
    }
    
    private void Update()
    {
        isPlayer = Physics.CheckBox(transform.position + _wireBoxPosition, _wireBoxSize, Quaternion.identity, LayerMask.GetMask("Player"));
        
        HandleMovement();
    }

    private void HandleMovement()
    {
        
        if(isPlayer && isLaserHit)
        {
            splineAnimate.Play();
            isLaserHit = false;
        }
        if(isPlayer && normalBlock)
        {
            splineAnimate.Play();
            isLaserHit = false;
        }
        if(splineAnimate.IsPlaying && isPlayer)
        {
            _player.transform.position = splineAnimate.transform.position + Vector3.up + new Vector3(0, -0.5f, 0);
        }
        if(splineAnimate.ElapsedTime >= time && !isLaserHit)
        {
            splineAnimate.Pause();
            splineAnimate.ElapsedTime = 3f;
            GameManager.instance.Link(false, true, false , false);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + _wireBoxPosition, _wireBoxSize);
        
    }
}
