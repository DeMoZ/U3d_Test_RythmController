public class ReturnState : NavMeshState
{
    private const float _returnToIdle = 0.2f;

    public override States Type { get; } = States.Return;

    public ReturnState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override States Update(float deltaTime)
    {
        if (IsInRange(_character.SpawnPosition, _returnToIdle))
            return States.Idle;

        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.ChaseRange))
            return States.Chase;

        CalculateInput(_character.SpawnPosition);

        return Type;
    }
}