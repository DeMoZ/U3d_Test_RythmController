using DMZ.Events;

public class CombatModel
{
    public readonly DMZState<CombatState> AttackSequenceState = new(CombatState.Idle);
    public readonly DMZState<CombatProgressModel> AttackProgress = new();
    public DMZState<(int, int)> CurrentSequenceKey = new((-1, -1));
}