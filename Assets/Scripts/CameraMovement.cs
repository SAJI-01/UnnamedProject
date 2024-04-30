using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

//testing

public class CameraMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Quaternion eulerAngle;
    private UIManager uiManager;
    private PlayerMovement playerMovement;
    private Vector2 delta;
    
    private bool isFreeRotation;
    [HideInInspector] public bool isRotating;
    private bool isBusy;
    
    private float xRotation;
    private Quaternion lastRotation;
    
    
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 0.1f;
     #endregion

    private void Awake()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        xRotation = transform.rotation.eulerAngles.x;
        uiManager = FindObjectOfType<UIManager>();
        lastRotation = transform.rotation;
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
        delta = context.ReadValue<Vector2>();
    }
    

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (isBusy) return;
            isRotating = context.started || context.performed;
            isFreeRotation = context.started || context.performed;
        if (context.canceled && !uiManager.isFreeView)
        {
                lastRotation = transform.rotation;
                isBusy = true;
                SnapRotation();
        }
        else if(context.canceled && uiManager.isFreeView)
        {
                isBusy = true;
                SmoothRotation();
        }
    }

    private void Update() 
    {
        var t = transform;
        var pt = playerMovement.transform;
        eulerAngle = t.rotation;
        
        if (isRotating && !uiManager.isFreeView )
        {
            t.Rotate(new Vector3(xRotation, delta.x * Vector3.right.x * rotationSpeed, 0.0f));
            t.rotation = Quaternion.Euler(xRotation, t.rotation.eulerAngles.y, 0.0f);
        }
        
        if (isFreeRotation && uiManager.isFreeView)
        {
            t.Rotate(new Vector3(-delta.y * Vector3.up.y * rotationSpeed, delta.x * Vector3.right.x * rotationSpeed, 0.0f));
        }
        if (uiManager.isFreeViewInverse)
        {
            t.DORotate(
                new Vector3(lastRotation.eulerAngles.x, lastRotation.eulerAngles.y, lastRotation.eulerAngles.z), 0.75f).SetEase(Ease.OutSine);
            uiManager.isFreeViewInverse = false;
        }
        var position = new Vector3(pt.position.x,pt.position.y + 1.5f, pt.position.z);

        t.position = Vector3.Lerp(t.position, position, 0.1f);
        
    }
    
    private void SmoothRotation()
    {
        var t = transform;
        t.DORotate(new Vector3(t.rotation.eulerAngles.x,
            t.rotation.eulerAngles.y, t.rotation.eulerAngles.z),
            0f).SetEase(Ease.Linear).onComplete = () =>
        {
            isBusy = false;
        };
    }
    


    private void SnapRotation()
    {
        transform.DORotate(SnappedVector(), 0.25f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
                isBusy = false;
        });
    }
    
    private Vector3 SnappedVector()
    {
        var endValue = 0.0f;
        var currentY = Mathf.Ceil(transform.rotation.eulerAngles.y);

        endValue = currentY switch
        {
            >= 0 and <= 90 => 45,
            >= 91 and <= 180 => 135,
            >= 181 and <= 270 => 225, 
            _ => 315
            
        };
        return new Vector3(xRotation, endValue, 0.0f);
    }
    
}
