using System;
using DMZ.Events;

public class CharacterModel : IDisposable
{
    public readonly DMZState<float> MoveSpeed = new();
    public readonly DMZState<CombatPhase> AttackSequenceState = new(CombatPhase.Idle);
    public readonly DMZState<CombatProgressModel> AttackProgress = new();
    public DMZState<(int, int)> CurrentSequenceKey = new((-1, -1));

    public void Dispose()
    {
        MoveSpeed?.Dispose();
        AttackSequenceState?.Dispose();
        AttackProgress?.Dispose();
        CurrentSequenceKey?.Dispose();
    }
}