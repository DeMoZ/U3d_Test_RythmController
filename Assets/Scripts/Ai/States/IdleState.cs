public class IdleState : StateBase<States>
{
    public override States Type { get; } = States.Idle;

    public IdleState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override States Update(float deltaTime)
    {
        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.ChaseRange))
            return States.Chase;

        return Type;
    }
}
