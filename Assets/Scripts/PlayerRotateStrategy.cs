using UnityEngine;

public class PlayerRotateStrategy : RotateStrategyBase
{
    private Transform _cameraTransform;

    public PlayerRotateStrategy(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;
    }

    // todo roman implement FSM if more states
    protected override void OnRotate(Vector3 axis, float deltaTime)
    {
        if (!_characterModel.IsInAttackPhase)
        {
            if (_characterModel.Target.Value != null)
            {
                var inputDirection = _characterModel.Target.Value.position - _transform.position;
                inputDirection.y = 0.0f;
                var _targetRotation = Quaternion.LookRotation(inputDirection).eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(
                    _transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.ActingRotationSmoothTime);
                _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            else if (axis != Vector3.zero)
            {
                var inputDirection = axis.normalized;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(
                    _transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
                _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
    }
}