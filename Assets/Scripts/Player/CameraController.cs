using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour, IConfigSaveable
{
    public Vector2 cameraSensitivity = new Vector2(5f, 5f);
    private Vector2 xyRotation;

    [SerializeField]
    private bool lockCursor;

    private Camera cam;
    private Controls inputActions;
    private InputAction mouseDelta;

    private Transform defaultParent;
    private Vector3 offset;
    private bool isPeaking = false;

    private void Awake()
    {
        cam = Camera.main;
        inputActions = new Controls();
        mouseDelta = inputActions.Mouse.Delta;
        defaultParent = transform;
        offset = cam.transform.localPosition;
    }

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnEnable()
    {
        mouseDelta.Enable();
    }

    private void OnDisable()
    {
        mouseDelta.Disable();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (isPeaking) 
        {
            cam.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            return; 
        }

        Vector2 mouseInput = mouseDelta.ReadValue<Vector2>();
        xyRotation.x -= mouseInput.y * cameraSensitivity.y * Time.deltaTime;
        xyRotation.y += mouseInput.x * cameraSensitivity.x * Time.deltaTime;

        xyRotation.x = Mathf.Clamp(xyRotation.x, -90f, 90f);
        //transform.Rotate(xyRotation);
        transform.eulerAngles = new Vector3(0f, xyRotation.y, 0f);
        cam.transform.localEulerAngles = new Vector3(xyRotation.x, 0f, 0f);
    }

    public void SwitchTarget()
    {
        cam.transform.SetParent(defaultParent);
        cam.transform.localPosition = offset;
        isPeaking = false;
    }
    public void SwitchTarget(GameObject target)
    {
        cam.transform.SetParent(target.transform);
        cam.transform.localPosition = offset;
        isPeaking = true;
    }

    public void ChangeRotation(Vector3 rotation)
    {
        xyRotation.x = rotation.x;
        xyRotation.y = rotation.y;
    }

    public void LoadData(ConfigData data)
    {
        cameraSensitivity = new Vector2(data.mouseSensitivity, data.mouseSensitivity);
    }

    public void SaveData(ref ConfigData data)
    {
        data.mouseSensitivity = cameraSensitivity.x;
    }
}
