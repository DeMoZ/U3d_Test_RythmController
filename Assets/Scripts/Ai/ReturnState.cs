using UnityEngine;

public class ReturnState : StateBase<BotStates>
{
    public override BotStates Type { get; } = BotStates.Return;

    public ReturnState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update()
    {
        if (InRangeWith(0.1f))
            return BotStates.Idle;

        return Type;
    }

    private bool InRangeWith(float distance) =>
       Vector3.Distance(character.SpawnPosition, character.Transform.position) <= distance;
}