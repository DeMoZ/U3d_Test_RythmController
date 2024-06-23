using System;
using UnityEngine;

public class ReturnState : StateBase
{
    public ReturnState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override Type Update()
    {
        if (InRangeWith(0.1f))
            return typeof(IdleState);

        return GetType();
    }

    private bool InRangeWith(float distance) =>
       Vector3.Distance(character.SpawnPosition, character.Transform.position) <= distance;
}