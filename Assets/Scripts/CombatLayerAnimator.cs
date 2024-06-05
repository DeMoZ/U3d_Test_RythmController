using System.Collections.Generic;
using DMZ.Extensions;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class CombatLayerAnimator
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
    private const string CombatStatePrefix = "Attack"; // pre attack state
    private const string AttackSuffix = "A"; // attack state
    private const string PostAttackSuffix = "P"; // post attack state

    private const string AttackTrigger = "Attack"; // Attack trigger
    private const string PostAttackTrigger = "PostAttack";

    private readonly int _layerIndex;
    private Dictionary<string, AnimInfo> _animationsCash;

    protected virtual string Layer => "CombatLayer";
    protected virtual string PreAttackSpeed => "PreSpeed";
    protected virtual string AttackSpeed => "AttackSpeed";
    protected virtual string PostAttackSpeed => "PostSpeed";

    private static string TupleToString((int, int) tuple) => $"{tuple.Item1}{tuple.Item2}";

    private readonly CombatModel _combatModel;
    private readonly Animator _animator;
    private readonly CombatRepository _combatRepository;

    private static readonly int PostAttackTriggerCashed = Animator.StringToHash(PostAttackTrigger);
    private static readonly int AttackTriggerCashed = Animator.StringToHash(AttackTrigger);

    public CombatLayerAnimator(CombatModel combatModel, Animator animator, CombatRepository combatRepository)
    {
        _combatModel = combatModel;
        _animator = animator;
        _combatRepository = combatRepository;
        _layerIndex = _animator.GetLayerIndex(Layer);

        CashAnimations();
    }

    private void CashAnimations()
    {
        _animationsCash = new Dictionary<string, AnimInfo>();
        _combatRepository.GetSequencesKeys().ForEach(key =>
        {
            var keyStr = TupleToString(key);
            var stateName = $"{CombatStatePrefix}{keyStr}";
            CashAnimation(stateName);

            stateName = $"{CombatStatePrefix}{keyStr}{AttackSuffix}";
            CashAnimation(stateName);

            stateName = $"{CombatStatePrefix}{keyStr}{PostAttackSuffix}";
            CashAnimation(stateName);
        });

        _animator.Play(DefaultState, _layerIndex);
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
        _animator.Play(stateName, _layerIndex);
        _animator.Update(0);

        var clips = _animator.GetCurrentAnimatorClipInfo(_layerIndex);
        if (clips.Length == 0)
            return false;

        clipInfo = clips[0];
        return clipInfo.clip != default;
    }

    public void TriggerPreAttackAnimation()
    {
        var stateName = $"{CombatStatePrefix}{TupleToString(_combatModel.CurrentSequenceKey.Value)}";
        Debug.Log("TriggerPreAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetAttackTime(_combatModel.CurrentSequenceKey.Value);
        _animator.SetFloat(PreAttackSpeed, length / time);
        _animator.SetTrigger(stateName);
    }

    public void TriggerAttackAnimation()
    {
        var stateName = $"{CombatStatePrefix}{TupleToString(_combatModel.CurrentSequenceKey.Value)}{AttackSuffix}";
        Debug.Log("TriggerAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetAttackTime(_combatModel.CurrentSequenceKey.Value);
        _animator.SetFloat(AttackSpeed, length / time);
        _animator.SetTrigger(AttackTriggerCashed);
    }

    public void TriggerPostAttackAnimation()
    {
        var stateName = $"{CombatStatePrefix}{TupleToString(_combatModel.CurrentSequenceKey.Value)}{PostAttackSuffix}";
        Debug.Log("TriggerPostAttackAnimation".Yellow() + $" {_animationsCash[stateName].Name}");
        var length = _animationsCash[stateName].Length;
        var time = _combatRepository.GetAttackTime(_combatModel.CurrentSequenceKey.Value);
        _animator.SetFloat(PostAttackSpeed, length / time);
        _animator.SetTrigger(PostAttackTriggerCashed);
    }
}