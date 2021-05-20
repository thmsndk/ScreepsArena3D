using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraPlayer : MonoBehaviour
{
    //[SerializeField] private float panSpeed = 20f;
    //[SerializeField] private float zoomSpeed = 3f;
    //[SerializeField] private float zoomInMax = 40f;
    //[SerializeField] private float zoomOutMax = 90f;

    private CameraControls cameraControls; // TODO: InputManager for camera controls, possibly other controls as well
    private CinemachineVirtualCamera virtualCamera;
    private Transform cameraTransform;

    [SerializeField] private Transform player;

    private void Awake()
    {
        cameraControls = new CameraControls();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        cameraTransform = virtualCamera.VirtualCameraGameObject.transform;
    }

    private void OnEnable()
    {
        cameraControls.Enable();
    }
    private void OnDisable()
    {
        cameraControls.Disable();
    }

    public Vector2 GetMovement()
    {
        return cameraControls.Camera.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetLook()
    {
        return cameraControls.Camera.Look.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // WASD movement
        var movement = GetMovement();
        var move = cameraTransform.forward * movement.y // move camera in looking direction
                 + cameraTransform.right * movement.x; // strafe camera left and right
        const float moveSpeed = 20f;

        if (move.x != 0 || move.y != 0)
        {
            player.position = Vector3.Lerp(cameraTransform.position, cameraTransform.position + move, moveSpeed * Time.deltaTime);
        }

    }
}
