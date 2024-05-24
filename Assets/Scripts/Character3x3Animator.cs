using System;
using System.Collections.Generic;
using DMZ.Extensions;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class Character3x3Animator : ICharacterAnimator
{
    private static string IdleState = "Idle Walk Run Blend";
    private static string AttackParameter = "Attack";
    private static string StateAttackPrefix = "Attack"; // pre attack state
    private static string AttackSuffix = "A"; // attack state
    private static string PostAttackSuffix = "P"; // post attack state
    private static string PreAttackSpeed = "PreSpeed";
    private static string AttackSpeed = "AttackSpeed";
    private static string PostAttackSpeed = "PostSpeed";

    private static string AttackTrigger = "Attack"; // Attack trigger
    private static string PostAttackTrigger = "PostAttack";

    private static int AttackLayer = 0;

    private readonly Attack3x3PlayerData _attackPlayerData;
    private readonly Character _character;
    private readonly Attack3x3Repository _attackRepository;

    private Dictionary<string, AnimatorClipInfo> _cashedAnimationsInfos;
    private Dictionary<string, (string, float)> _cashedAnimationsDatas;

    public Character3x3Animator(Attack3x3PlayerData attackPlayerData, Character character,
        Attack3x3Repository attackRepository)
    {
        _attackPlayerData = attackPlayerData;
        _character = character;
        _attackRepository = attackRepository;
        _attackPlayerData.AttackSequenceState.Subscribe(OnAttackSequenceStateChanged);
        CashAnimations();
    }

    public void Dispose()
    {
        _attackPlayerData.AttackSequenceState.Unsubscribe(OnAttackSequenceStateChanged);
    }

    private string TupleToString((int, int) tuple) => $"{tuple.Item1}{tuple.Item2}";

    private void CashAnimations()
    {
        _cashedAnimationsInfos = new Dictionary<string, AnimatorClipInfo>();
        _cashedAnimationsDatas = new Dictionary<string, (string, float)>();

        _attackRepository.GetSequencesKeys().ForEach(key =>
        {
            var keyStr = TupleToString(key);
            var stateName = $"{StateAttackPrefix}{keyStr}";
            CashAnimation(stateName);

            stateName = $"{StateAttackPrefix}{keyStr}{AttackSuffix}";
            CashAnimation(stateName);

            stateName = $"{StateAttackPrefix}{keyStr}{PostAttackSuffix}";
            CashAnimation(stateName);
        });

        _character.Animator.Play(IdleState);
        return;

        void CashAnimation(string stateName)
        {
            if (!TryGetAnimationInfo(stateName, out var clipInfo))
                return;

            Debug.Log($"clipname {clipInfo.clip.name}");
            _cashedAnimationsInfos[stateName] = clipInfo;
            _cashedAnimationsDatas[stateName] = (clipInfo.clip.name, clipInfo.clip.length);
        }
    }

    private bool TryGetAnimationInfo(string stateName, out AnimatorClipInfo clipInfo)
    {
        clipInfo = default;
        Debug.Log($"CASH ANIMATIONS. SWITCH STATES. set state name {stateName}");
        _character.Animator.Play(stateName);
        _character.Animator.Update(0);

        var clips = _character.Animator.GetCurrentAnimatorClipInfo(AttackLayer);
        if (clips.Length == 0)
            return false;

        clipInfo = clips[0];
        return clipInfo.clip != default;
    }

    private void OnAttackSequenceStateChanged(Attack3x3State attackState)
    {
        Debug.Log("OnAttackSequenceStateChanged".Yellow());

        switch (attackState)
        {
            case Attack3x3State.None:
                break;
            case Attack3x3State.Idle:
                break;
            case Attack3x3State.Pre:
                TriggerPreAttackAnimation();
                break;
            case Attack3x3State.Attack:
                TriggerAttackAnimation();
                break;
            case Attack3x3State.After:
                TriggerPostAttackAnimation();
                break;
            case Attack3x3State.Fail:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(attackState), attackState, null);
        }
    }

    private void TriggerPreAttackAnimation()
    {
        var stateName = $"{StateAttackPrefix}{TupleToString(_attackPlayerData.CurrentSequenceKey)}";
        Debug.Log("TriggerPreAttackAnimation".Yellow() + $" {_cashedAnimationsDatas[stateName].Item1}");
        var length = _cashedAnimationsDatas[stateName].Item2;
        var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceKey);
        _character.Animator.SetFloat(AttackSpeed, length / time);
        _character.Animator.Play(stateName);
    }

    private void TriggerAttackAnimation()
    {
        var stateName = $"{StateAttackPrefix}{TupleToString(_attackPlayerData.CurrentSequenceKey)}{AttackSuffix}";
        Debug.Log("TriggerAttackAnimation".Yellow() + $" {_cashedAnimationsDatas[stateName].Item1}");
        var length = _cashedAnimationsDatas[stateName].Item2;
        var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceKey);
        _character.Animator.SetFloat(AttackSpeed, length / time);
        _character.Animator.SetTrigger(AttackTrigger);
    }

    private void TriggerPostAttackAnimation()
    {
        var stateName = $"{StateAttackPrefix}{TupleToString(_attackPlayerData.CurrentSequenceKey)}{PostAttackSuffix}";
        Debug.Log("TriggerPostAttackAnimation".Yellow() + $" {_cashedAnimationsDatas[stateName].Item1}");
        var length = _cashedAnimationsDatas[stateName].Item2;
        var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceKey);
        _character.Animator.SetFloat(PostAttackSpeed, length / time);
        _character.Animator.SetTrigger(PostAttackTrigger);
    }
}