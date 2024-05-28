using DG.Tweening;
using UnityEngine;



public class Laser : MonoBehaviour
{
    [Header("Booleans")]
    [Space] public bool turnOnLaser;
    private bool isPlayerInSphere,isTurning;
    
    
    
    [Header("Settings")]
    [Space]
    [SerializeField] private int numberOfReflection = 1;
    [SerializeField] private float maxDistance =30;
    [SerializeField] private float sphereSize = 1;
    [SerializeField] private float snapTiming = 0.2f;
    
    
    [Header("References")]
    [Space]
    [SerializeField] private LineRenderer lineRenderer;
    
    
    
    private void Update()
    {
         isPlayerInSphere = Physics.CheckSphere(transform.position, sphereSize, LayerMask.GetMask("Player"));
         
         if (Input.GetMouseButtonDown(0))
         {
             var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             if (Physics.Raycast(ray, out var hit))
             {
                 if (hit.transform == transform && isPlayerInSphere)
                 { 
                     gameObject.transform.GetChild(1).transform.DORotate(SnapRotate(), snapTiming).SetEase(Ease.OutBounce);
                     Invoke(nameof(LaserTurning), snapTiming);
                     isTurning = true;
                 }
             }
         }
         if (turnOnLaser)
         {
             CastRay();
         }
         if(isTurning && turnOnLaser)
         {
             lineRenderer.positionCount = 0;
             
         }
    }
    
    private void LaserTurning()
    {
        isTurning = false;
    }


    private void CastRay()
    {
        var ray = new Ray(lineRenderer.transform.position, lineRenderer.transform.forward);
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        for (var i = 0; i < numberOfReflection; i++)
            if (Physics.Raycast(ray.origin, ray.direction, out var hit, maxDistance))
            {
                ray = Ray(hit, ray);
            }
            else
            {
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction * maxDistance);
            }
    }

    private Ray Ray(RaycastHit hit, Ray ray)
    {
        if (hit.transform.CompareTag("ReflectiveSurface"))
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
            ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
        }
        else
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
        }

        if (!hit.transform.CompareTag("HitBoxOfMovableObject")) return ray;
        hit.transform.parent.GetComponent<MovableBlock>().isLaserHit = true;
        hit.transform.gameObject.transform.DOScale(new Vector3(.5f, .5f, .5f), 1f)
            .SetEase(Ease.InQuad).onComplete = () =>
        {
            hit.transform.gameObject.transform.DOLocalMove(new Vector3(0f, .27f, 0f), .5f)
                .SetEase(Ease.OutBack).onComplete = () => { lineRenderer.enabled = false; };
        };

        return ray;
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
