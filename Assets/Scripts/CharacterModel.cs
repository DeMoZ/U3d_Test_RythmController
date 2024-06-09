using DMZ.Events;

public class CharacterModel
{
    public readonly DMZState<float> MoveSpeed = new();
    public readonly DMZState<CombatState> AttackSequenceState = new(CombatState.Idle);
    public readonly DMZState<CombatProgressModel> AttackProgress = new();
    public DMZState<(int, int)> CurrentSequenceKey = new((-1, -1));
}