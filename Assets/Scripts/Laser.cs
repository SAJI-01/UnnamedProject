
using DG.Tweening;

using UnityEngine;
using UnityEngine.Serialization;


public class Laser : MonoBehaviour
{
    public bool turnOnLaser;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int numberOfReflection = 1;
    [SerializeField] private float maxDistance =10;
    [SerializeField] private float SnapTiming = 0.2f;
    [SerializeField] private LayerMask laserBeamHitLayer;
    [SerializeField] private float sphereSize = 1;
    private Lever lever;
    private bool isPlayerInTrigger,turning;
    


    private void Awake()
    {
        lever = FindObjectOfType<Lever>();
    }
    
    
    private void Update()
    {
         isPlayerInTrigger = Physics.CheckSphere(transform.position, sphereSize, LayerMask.GetMask("Player"));
         
         if (Input.GetMouseButtonDown(0))
         {
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out var hit))
             {
                 if (hit.transform == transform && isPlayerInTrigger)
                 {
                     gameObject.transform.GetChild(1).transform.DORotate(SnapRotate(), SnapTiming).SetEase(Ease.OutBounce);
                     Invoke(nameof(laserTurning), SnapTiming);
                     turning = true;
                 }
             }
         }
         if (turnOnLaser)
         {
             CastRay();
         }
         if(turning && turnOnLaser)
         {
             lineRenderer.positionCount = 0;
             
         }

    }
    
    private void laserTurning()
    {
        turning = false;
    }


    private void CastRay()
    {
        var ray = new Ray(lineRenderer.transform.position, lineRenderer.transform.forward);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        var remainLength = maxDistance;
        for (var i = 0; i <= numberOfReflection; i++)
        {
            if (CheckHits(ref ray, ref remainLength)) break;
        }
        
    }

    private bool CheckHits(ref Ray ray, ref float remainLength)
    {
        if (Physics.Raycast(ray.origin, ray.direction, out var hit, remainLength, laserBeamHitLayer))
        {
            ObjectToHit(hit);
            lineRenderer.positionCount += 1; //increase the position count
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
            //lineRenderer.positionCount - 1  if set-position is 0, first position-count =1 in for-loop count+1 so it will be 2 but we want 1 so we use -1
            remainLength -= Vector3.Distance(ray.origin, hit.point);
            ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
        }
        else
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + (ray.direction * remainLength));
            return true;
        }
        return false;
        //if turn off and turn on set-position to 0
    }

    private void ObjectToHit(RaycastHit hit)
    {
        if (hit.transform.CompareTag("MoveAbleObject") && hit.transform.GetComponent<MovableBlock>().isMoveAble)
        {
            hit.transform.GetComponent<MovableBlock>().MoveAbleBlock(hit, lever);
        }
    }


    private Vector3 SnapRotate()
    {
        var currentRotation = gameObject.transform.GetChild(1).transform.eulerAngles;
        var snapRotation = new Vector3(0, 0, 0);
        switch (currentRotation.y)
        {
            case < 45:
                snapRotation = new Vector3(0, 90,0);
                break;
            case < 135:
                snapRotation = new Vector3(0, 180, 0);
                break;
            case < 225:
                snapRotation = new Vector3(0, 270, 0);
                break;
            case < 315:
                snapRotation = new Vector3(0, 0, 0);
                break;
        }
        return snapRotation;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereSize);
    }

}
