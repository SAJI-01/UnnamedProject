using DG.Tweening;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    #region Variables

    
    private Vector2 delta;

    private bool isFreeRotation;
    [HideInInspector] public bool isRotating;
    private bool isBusy;
    private bool isMoving;

    private float xRotation;
    private Transform t;


    [Header("Settings")] [SerializeField] private float rotationSpeed = 0.1f;

    #endregion

    private void Awake()
    {
        t = transform;
        xRotation = t.rotation.eulerAngles.x;
    }

    

    private void Update()
    {
        delta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        OnRotate();

        if (!isRotating) return;
        t.Rotate(new Vector3(xRotation, delta.x * Vector3.right.x * rotationSpeed, 0.0f));
        t.rotation = Quaternion.Euler(xRotation, t.rotation.eulerAngles.y, 0.0f);

    }

    private void OnRotate()
    {
        if (isBusy) return;
        isRotating = Input.GetMouseButton(1);
        if (Input.GetMouseButtonUp(1))
        {
            isBusy = true;
            SnapRotation();
        }
    }
    
    private void SnapRotation()
    {
        transform.DORotate(SnappedVector(), 0.25f).SetEase(Ease.OutCirc).OnComplete(() => { isBusy = false; });
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