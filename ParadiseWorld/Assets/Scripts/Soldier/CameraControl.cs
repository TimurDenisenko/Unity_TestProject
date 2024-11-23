using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [Header("Inputs"), Space(5)]
    [SerializeField] InputAction cameraRotation;
    [SerializeField] InputAction cameraZoom;
    [Space(7.5f)]
    [Header("Characteristics"), Space(5)]
    [SerializeField] Transform follow;
    [SerializeField] float sensitivity = 1f;
    [SerializeField] float speedOfChange = 1f;
    [Space(5)]
    [Header("Vectical angle"), Space(5)]
    [SerializeField] float minVerticalAngle = -45f;
    [SerializeField] float maxVerticalAngle = 45f;
    [Space(5)]
    [Header("Distance to object"), Space(5)]
    [SerializeField] float distance = 6f;
    [SerializeField] float minDistance = 4f;
    [SerializeField] float maxDistance = 15f;
    Vector2 rotation;
    Quaternion targetRotation;
    bool isControl;
    float angleBetweenOne;
    float bValue;
    Vector2 rotationVelocity;
    void Awake()
    {
       SoldierComponents.CameraComponent = this;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraZoom.started += CameraZoom_started;
        angleBetweenOne = Mathf.Abs(minVerticalAngle / distance);
        bValue = minVerticalAngle - (angleBetweenOne * distance);
        ChangeCameraRotation(false);
    }
    private void CameraZoom_started(InputAction.CallbackContext obj)
    {
        distance += cameraZoom.ReadValue<Vector2>().y;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        minVerticalAngle = Mathf.Clamp(angleBetweenOne * distance + bValue, minVerticalAngle, 0);
        ChangeCameraRotation(false);
    }
    private void LateUpdate()
    {
        ChangeCameraRotation();
        ChangeCameraPosition();
    }
    void ChangeCameraRotation(bool isPlayerRequired = true)
    {
        Vector2 cameraRotate = cameraRotation.ReadValue<Vector2>() * sensitivity;
        if (cameraRotate == Vector2.zero && isPlayerRequired) return;
        rotation += cameraRotate;
        rotation.y = Mathf.Clamp(rotation.y, minVerticalAngle, maxVerticalAngle);
        targetRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
        transform.rotation = targetRotation;
    }
    void ChangeCameraPosition()
    {
        transform.position = Vector3.Lerp(transform.position, follow.position - targetRotation * new Vector3(0, 0, distance), speedOfChange);
    }
    void OnEnable()
    {
        Array.ForEach(new InputAction[] { cameraRotation, cameraZoom }, x => x.Enable());
    }
    void OnDisable()
    {
        Array.ForEach(new InputAction[] { cameraRotation, cameraZoom }, x => x.Disable());
    }
}