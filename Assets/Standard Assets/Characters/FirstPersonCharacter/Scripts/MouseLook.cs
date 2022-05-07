using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Tooltip("A multiplier to the input. Describes the maximum speed in degrees/second. To flip vertical rotation, set Y to a negative value")]
    [SerializeField]
    private Vector2 Sensitivity;

    [Tooltip("The rotation acceleration, in degrees/second")]
    [SerializeField]
    private Vector2 Acceleration;

    [Tooltip("The period to wait until resetting the input value. Should be set as low as possible while avoiding stuttering")]
    [SerializeField]
    private float InputLagPeriod;

    private Vector2 _currentRotation;
    private Vector2 _currentVelocity;

    private Vector2 _lastInputEvent;
    private float _inputLagTimer;

    private void Update()
    {
        UpdateCurrentRotation();
        transform.localEulerAngles = GetDesiredTransform();
    }

    private void UpdateCurrentRotation()
    {
        _currentVelocity = GetDesiredVelocity();
        _currentRotation += _currentVelocity * Time.deltaTime;
    }

    private Vector2 GetDesiredVelocity()
    {
        var maximumVelocity = GetInputVector() * Sensitivity;
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
}
