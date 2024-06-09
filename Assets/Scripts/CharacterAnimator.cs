//#define LOGGER_ON
using System;
using DMZ.Extensions;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class CharacterAnimator : IDisposable
{
    private readonly CharacterModel _characterModel;

    private readonly CombatLayerAnimator _combatLayerAnimator;
    private readonly CombatLayerAnimator _legsLayerAnimator;
    private readonly MoveAnimator _moveAnimator;

    public CharacterAnimator(CharacterModel characterModel, Animator animator, CombatRepository combatRepository)
    {
        _characterModel = characterModel;

        _combatLayerAnimator = new CombatLayerAnimator(characterModel, animator, combatRepository);
        _legsLayerAnimator = new CombatLegsLayerAnimator(characterModel, animator, combatRepository);
        _characterModel.AttackSequenceState.Subscribe(OnCombatSequenceStateChanged);

        _moveAnimator = new MoveAnimator(characterModel, animator);
    }

    public void Dispose()
    {
        _characterModel.AttackSequenceState.Unsubscribe(OnCombatSequenceStateChanged);
    }

    private void OnCombatSequenceStateChanged(CombatState attackState)
    {
#if LOGGER_ON
        Debug.Log("OnAttackSequenceStateChanged".Yellow());
#endif
        switch (attackState)
        {
            case CombatState.None:
                break;
            case CombatState.Idle:
                break;
            case CombatState.Pre:
                _combatLayerAnimator.TriggerPreAttackAnimation();
                _legsLayerAnimator.TriggerPreAttackAnimation();
                break;
            case CombatState.Attack:
                _combatLayerAnimator.TriggerAttackAnimation();
                _legsLayerAnimator.TriggerAttackAnimation();
                break;
            case CombatState.After:
                _combatLayerAnimator.TriggerPostAttackAnimation();
                _legsLayerAnimator.TriggerPostAttackAnimation();
                break;
            case CombatState.Fail:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackState), attackState, null);
        }
    }
}
