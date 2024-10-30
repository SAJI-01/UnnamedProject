using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    #region Variables

    [FormerlySerializedAs("rotationSpeed")]
    [Header("Settings")] 
    public bool isRotating;
    [SerializeField] private float isometricRotSpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 0.01f; 
    [SerializeField] private float minCameraZoom = 7f;
    [SerializeField] private float maxCameraZoom = 14f;
    private const float XRotation = 35.264f;
    private int endValue;
    private Transform t;
    private Vector2 delta;
    private bool isBusy;
    private PlayerMovement player;
    
    
    
    

    #endregion

    private void Awake()
    {
        t = transform;
        player = FindObjectOfType<PlayerMovement>();
    }
    
    private void Update()
    {
        if (player.isWalking) return;
        Zoom();
        OnRotate();
        IsometricRotateCamera();
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        delta = context.ReadValue<Vector2>();
    }

    private void IsometricRotateCamera()
    {
        if (!isRotating) return;
        t.Rotate(new Vector3(XRotation, delta.x * isometricRotSpeed, 0.0f));
        t.rotation = Quaternion.Euler(XRotation, t.rotation.eulerAngles.y, 0.0f);
    }
    private void OnRotate()
    {
        if (isBusy) return;
        isRotating = Input.GetMouseButton(0);
        
        if (!Input.GetMouseButtonUp(0)) return;
        isBusy = true;
        SnapRotation();
    }
    
    private void SnapRotation()
    {
        transform.DORotate(SnappedVector(), 0.25f).SetEase(Ease.OutCirc).OnComplete(() => { isBusy = false; });
    }

    private Vector3 SnappedVector()
    {
        var currentY = t.rotation.eulerAngles.y;
        endValue = currentY switch
        {
            >= 0f and <= 90f => 45,
            >= 90f and <= 180f => 135,
            >= 180f and <= 270f => 225,
            _ => 315
        };

        return new Vector3(math.abs(XRotation), Mathf.Abs(endValue), math.abs(0));
    }
    
    private void Zoom()
    {
        // for touch devices change the projection size for zooming
        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            var prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            var currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            var difference = currentMagnitude - prevMagnitude;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - difference * zoomSpeed, minCameraZoom, maxCameraZoom);
        }
        
        // for desktop devices change the projection size for zooming
        if (Input.mouseScrollDelta.y > 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomSpeed, minCameraZoom, maxCameraZoom);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + zoomSpeed, minCameraZoom, maxCameraZoom);
        }
    }
}