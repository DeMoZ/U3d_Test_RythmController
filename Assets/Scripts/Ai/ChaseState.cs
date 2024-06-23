using System;
using UnityEngine;

public class ChaseState : StateBase
{

    public ChaseState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override Type Update()
    {
        if (InRangeWithPlayer(character.MeleAttackRange))
            return typeof(AttackState);

        if (!InRangeWithPlayer(character.StopChaseRange))
            return typeof(ReturnState);

        return GetType();
    }

    private bool InRangeWithPlayer(float distance) =>
        Vector3.Distance(gameBus.Player.Transform.position, character.Transform.position) <= distance;
}
