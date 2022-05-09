using UnityEngine;
using Assets.Helpers;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform _playerCamera;

    [SerializeField]
    private bool _lockCursor = true;

    [SerializeField]
    private float _mouseSensitivity = 3.5f;

    [SerializeField]
    private float _mouseSmoothTime = 0.03f;

    [SerializeField]
    private float _walkSpeed = 6f;

    [SerializeField]
    [Range(0.0f, 0.5f)]
    private float _moveSmoothTime = 0.3f;

    [SerializeField]
    private float _gravity = -13f;

    private float _cameraPitch = 0.0f;
    private CharacterController _characterController;
    private float _velocityY = default;

    private Vector2 _currentMouseDelta = default;
    private Vector2 _currentMouseDeltaVelocity = default;

    private Vector2 _currentDirection = default;
    private Vector2 _currentDirectionVelocity = default;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
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
        UpdateMouseLook();
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector2 targetDirection = new Vector2(InputHelper.HorizontalAxis, InputHelper.VerticalAxis);
        targetDirection.Normalize();

        _currentDirection = Vector2.SmoothDamp(_currentDirection, targetDirection, ref _currentDirectionVelocity, _moveSmoothTime);

        _velocityY += _gravity * Time.deltaTime;

        if (_characterController.isGrounded)
        {
            _velocityY = 0;
        }

        Vector3 velocity = (transform.forward * _currentDirection.y + transform.right * _currentDirection.x) * _walkSpeed + Vector3.up * _velocityY;
        _characterController.Move(velocity * Time.deltaTime);   
    }

    private void UpdateMouseLook()
    {
        var targetMouseDelta = InputHelper.MouseXY;

        _currentMouseDelta = Vector2.SmoothDamp(_currentMouseDelta, targetMouseDelta, ref _currentMouseDeltaVelocity, _mouseSmoothTime);

        _cameraPitch -= _currentMouseDelta.y * _mouseSensitivity;
        _cameraPitch = ClampVerticalAngle(_cameraPitch);

        _playerCamera.localEulerAngles = Vector3.right * _cameraPitch;
        transform.Rotate(Vector3.up * _currentMouseDelta.x * _mouseSensitivity);
    }

    private static float ClampVerticalAngle(float angle) => Mathf.Clamp(angle, -90, 90);
}
