using System;
using DMZ.Extensions;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class CharacterAnimator : IDisposable
{
    private readonly CombatModel _combatModel;

    private readonly CombatLayerAnimator _combatLayerAnimator;
    private readonly CombatLayerAnimator _legsLayerAnimator;

    public CharacterAnimator(CombatModel combatModel, Animator animator, CombatRepository combatRepository)
    {
        _combatModel = combatModel;

        _combatLayerAnimator = new CombatLayerAnimator(combatModel, animator, combatRepository);
        _legsLayerAnimator = new CombatLegsLayerAnimator(combatModel, animator, combatRepository);

        _combatModel.AttackSequenceState.Subscribe(OnAttackSequenceStateChanged);
    }

    public void Dispose()
    {
        _combatModel.AttackSequenceState.Unsubscribe(OnAttackSequenceStateChanged);
    }

    private void OnAttackSequenceStateChanged(CombatState attackState)
    {
        Debug.Log("OnAttackSequenceStateChanged".Yellow());

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