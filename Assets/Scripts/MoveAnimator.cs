//#define LOGGER_ON
using UnityEngine;

public class MoveAnimator
{
    private CharacterModel _characterModel;
    private Animator _animator;

    public MoveAnimator(CharacterModel characterModel, Animator animator)
    {
        _characterModel = characterModel;
        _animator = animator;

        _characterModel.MoveSpeed.Subscribe(OnMoveSpeedChanged);
    }

    private void OnMoveSpeedChanged(float value)
    {
        _animator.SetFloat("Speed", value);
    }
}