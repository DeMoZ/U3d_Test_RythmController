public class IdleState : StateBase<BotStates>
{
    public override BotStates Type { get; } = BotStates.Idle;

    public IdleState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update()
    {
        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.chaseRange))
            return BotStates.Chase;

        return Type;
    }
}
