public class IdleState : StateBase<BotStates>
{
    public override BotStates Type { get; } = BotStates.Idle;

    public IdleState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update(float deltaTime)
    {
        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.ChaseRange))
            return BotStates.Chase;

        return Type;
    }
}
