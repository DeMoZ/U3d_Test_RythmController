#define LOGGER_ON 
using System;
using System.Collections.Generic;
using DMZ.Extensions;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class CombatLayerAnimator : IDisposable
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

    private const string DefaultState = "Default";

    //Combat
    private const string CombatStatePrefix = "Attack"; // pre attack state
    private const string AttackSuffix = "A"; // attack state
    private const string PostAttackSuffix = "P"; // post attack state

    private const string AttackTrigger = "Attack"; // Attack trigger
    private const string PostAttackTrigger = "PostAttack";
    // --- Combat

    // Block
    private const string SBlockStatePrefix = "SBlock";
    private const string WBlockStatePrefix = "WBlock";
    private const string BlockSuffix = "A"; // block state
    private const string PostBlockSuffix = "P"; // post block state

    private const string BlockTrigger = "Block"; // Block trigger
    private const string PostBlockTrigger = "PostBlock";
    // --- Block

    private readonly int _layerIndex;
    private Dictionary<string, AnimInfo> _animationsCash;

    protected virtual string Layer => "CombatLayer";
    protected virtual string PreActionSpeed => "PreActionSpeed";
    protected virtual string ActionSpeed => "ActionSpeed";
    protected virtual string PostActionSpeed => "PostActionSpeed";

    // todo roman make decision by the Test of time enstead of speed
    protected virtual string PreActionTime => "PreActionTime";
    protected virtual string ActionTime => "ActionSpeedTime";
    protected virtual string PostActionTime => "PostActionTime";
    // --- Test

    private static string TupleToString((int, int) tuple) => $"{tuple.Item1}{tuple.Item2}";

    private readonly CharacterModel _characterModel;
    private readonly Animator _animator;
    private readonly ICombatRepository _combatRepository;

    private static readonly int PostAttackTriggerCashed = Animator.StringToHash(PostAttackTrigger);
    private static readonly int AttackTriggerCashed = Animator.StringToHash(AttackTrigger);

    private static readonly int PostBlockTriggerCashed = Animator.StringToHash(PostBlockTrigger);
    private static readonly int BlockTriggerCashed = Animator.StringToHash(BlockTrigger);

    public CombatLayerAnimator(CharacterModel characterModel, Animator animator, ICombatRepository combatRepository)
    {
        _characterModel = characterModel;
        _animator = animator;
        _combatRepository = combatRepository;
        _layerIndex = _animator.GetLayerIndex(Layer);

        CashAnimations();
    }

    public void Dispose()
    {
    }

    private void CashAnimations()
    {
        _animationsCash = new Dictionary<string, AnimInfo>();
        CashCombatAnimations();
        CashBlockAnimations();
        _animator.Play(DefaultState, _layerIndex);
    }

    private void CashAnimation(string stateName)
    {
        if (!TryGetAnimationInfo(stateName, out var clipInfo))
            return;

#if LOGGER_ON
        Debug.Log($"clipName {clipInfo.clip.name}");
#endif
        _animationsCash[stateName] = new AnimInfo(clipInfo.clip.name, clipInfo.clip.length);
    }

    private void CashCombatAnimations()
    {
        _combatRepository.GetSequencesKeys().ForEach(key =>
        {
            var keyStr = TupleToString(key);
            CashAnimation($"{CombatStatePrefix}{keyStr}");
            CashAnimation($"{CombatStatePrefix}{keyStr}{AttackSuffix}");
            CashAnimation($"{CombatStatePrefix}{keyStr}{PostAttackSuffix}");
        });
    }

    private void CashBlockAnimations()
    {
        CashAnimation($"{SBlockStatePrefix}00");
        CashAnimation($"{SBlockStatePrefix}00{BlockSuffix}");
        CashAnimation($"{SBlockStatePrefix}00{PostBlockSuffix}");

        for (var i = 0; i < 3; i++)
        {
            CashAnimation($"{WBlockStatePrefix}0{i}");
            CashAnimation($"{WBlockStatePrefix}0{i}{BlockSuffix}");
            CashAnimation($"{WBlockStatePrefix}0{i}{PostBlockSuffix}");
        }
    }

    private bool TryGetAnimationInfo(string stateName, out AnimatorClipInfo clipInfo)
    {
        clipInfo = default;
        _animator.Play(stateName, _layerIndex);
        _animator.Update(0);

        var clips = _animator.GetCurrentAnimatorClipInfo(_layerIndex);
        if (clips.Length == 0)
            return false;

        clipInfo = clips[0];
        return clipInfo.clip != default;
    }

    #region Combat

    public void TriggerPreAttackAnimation()
    {
        var stateName = $"{CombatStatePrefix}{TupleToString(_characterModel.CurrentSequenceKey.Value)}";
#if LOGGER_ON        
        Debug.Log("TriggerPreAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPreAttackTime(_characterModel.CurrentSequenceKey.Value);
        _animator.SetFloat(PreActionSpeed, length / time);
        _animator.SetTrigger(stateName);
    }

    public void TriggerAttackAnimation()
    {
        var stateName = $"{CombatStatePrefix}{TupleToString(_characterModel.CurrentSequenceKey.Value)}{AttackSuffix}";
#if LOGGER_ON        
        Debug.Log("TriggerAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif        
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetAttackTime(_characterModel.CurrentSequenceKey.Value);
        _animator.SetFloat(ActionSpeed, length / time);
        _animator.SetTrigger(AttackTriggerCashed);
    }

    public void TriggerPostAttackAnimation()
    {
        var stateName = $"{CombatStatePrefix}{TupleToString(_characterModel.CurrentSequenceKey.Value)}{PostAttackSuffix}";
#if LOGGER_ON        
        Debug.Log("TriggerPostAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif        
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPostAttackTime(_characterModel.CurrentSequenceKey.Value);
        _animator.SetFloat(PostActionSpeed, length / time);
        _animator.SetTrigger(PostAttackTriggerCashed);
    }

    #endregion //Combat

    #region Block

    // todo reoman now only for shield block animation and then for weapon block too
    public void TriggerPreBlockAnimation()
    {
        var stateName = $"{SBlockStatePrefix}00";
#if LOGGER_ON
        Debug.Log("TriggerPreBlockAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPreBlockTime();
        _animator.SetFloat(PreActionSpeed, length / time);
        _animator.SetTrigger(stateName);
    }

    public void TriggerBlockAnimation()
    {
        var stateName = $"{SBlockStatePrefix}00{BlockSuffix}";
#if LOGGER_ON        
        Debug.Log("TriggerBlockAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif        
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetBlockTime();
        _animator.SetFloat(ActionSpeed, length / time);
        _animator.SetTrigger(BlockTriggerCashed);
    }

    public void TriggerPostBlockAnimation()
    {
        var stateName = $"{SBlockStatePrefix}00{PostBlockSuffix}";
#if LOGGER_ON        
        Debug.Log("TriggerPostBlockAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif        
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPostBlockTime();
        _animator.SetFloat(PostActionSpeed, length / time);
        _animator.SetTrigger(PostBlockTriggerCashed);
    }

    #endregion //Block
}