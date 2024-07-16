#define LOGGER_ON
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class PlayerMoveStrategy : MoveStrategyBase
{
    private Transform _cameraTransform;

    public PlayerMoveStrategy(Camera mainCamera)
    {
        _cameraTransform = mainCamera.transform;
    }

    // todo need to limit movement speed
    protected override void OnMove(Vector3 axis, float deltaTime)
    {
        axis = _characterModel.IsInAttackPhase ? Vector3.zero : axis;

        // var configSpeed = _characterConfig.WalkSpeed;
        var configSpeed = _characterConfig.SprintSpeed;
        var targetDirection = Quaternion.Euler(0.0f, _cameraTransform.eulerAngles.y, 0.0f) * axis;
        _velocity += targetDirection.normalized * (configSpeed * _characterConfig.SpeedChangeRate * deltaTime);
        _velocity += -_velocity * (_characterConfig.SpeedChangeRate * deltaTime); // friction/resistance

        var horizontalMove = _velocity * deltaTime;
        var verticalMove = new Vector3(0.0f, _verticalVelocity, 0.0f) * deltaTime;
        _controller.Move(horizontalMove + verticalMove);

        _characterModel.MoveSpeed.Value = _velocity.magnitude;
    }
}