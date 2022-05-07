using System;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Flags]
    public enum RotationDirection
    {
        None,
        Horizontal = 1 << 0,
        Vertical = 1 << 1
    }

    [Tooltip("A multiplier to the input. Describes the maximum speed in degrees/second. To flip vertical rotation, set Y to a negative value")]
    [SerializeField]
    private Vector2 Sensitivity;

    [Tooltip("The rotation acceleration, in degrees/second")]
    [SerializeField]
    private Vector2 Acceleration;

    [Tooltip("The period to wait until resetting the input value. Should be set as low as possible while avoiding stuttering")]
    [SerializeField]
    private float InputLagPeriod;

    [Tooltip("Which directions this object can rotate")]
    [SerializeField]
    private RotationDirection _rotationDirections;

    private Vector2 _currentRotation;
    private Vector2 _currentVelocity;

    private Vector2 _lastInputEvent;
    private float _inputLagTimer;

    private void OnEnable()
    {
        ResetState();
        var cameraAngles = GetCurrentCameraAngles();
        SetLocalRotation(cameraAngles);

        void ResetState()
        {
            _currentVelocity = default;
            _inputLagTimer = default;
            _lastInputEvent = default;
        }

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
            _currentRotation = new Vector2(euler.y, euler.x);
        }
    }

    private void Update()
    {
        UpdateCurrentRotation();
        transform.localEulerAngles = GetDesiredTransform();
    }

    private void UpdateCurrentRotation()
    {
        _currentVelocity = GetDesiredVelocity();
        _currentRotation += _currentVelocity * Time.deltaTime;
        _currentRotation.y = ClampVerticalAngle(_currentRotation.y);
    }

    private Vector2 GetDesiredVelocity()
    {
        var maximumVelocity = GetInputVector() * Sensitivity;

        if ((_rotationDirections & RotationDirection.Horizontal) == 0)
        {
            maximumVelocity.x = 0;
        }

        if ((_rotationDirections & RotationDirection.Vertical) == 0)
        {
            maximumVelocity.y = 0;
        }

        return new Vector2(
            Mathf.MoveTowards(_currentVelocity.x, maximumVelocity.x, Acceleration.x * Time.deltaTime),
            Mathf.MoveTowards(_currentVelocity.y, maximumVelocity.y, Acceleration.y * Time.deltaTime));
    }

    private Vector3 GetDesiredTransform() => new Vector3(_currentRotation.y, _currentRotation.x, 0);

    private Vector2 GetInputVector()
    {
        _inputLagTimer += Time.deltaTime;

        var input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        if (InputIsNonZero() || InputLagTimeHasElapsed())
        {
            _lastInputEvent = input;
            _inputLagTimer = 0;
        }

        return _lastInputEvent;

        bool InputIsNonZero() => !(Mathf.Approximately(0, input.x) && Mathf.Approximately(0, input.y));
        bool InputLagTimeHasElapsed() => _inputLagTimer > InputLagPeriod;
    }

    private float ClampVerticalAngle(float angle)
    {
        return Mathf.Clamp(angle, -90, 90);
    }
}
