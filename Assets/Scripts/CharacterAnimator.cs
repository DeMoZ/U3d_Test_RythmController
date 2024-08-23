#define LOGGER_ON
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
    private readonly int _baseLayerId;

    public CharacterAnimator(CharacterModel characterModel, Animator animator, ICombatRepository combatRepository)
    {
        _characterModel = characterModel;

        _combatLayerAnimator = new CombatLayerAnimator(characterModel, animator, combatRepository);
        _legsLayerAnimator = new CombatLegsLayerAnimator(characterModel, animator, combatRepository);
        _characterModel.CombatPhaseState.Subscribe(OnCombatPhaseStateChanged);
        _characterModel.BlockPhaseState.Subscribe(OnBlockPhaseStateChanged);

        _moveAnimator = new MoveAnimator(characterModel, animator);

        _baseLayerId = animator.GetLayerIndex(AnimatorConstants.BaseLayer);
        animator.Play(AnimatorConstants.DefaultStateOnBaseLayer, _baseLayerId);
    }

    public void Dispose()
    {
        _characterModel.CombatPhaseState.Unsubscribe(OnCombatPhaseStateChanged);
        _characterModel.BlockPhaseState.Unsubscribe(OnBlockPhaseStateChanged);

        _combatLayerAnimator.Dispose();
        _legsLayerAnimator.Dispose();
        _moveAnimator.Dispose();
    }

    private void OnCombatPhaseStateChanged(CombatPhase combatPhase)
    {
#if LOGGER_ON
        Debug.Log($"{nameof(OnCombatPhaseStateChanged)}".Yellow());
#endif
        switch (combatPhase)
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
                throw new ArgumentOutOfRangeException(nameof(combatPhase), combatPhase, null);
        }
    }

    private void _OnBlockPhaseStateChanged(BlockPhase blockPhase)
    {
#if LOGGER_ON
        Debug.Log($"{nameof(_OnBlockPhaseStateChanged)}".Yellow());
#endif
        switch (blockPhase)
        {
            case BlockPhase.None:
                break;
            case BlockPhase.Pre:
                _combatLayerAnimator.TriggerPreBlockAnimation();
                _legsLayerAnimator.TriggerPreBlockAnimation();
                break;
            case BlockPhase.Block:
                _combatLayerAnimator.TriggerBlockAnimation();
                _legsLayerAnimator.TriggerBlockAnimation();
                break;
            case BlockPhase.After:
                _combatLayerAnimator.TriggerPostBlockAnimation();
                _legsLayerAnimator.TriggerPostBlockAnimation();
                break;
            case BlockPhase.Fail:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(blockPhase), blockPhase, null);
        }
    }

    private void OnBlockPhaseStateChanged(BlockPhase phase, BlockNames names)
    {
#if LOGGER_ON
        Debug.LogError($"{nameof(OnBlockPhaseStateChanged)} but two params in method; name {names}".Yellow());
#endif
    }
}
