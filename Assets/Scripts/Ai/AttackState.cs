using System;
using UnityEngine;

public class AttackState : StateBase
{
    public AttackState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override Type Update()
    {
        if (!InRangeWithPlayer(character.MeleAttackRange))
            return typeof(ChaseState);

        return GetType();
    }

    private bool InRangeWithPlayer(float distance) =>
        Vector3.Distance(gameBus.Player.Transform.position, character.Transform.position) <= distance;
}
