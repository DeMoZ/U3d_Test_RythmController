using DMZ.Events;

public class Attack3x3PlayerData : Attack.IAttackPlayerData
{
    public DMZState<Attack3x3State> AttackSequenceState = new (Attack3x3State.Idle);
    public (int, int) CurrentSequenceKey;
}