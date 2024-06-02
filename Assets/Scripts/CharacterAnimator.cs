using System;
using System.Collections.Generic;
using DMZ.Extensions;
using UnityEngine;

public class CharacterAnimator
{
    private class AnimInfo
    {
        public readonly string Name;
        public readonly float Length;

        public AnimInfo(string name, float length)
        {
            Name = name;
            Length = length;
        }
    }

    private static string IdleState = "Default";
    private static string StateAttackPrefix = "Attack"; // pre attack state
    private static string AttackSuffix = "A"; // attack state
    private static string PostAttackSuffix = "P"; // post attack state
    private static string PreAttackSpeed = "PreSpeed";
    private static string AttackSpeed = "AttackSpeed";
    private static string PostAttackSpeed = "PostSpeed";

    private static string AttackTrigger = "Attack"; // Attack trigger
    private static string PostAttackTrigger = "PostAttack";
    private static string AttackLayer = "CombatLayer";
    private static float PreAttackTransitionTime = 0.15f;
    private static float PreAttackTransitionTimeSequence = 1f;

    private readonly CombatModel _combatModel;
    private readonly Animator _animator;

    private Dictionary<string, AnimInfo> _animationsCash;
    private int _attackLayerIndex;

   private CombatRepository _combatRepository;

    public CharacterAnimator(CombatModel combatModel, Animator animator, CombatRepository combatRepository)
    {
        _combatModel = combatModel;
        _animator = animator;
        _combatRepository = combatRepository;

        _attackLayerIndex = _animator.GetLayerIndex(AttackLayer);
        CashAnimations();
        _combatModel.AttackSequenceState.Subscribe(OnAttackSequenceStateChanged);
    }

    public void Dispose()
    {
        _combatModel.AttackSequenceState.Unsubscribe(OnAttackSequenceStateChanged);
    }

    private string TupleToString((int, int) tuple) => $"{tuple.Item1}{tuple.Item2}";

    private void CashAnimations()
    {
        _animationsCash = new Dictionary<string, AnimInfo>();
        _combatRepository.GetSequencesKeys().ForEach(key =>
        {
            var keyStr = TupleToString(key);
            var stateName = $"{StateAttackPrefix}{keyStr}";
            CashAnimation(stateName);

            stateName = $"{StateAttackPrefix}{keyStr}{AttackSuffix}";
            CashAnimation(stateName);

            stateName = $"{StateAttackPrefix}{keyStr}{PostAttackSuffix}";
            CashAnimation(stateName);
        });

        _animator.Play(IdleState);
        return;

        void CashAnimation(string stateName)
        {
            if (!TryGetAnimationInfo(stateName, out var clipInfo))
                return;

            Debug.Log($"clipName {clipInfo.clip.name}");
            _animationsCash[stateName] = new AnimInfo(clipInfo.clip.name, clipInfo.clip.length);
        }
    }

    private bool TryGetAnimationInfo(string stateName, out AnimatorClipInfo clipInfo)
    {
        clipInfo = default;
        Debug.Log($"CASH ANIMATIONS. SWITCH STATES. set state name {stateName}");
        _animator.Play(stateName);
        _animator.Update(0);

        var clips = _animator.GetCurrentAnimatorClipInfo(_attackLayerIndex);
        if (clips.Length == 0)
            return false;

        clipInfo = clips[0];
        return clipInfo.clip != default;
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
                TriggerPreAttackAnimation();
                break;
            case CombatState.Attack:
                TriggerAttackAnimation();
                break;
            case CombatState.After:
                TriggerPostAttackAnimation();
                break;
            case CombatState.Fail:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackState), attackState, null);
        }
    }

    private void TriggerPreAttackAnimation()
    {
        var stateName = $"{StateAttackPrefix}{TupleToString(_combatModel.CurrentSequenceKey.Value)}";
        Debug.Log("TriggerPreAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
        var length = _animationsCash[stateName].Length;
        var configTime = _combatRepository.GetAttackTime(_combatModel.CurrentSequenceKey.Value);
        var time = length > configTime ? length / configTime : 1;
        var transitionTime = _combatModel.CurrentSequenceKey.PreviousValue == (-1, -1)
            ? PreAttackTransitionTime
            : PreAttackTransitionTimeSequence;
        _animator.CrossFade(stateName, transitionTime);
        _animator.SetFloat(PreAttackSpeed, time);
        // _character.Animator.Play(stateName);
    }

    private void TriggerAttackAnimation()
    {
        var stateName =
            $"{StateAttackPrefix}{TupleToString(_combatModel.CurrentSequenceKey.Value)}{AttackSuffix}";
        Debug.Log("TriggerAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetAttackTime(_combatModel.CurrentSequenceKey.Value);
        _animator.SetFloat(AttackSpeed, length / time);
        _animator.SetTrigger(AttackTrigger);
    }

    private void TriggerPostAttackAnimation()
    {
        var stateName =
            $"{StateAttackPrefix}{TupleToString(_combatModel.CurrentSequenceKey.Value)}{PostAttackSuffix}";
        Debug.Log("TriggerPostAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
        var length = _animationsCash[stateName].Length;
        var configTime = _combatRepository.GetPostAttackTime(_combatModel.CurrentSequenceKey.Value);
        var time = length > configTime ? length / configTime : 1;
        _animator.SetFloat(PostAttackSpeed, time);
        _animator.SetTrigger(PostAttackTrigger);
    }
}