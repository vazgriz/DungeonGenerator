using System;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private Transform _playerCamera;

    [SerializeField]
    private bool _lockCursor = true;

    [SerializeField]
    private float MouseSensitivity = 3.5f;

    private float _cameraPitch = 0.0f;

    private void Start()
    {
        if (_lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnEnable()
    {
        var cameraAngles = GetCurrentCameraAngles();
        SetLocalRotation(cameraAngles);

        Vector3 GetCurrentCameraAngles()
        {
            var cameraEulerAngles = NormaliseEuler(transform.localEulerAngles);
            cameraEulerAngles.x = ClampVerticalAngle(cameraEulerAngles.x);
            return cameraEulerAngles;

            Vector3 NormaliseEuler(Vector3 euler)
            {
                // Euler angles range from [0, 360] but we want [-180, 180]
                if (euler.x >= 180)
                {
                    euler.x -= 360;
                }
                return euler;
            }
        }

        void SetLocalRotation(Vector3 euler)
        {
            transform.localEulerAngles = euler;
        }
    }

    private void Update()
    {
        var mouseDelta = GetMouseInput();

        _cameraPitch -= mouseDelta.y * MouseSensitivity;
        _cameraPitch = ClampVerticalAngle(_cameraPitch);

        _playerCamera.localEulerAngles = Vector3.right * _cameraPitch;
        transform.Rotate(Vector3.up * mouseDelta.x * MouseSensitivity);
    }

    private Vector2 GetMouseInput()
    { 
        var input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        return input;
    }

    private float ClampVerticalAngle(float angle)
    {
        return Mathf.Clamp(angle, -90, 90);
    }
}
