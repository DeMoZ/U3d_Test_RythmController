using System;
using DMZ.Events;
using UnityEngine;

public class CharacterModel : IDisposable
{
    public readonly DMZState<Vector3> MoveSpeed = new();
    public readonly DMZState<CombatPhase, AttackNames> CombatPhaseState = new(CombatPhase.Idle, AttackNames.Attack1);
    public readonly DMZState<BlockPhase, BlockNames> BlockPhaseState = new(BlockPhase.None, BlockNames.None);
    public readonly DMZState<CombatProgressModel> AttackProgress = new();
    public readonly DMZState<(int, int)> CurrentSequenceKey = new((-1, -1));
    public readonly DMZState<ITargetable> Target = new();

    public Action<Vector3[]> OnMovePath;
    public Action<bool> OnMovePathEnable;
    public States State;
    public bool IsRunning;
    public Transform Transform;

    /// <summary>
    /// The character is in attack phase CombatPhase.Attack, CombatPhase.PreAttack, CombatPhase.AfterAttack
    /// </summary>
    /// <returns></returns>
    public bool IsInAttackPhase => CombatPhaseState.Value is CombatPhase.Attack or CombatPhase.Pre or CombatPhase.After;

    public bool IsInHardAttack => CurrentSequenceKey.Value.Item1 == 1;

    public void Dispose()
    {
        MoveSpeed?.Dispose();
        CombatPhaseState?.Dispose();
        AttackProgress?.Dispose();
        CurrentSequenceKey?.Dispose();
    }
}