using UnityEngine;

public class ChaseState : StateBase<BotStates>
{
    public override BotStates Type { get; } = BotStates.Chase;

    public ChaseState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update()
    {
        if (InRangeWithPlayer(character.MeleAttackRange))
            return BotStates.Attack;

        if (!InRangeWithPlayer(character.StopChaseRange))
            return BotStates.Return;

        return Type;
    }

    private bool InRangeWithPlayer(float distance) =>
        Vector3.Distance(gameBus.Player.Transform.position, character.Transform.position) <= distance;
}
