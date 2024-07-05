using System;
using System.Collections.Generic;
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

    public void Dispose()
    {
        MoveSpeed?.Dispose();
        AttackSequenceState?.Dispose();
        AttackProgress?.Dispose();
        CurrentSequenceKey?.Dispose();
    }
}