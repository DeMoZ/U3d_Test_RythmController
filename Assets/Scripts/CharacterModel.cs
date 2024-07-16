using System;
using DMZ.Events;
using UnityEngine;

public class CharacterModel : IDisposable
{
    public readonly DMZState<float> MoveSpeed = new();
    public readonly DMZState<CombatPhase> AttackSequenceState = new(CombatPhase.Idle);
    public readonly DMZState<CombatProgressModel> AttackProgress = new();
    public readonly DMZState<(int, int)> CurrentSequenceKey = new((-1, -1));
    public Action<Vector3[]> OnMovePath;
    public Action<bool> OnMovePathEnable;
    public States State;

    public Transform Target;

    /// <summary>
    /// The character is in attack phase CombatPhase.Attack, CombatPhase.PreAttack, CombatPhase.AfterAttack
    /// </summary>
    /// <returns></returns>
    public bool IsInAttackPhase => AttackSequenceState.Value is CombatPhase.Attack or CombatPhase.Pre or CombatPhase.After;

    public void Dispose()
    {
        MoveSpeed?.Dispose();
        AttackSequenceState?.Dispose();
        AttackProgress?.Dispose();
        CurrentSequenceKey?.Dispose();
    }
}