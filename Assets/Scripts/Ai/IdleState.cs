using System;
using UnityEngine;

public class IdleState : StateBase<BotStates>
{
    public override BotStates Type { get; } = BotStates.Idle;

    public IdleState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update()
    {
        if (CanSeePlayer())
            return BotStates.Chase;

        return Type;
    }

    private bool CanSeePlayer()
    {
        if (gameBus.Player == null)
            return false;

        return Vector3.Distance(gameBus.Player.Transform.position, character.Transform.position) <= character.SightRange;
    }
}
