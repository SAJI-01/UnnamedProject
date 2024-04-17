using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Lever : MonoBehaviour
{
    public bool turnOnLever;
    private bool isPlayerNearLeverLineSphere;
    public GameObject nearByBlock;
    public GameObject previousBlocks;
    public GameObject objectToToggle;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask playerLayer;
    private MovableBlock movableBlock;
    private Laser laser;
    private bool isLeverRotating;
    
    private void Awake()
    {
        laser = FindObjectOfType<Laser>();
        movableBlock = FindObjectOfType<MovableBlock>();
    }
    
    private void Update()
    {
        
        isPlayerNearLeverLineSphere = Physics.CheckSphere(transform.position, radius, playerLayer);
        if(isLeverRotating) return;
        
        if (isPlayerNearLeverLineSphere && turnOnLever)
        { 
            isLeverRotating = true;//prevent multiple click
            
            if (isLeverRotating)
            {
                transform.GetChild(1).transform.DOLocalRotate(new Vector3(-40, 0, -90), 0.5f).SetEase(Ease.InBack).onComplete = ToggleObject;
            }
           
        }
    }

    private void ToggleObject()
    {
        #region MovableObject
        if (objectToToggle.gameObject.CompareTag("MoveAbleObject"))
        {
            objectToToggle.SetActive(true);
            previousBlocks.gameObject.GetComponent<WalkablePath>().possiblePaths[1].active = true;
            movableBlock.gameObject.GetComponent<WalkablePath>().possiblePaths[1].active = true;
        }
        #endregion
        if(objectToToggle.gameObject.CompareTag("Laser"))
        {
            laser.turnOnLaser = true;
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

