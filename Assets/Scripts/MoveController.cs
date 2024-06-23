#define LOGGER_ON
using System;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class MoveController : MonoBehaviour
{
    private const float SpeedOffset = 0.1f;
    [SerializeField] private float walkSpeed = 2.0f;
    //[SerializeField] private float sprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float speedChangeRate = 10.0f;

    private float _targetRotation;
    private float _rotationVelocity;
    private float _speed;
    private float _verticalVelocity;
    private Transform _transform;

    private CharacterController _controller;
    private Transform _camera;
    private InputModel _inputModel;
    private CharacterModel _characterModel;

    public void Init(InputModel inputModel, CharacterModel characterModel,
        CharacterController characterController, Transform relativeCamera)
    {
        _inputModel = inputModel;
        _characterModel = characterModel;
        _controller = characterController;
        _camera = relativeCamera;

        _transform = _controller.transform;
    }

    private void Update()
    {
        if (_inputModel == null)
            return;
            
        var axis = _inputModel.OnMove.Value;
        OnMove(axis);
    }

    public void Init(Transform camera)
    {
        _camera = camera;
    }

    private void OnMove(Vector3 axis)
    {
        var targetSpeed = walkSpeed;

        if (axis == Vector3.zero)
            targetSpeed = 0.0f;

        var curHorSpeedVel = _controller.velocity;
        var curHorSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        if (curHorSpeed < targetSpeed - SpeedOffset || curHorSpeed > targetSpeed + SpeedOffset)
        {
            _speed = Mathf.Lerp(curHorSpeed, targetSpeed, speedChangeRate * Time.deltaTime);
            _speed = (float)Math.Round(_speed, 3);
        }
        else
        {
            _speed = targetSpeed;
        }

        if (axis != Vector3.zero)
        {
            var inputDirection = axis.normalized;
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        var horizontalMove = targetDirection.normalized * (_speed * Time.deltaTime);
        var verticalMove = new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
        _controller.Move(horizontalMove + verticalMove);

        _characterModel.MoveSpeed.Value = _speed;
    }
}