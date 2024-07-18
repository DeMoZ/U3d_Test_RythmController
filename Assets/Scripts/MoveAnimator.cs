#define LOGGER_ON
using System;
using UnityEngine;

public class MoveAnimator : IDisposable
{
    private CharacterModel _characterModel;
    private Animator _animator;
    private int _animIDIsRunning;
    private int _animIDIsMoving;
    private int _animIDForwardSpeed;
    private int _animIDRightSpeed;
    // private int _animIDUpSpeed;

    public MoveAnimator(CharacterModel characterModel, Animator animator)
    {
        _characterModel = characterModel;
        _animator = animator;

        _characterModel.MoveSpeed.Subscribe(OnMoveSpeedChanged);
        _animIDIsRunning = Animator.StringToHash(AnimatorConstants.IsRunning);
        _animIDIsMoving = Animator.StringToHash(AnimatorConstants.IsMoving);
        _animIDForwardSpeed = Animator.StringToHash(AnimatorConstants.MoveForward);
        _animIDRightSpeed = Animator.StringToHash(AnimatorConstants.MoveRight);
        // _animIDUpSpeed = Animator.StringToHash(AnimatorConstants.MoveUp);
    }

    public void Dispose()
    {
        _characterModel.MoveSpeed.Unsubscribe(OnMoveSpeedChanged);
    }

    // todo roman separate move and run animations and change in animator controller
    private void OnMoveSpeedChanged(Vector3 value)
    {
        var isRunning = _characterModel.IsRunning;
        var isMoving = value.magnitude > 0.05f;

        if (value.magnitude < 0.2f && isRunning)
            isRunning = false;

        _animator.SetBool(_animIDIsRunning, isRunning);
        _animator.SetBool(_animIDIsMoving, isMoving);

        _animator.SetFloat(_animIDForwardSpeed, value.z);
        _animator.SetFloat(_animIDRightSpeed, value.x);
        //_animator.SetFloat(_animIDUpSpeed, value.y);
    }
}