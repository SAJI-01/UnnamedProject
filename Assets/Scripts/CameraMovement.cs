using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;


public class CameraMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Quaternion eulersAngle;
    private UIManager uiManager;
    private PlayerMovement playerMovement;
    private Vector2 delta;
    
    private bool isFreeRotation;
    private bool isRotating;
    private bool isBusy;
    
    private float xRotation;
    private Quaternion lastRotation;
    
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 0.1f;
     #endregion

    private void Awake()
    {
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
        }
    }

    private void Update() 
    {
        eulersAngle = transform.rotation;
        
        if (isRotating && !uiManager.isFreeView )
        {
            transform.Rotate(new Vector3(xRotation, delta.x * Vector3.right.x * rotationSpeed, 0.0f));
            transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0.0f);
        }
        
        if (isFreeRotation && uiManager.isFreeView)
        {
            transform.Rotate(new Vector3(-delta.y * Vector3.up.y * rotationSpeed, delta.x * Vector3.right.x * rotationSpeed, 0.0f));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
        if (uiManager.isFreeViewInverse)
        {
            transform.DORotate(
                new Vector3(lastRotation.eulerAngles.x, lastRotation.eulerAngles.y, lastRotation.eulerAngles.z), 0.75f).SetEase(Ease.OutSine);
            uiManager.isFreeViewInverse = false;
        }
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
