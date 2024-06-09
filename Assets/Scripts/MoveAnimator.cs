#define LOGGER_ON
using System;
using UnityEngine;

public class MoveAnimator : IDisposable
{
    private CharacterModel _characterModel;
    private Animator _animator;
    private int _animIDSpeed;

    public MoveAnimator(CharacterModel characterModel, Animator animator)
    {
        _characterModel = characterModel;
        _animator = animator;

        _characterModel.MoveSpeed.Subscribe(OnMoveSpeedChanged);
        _animIDSpeed = Animator.StringToHash(AnimatorConstants.MoveSpeed);
    }

    public void Dispose()
    {
        _characterModel.MoveSpeed.Unsubscribe(OnMoveSpeedChanged);
    }

    private void OnMoveSpeedChanged(float value)
    {
        _animator.SetFloat(_animIDSpeed, value);
    }
}