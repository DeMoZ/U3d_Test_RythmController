//#define LOGGER_ON
using System;
using DMZ.Extensions;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class CharacterAnimator : IDisposable
{
    private readonly CharacterModel _characterModel;
    private int _baseLayerId;

    private readonly CombatLayerAnimator _combatLayerAnimator;
    private readonly CombatLayerAnimator _legsLayerAnimator;
    private readonly MoveAnimator _moveAnimator;

    public CharacterAnimator(CharacterModel characterModel, Animator animator, ICombatRepository combatRepository)
    {
        _characterModel = characterModel;

        _combatLayerAnimator = new CombatLayerAnimator(characterModel, animator, combatRepository);
        _legsLayerAnimator = new CombatLegsLayerAnimator(characterModel, animator, combatRepository);
        _characterModel.AttackSequenceState.Subscribe(OnCombatSequenceStateChanged);

        _moveAnimator = new MoveAnimator(characterModel, animator);

        _baseLayerId = animator.GetLayerIndex(AnimatorConstants.BaseLayer);
        animator.Play(AnimatorConstants.DefaultStateOnBaseLayer, _baseLayerId);
    }

    public void Dispose()
    {
        _characterModel.AttackSequenceState.Unsubscribe(OnCombatSequenceStateChanged);
    }

    private void OnCombatSequenceStateChanged(CombatPhase attackState)
    {
#if LOGGER_ON
        Debug.Log("OnAttackSequenceStateChanged".Yellow());
#endif
        switch (attackState)
        {
            case CombatPhase.None:
                break;
            case CombatPhase.Idle:
                break;
            case CombatPhase.Pre:
                _combatLayerAnimator.TriggerPreAttackAnimation();
                _legsLayerAnimator.TriggerPreAttackAnimation();
                break;
            case CombatPhase.Attack:
                _combatLayerAnimator.TriggerAttackAnimation();
                _legsLayerAnimator.TriggerAttackAnimation();
                break;
            case CombatPhase.After:
                _combatLayerAnimator.TriggerPostAttackAnimation();
                _legsLayerAnimator.TriggerPostAttackAnimation();
                break;
            case CombatPhase.Fail:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackState), attackState, null);
        }
    }
}
