using System;
using UnityEngine;

public class IdleState : StateBase
{
    public IdleState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override Type Update()
    {
        if (CanSeePlayer())
            return typeof(ChaseState);

        return GetType();
    }

    private bool CanSeePlayer()
    {
        if (gameBus.Player == null)
            return false;

        return Vector3.Distance(gameBus.Player.Transform.position, character.Transform.position) <= character.SightRange;
    }
}
