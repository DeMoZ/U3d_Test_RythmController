using System;
using System.Collections.Generic;
using Attack;
using UnityEngine;

public interface ICharacterAnimator : IDisposable
{
}

public class CharacterAnimator : ICharacterAnimator
{
    private static string StatePrefix = "Attack";

    private Dictionary<string, AnimationClip> _cashedStateAnimations;

    private readonly AttackPlayerData _attackPlayerData;
    private readonly Character _character;
    private readonly IAttackRepository _attackRepository;

    public CharacterAnimator(AttackPlayerData attackPlayerData, Character character, IAttackRepository attackRepository)
    {
        _attackPlayerData = attackPlayerData;
        _character = character;
        _attackRepository = attackRepository;
        CashStateAnimations();
        _attackPlayerData.AttackSequenceState.Subscribe(OnAttackSequenceStateChanged);
    }

    public void Dispose()
    {
        _attackPlayerData.AttackSequenceState.Unsubscribe(OnAttackSequenceStateChanged);
    }

    private void OnAttackSequenceStateChanged(AttackState attackState)
    {
        switch (attackState)
        {
            case AttackState.None:
                break;
            case AttackState.Idle:
                break;
            case AttackState.Attack:
                TriggerAttackAnimation();
                break;
            case AttackState.SequenceReady:
                break;
            case AttackState.SequenceFail:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackState), attackState, null);
        }
    }

    private void TriggerAttackAnimation()
    {
        var stateName = $"{StatePrefix}{_attackPlayerData.CurrentSequenceCode}";

        var clip = _cashedStateAnimations[stateName];
        if (clip == default)
            return;

        _character.Animator.Play(stateName: stateName, normalizedTime: 0f, layer: -1);

        var length = clip.length;
        var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceCode);
        _character.Animator.speed = length / (time + time * 1.02f); // todo roman reset animator speed to 1 after animation
    }

    private void CashStateAnimations()
    {
        _cashedStateAnimations = new();

        foreach (var key in _attackRepository.GetSequencesKeys())
        {
            var stateName = $"{StatePrefix}{key}";
            _cashedStateAnimations[stateName] = GetAttackAnimationNameFromState(_character.Animator, stateName);
        }
    }

    private AnimationClip GetAttackAnimationNameFromState(Animator animator, string stateName, int layerIndex = 0)
    {
        var parameters = animator.parameters;
        foreach (var parameter in parameters)
        {
            if (parameter.name != stateName)
                continue;

            var clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);

            if (clipInfos.Length <= 0 || !clipInfos[0].clip)
                continue;

            return clipInfos[0].clip;
        }

        return default;
    }
}