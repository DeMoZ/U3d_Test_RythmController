using System;
using System.Collections.Generic;
using Attack;
using UnityEngine;

namespace Attack3x3
{
    public interface ICharacterAnimator : IDisposable
    {
    }

    public class CharacterAnimator : ICharacterAnimator
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

        private static string IdleState = "Idle Walk Run Blend";
        private static string AttackLayer = "Base Layer";

        private static string StatePrefix = "Attack";

        private Dictionary<string, AnimInfo> _animationsCash;
        private int _attackLayerIndex;

        private readonly AttackPlayerData _attackPlayerData;
        private readonly Character _character;
        private readonly IAttackRepository _attackRepository;

        public CharacterAnimator(AttackPlayerData attackPlayerData, Character character,
            IAttackRepository attackRepository)
        {
            _attackPlayerData = attackPlayerData;
            _character = character;
            _attackRepository = attackRepository;
            _attackPlayerData.AttackSequenceState.Subscribe(OnAttackSequenceStateChanged);
            _attackLayerIndex = _character.Animator.GetLayerIndex(AttackLayer);
            CashStateAnimations();
        }

        public void Dispose()
        {
            _attackPlayerData.AttackSequenceState.Unsubscribe(OnAttackSequenceStateChanged);
        }

        private void OnAttackSequenceStateChanged(AttackState combatState)
        {
            switch (combatState)
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
                    throw new ArgumentOutOfRangeException(nameof(combatState), combatState, null);
            }
        }

        private void TriggerAttackAnimation()
        {
            var stateName = $"{StatePrefix}{_attackPlayerData.CurrentSequenceCode}";

            var clip = _animationsCash[stateName];
            if (clip == default)
                return;

            _character.Animator.Play(stateName: stateName, normalizedTime: 0f, layer: -1);

            var length = clip.Length;
            var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceCode);
            _character.Animator.speed =
                length / (time + time * 1.02f); // todo roman reset animator speed to 1 after animation
        }

        private void CashStateAnimations()
        {
            _animationsCash = new Dictionary<string, AnimInfo>();

            _attackRepository.GetSequencesKeys().ForEach(key =>
            {
                var stateName = $"{StatePrefix}{key}";
                CashAnimation(stateName);
            });

            _character.Animator.Play(IdleState);

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
            _character.Animator.Play(stateName);
            _character.Animator.Update(0);

            var clips = _character.Animator.GetCurrentAnimatorClipInfo(_attackLayerIndex);
            if (clips.Length == 0)
                return false;

            clipInfo = clips[0];
            return clipInfo.clip != default;
        }
    }
}