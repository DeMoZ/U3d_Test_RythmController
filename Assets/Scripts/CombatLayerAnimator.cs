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

    private static string DefaultState = "Default";

    //Attack
    private static string StartAttackState = "Start"; // start attack state
    private static string AttackState = "Attack";
    private static string PostAttackState = "Post"; // post attack state

    private static readonly int AttackIndexHash = Animator.StringToHash(AnimatorConstants.AttackIndex);
    private static readonly int StartAttackHash = Animator.StringToHash(AnimatorConstants.StartAttackTrigger);
    private static readonly int AttackHash = Animator.StringToHash(AnimatorConstants.AttackTrigger);
    private static readonly int PostAttackHash = Animator.StringToHash(AnimatorConstants.PostAttackTrigger);
    // --- Attack
    
    // Block
    private static string StartBlockState = "Start"; // block state
    private static string BlockState = "Block";
    private static string PostBlockState = "Post"; // post block state

    private static readonly int BlockIndexHash = Animator.StringToHash(AnimatorConstants. BlockIndex);
    private static readonly int StartBlockHash = Animator.StringToHash(AnimatorConstants.StartBlockTrigger);
    private static readonly int BlockHash = Animator.StringToHash(AnimatorConstants.BlockTrigger);
    private static readonly int PostBlockHash = Animator.StringToHash(AnimatorConstants.PostBlockTrigger);
    // --- Block

    private readonly int _layerIndex;
    private Dictionary<string, AnimInfo> _animationsCash;

    protected virtual string Layer => AnimatorConstants.CombatLayer;
    protected virtual string StartActionSpeed => AnimatorConstants.StartActionSpeed;
    protected virtual string ActionSpeed => AnimatorConstants.ActionSpeed;
    protected virtual string PostActionSpeed => AnimatorConstants.PostActionSpeed;

    private static string TupleToString((int, int) tuple) => $"{(tuple.Item1 == 0 ? "" : tuple.Item1)}{tuple.Item2}";
    private static int TupleToInt((int, int) tuple) => tuple.Item1 * 10 + tuple.Item2;

    private readonly CharacterModel _characterModel;
    private readonly Animator _animator;
    private readonly ICombatRepository _combatRepository;

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
            var state = $"{AttackState}{keyStr}";
            CashAnimation($"{StartAttackState}{state}");
            CashAnimation(state);
            CashAnimation($"{PostAttackState}{state}");
        });
    }

    private void CashBlockAnimations()
    {
        // todo roman here should be enum length or config
        for (var i = 1; i < 5; i++)
        {
            var state = $"{BlockState}{i}";
            CashAnimation($"{StartBlockState}{state}");
            CashAnimation(state);
            CashAnimation($"{PostBlockState}{state}");
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

    public void TriggerStartAttackAnimation(AttackNames attackName)
    {
        var stateName = $"{StartAttackState}{AttackState}{TupleToString(_characterModel.CurrentSequenceKey.Value)}";
#if LOGGER_ON
        Debug.Log("TriggerStartAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPreAttackTime(_characterModel.CurrentSequenceKey.Value);
        _animator.SetFloat(StartActionSpeed, length / time);
        _animator.SetInteger(AttackIndexHash, TupleToInt(_characterModel.CurrentSequenceKey.Value));
        _animator.SetTrigger(StartAttackHash);
    }

    public void TriggerAttackAnimation(AttackNames attackName)
    {
        var stateName = $"{AttackState}{TupleToString(_characterModel.CurrentSequenceKey.Value)}";
#if LOGGER_ON
        Debug.Log("TriggerAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetAttackTime(_characterModel.CurrentSequenceKey.Value);
        _animator.SetFloat(ActionSpeed, length / time);
        _animator.SetTrigger(AttackHash);
    }

    public void TriggerPostAttackAnimation(AttackNames attackName)
    {
        var stateName =
            $"{PostAttackState}{AttackState}{TupleToString(_characterModel.CurrentSequenceKey.Value)}";
#if LOGGER_ON
        Debug.Log("TriggerPostAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPostAttackTime(_characterModel.CurrentSequenceKey.Value);
        _animator.SetFloat(PostActionSpeed, length / time);
        _animator.SetTrigger(PostAttackHash);
    }

    #endregion //Combat

    #region Block

    // todo roman now only for shield block animation and then for weapon block too
    public void TriggerStartBlockAnimation(BlockNames blockName)
    {
        var stateName = $"{StartBlockState}{blockName}";
#if LOGGER_ON
        Debug.Log("TriggerPreBlockAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPreBlockTime();
        _animator.SetFloat(StartActionSpeed, length / time);

        _animator.SetInteger(BlockIndexHash, (int)blockName);
        _animator.SetTrigger(StartBlockHash);
    }

    public void TriggerBlockAnimation(BlockNames blockName)
    {
        var stateName = $"{blockName}";
#if LOGGER_ON
        Debug.Log("TriggerBlockAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetBlockTime();
        _animator.SetFloat(ActionSpeed, length / time);
        _animator.SetTrigger(BlockHash);
    }

    public void TriggerPostBlockAnimation(BlockNames blockName)
    {
        var stateName = $"{PostBlockState}{blockName}";
#if LOGGER_ON
        Debug.Log("TriggerPostBlockAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
#endif
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetPostBlockTime();
        _animator.SetFloat(PostActionSpeed, length / time);
        _animator.SetTrigger(PostBlockHash);
    }

    #endregion //Block
}