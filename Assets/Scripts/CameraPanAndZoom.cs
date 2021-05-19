using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraPanAndZoom : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomSpeed = 3f;
    [SerializeField] private float zoomInMax = 40f;
    [SerializeField] private float zoomOutMax = 90f;

    private CinemachineInputProvider inputProvider;
    private CinemachineVirtualCamera virtualCamera;
    private Transform cameraTransform;

    private void Awake()
    {
        inputProvider = GetComponent<CinemachineInputProvider>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        cameraTransform = virtualCamera.VirtualCameraGameObject.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var x = inputProvider.GetAxisValue(0);
        var y = inputProvider.GetAxisValue(1);
        var z = inputProvider.GetAxisValue(2);

        if (x !=  0 || y != 0)
        {
            PanScreen(x, y);
        }

        if (z!= 0)
        {
            Zoom(z);
        }
    }
    public void Zoom(float increment)
    {
        /*
         * You are doing this in 2d game so it is less important but you want to avoid changing the Field of View for zoom. 
         * Instead translate the camera along its forward vector. 
         * In a 3d game, adjusting the field of view can distort your visuals. 
         * Sometimes you might want that like a scope or something but in an RTS you probably don't want to stretch the edge of your screen
         */
        var fov = virtualCamera.m_Lens.FieldOfView; // TODO: do something else in 3D
        var target = Mathf.Clamp(fov + increment, zoomInMax, zoomOutMax); // TODO: invert on action?
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(fov, target, zoomSpeed * Time.deltaTime);
    }
    public Vector3 PanDirection(float x, float y)
    {
        var direction = Vector3.zero;
        if (y >= Screen.height * .95f)
        {
            direction.z += 1;
        }
        else if (y <= Screen.height * .05f)
        {
            direction.z -= 1;
        }

        if (x >= Screen.width * .95f)
        {
            direction.x += 1;
        }
        else if (x <= Screen.width * .05f)
        {
            direction.x -= 1;
        }

        return direction;
    }

    public void PanScreen(float x, float y)
    {
        var direction = PanDirection(x, y);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraTransform.position + direction, panSpeed * Time.deltaTime);
    }
}
