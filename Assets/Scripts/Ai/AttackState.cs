using UnityEngine;

public class AttackState : StateBase<BotStates>
{
    public override BotStates Type { get; } = BotStates.Attack;
    
    public AttackState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update()
    {
        if (!InRangeWithPlayer(character.MeleAttackRange))
            return BotStates.Chase;

        return Type;
    }

    private bool InRangeWithPlayer(float distance) =>
        Vector3.Distance(gameBus.Player.Transform.position, character.Transform.position) <= distance;
}
