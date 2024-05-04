using UnityEngine;
using DG.Tweening;

public class Lever : MonoBehaviour
{
    public bool toggle;
    private bool isPlayerNear;
    private bool isRotating;
    
    [Space]
    public GameObject nearByBlock; 
    public GameObject objectToToggle;
    
    
    [Space]
    [SerializeField] private float radius = 0.62f;
    
    
    private Laser laser;
    
    
    
    private void Awake()
    {
        laser = FindObjectOfType<Laser>();
        
    }
    
    private void Update()
    {
        
        isPlayerNear = Physics.CheckSphere(transform.position, radius, LayerMask.GetMask("Player"));
        if(isRotating)return;
        if (isPlayerNear && toggle)
        {
            isRotating = true;
            if(isRotating)
                transform.GetChild(1).transform.DOLocalRotate(new Vector3(-40, 0, -90), 0.5f).SetEase(Ease.InBack).onComplete = ToggleObject;
        }

    }

    private void ToggleObject()
    {
        
        if (objectToToggle.gameObject.CompareTag("movableBlockWithLaser"))
        {
            objectToToggle.SetActive(true);
            GameManager.instance.Link(true, false, false, false);
        }
        
        if(objectToToggle.gameObject.CompareTag("Laser"))
        {
            laser.turnOnLaser = true;
        }

        if (!objectToToggle.gameObject.CompareTag("normalMovableBlock")) return;
        objectToToggle.SetActive(true);
        GameManager.instance.Link(false, false,  true , false);


    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

