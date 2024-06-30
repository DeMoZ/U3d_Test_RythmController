public class ReturnState : NavMeshState
{
    private const float _returnToIdle = 0.2f;

    public override BotStates Type { get; } = BotStates.Return;

    public ReturnState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update(float deltaTime)
    {
        if (IsInRange(_character.SpawnPosition, _returnToIdle))
            return BotStates.Idle;

        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.ChaseRange))
            return BotStates.Chase;

        GetInput(_character.SpawnPosition);

        return Type;
    }
}