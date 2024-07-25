public class ReturnState : NavMeshState<States>
{
    // todo roman move in config
    private const float RETURN_TO_IDLE_DISTANCE = 0.2f; // distance to point

    public override States Type { get; } = States.Return;

    public ReturnState(Character character) : base(character)
    {
    }

    public override States Update(float deltaTime)
    {
        if (IsInRange(_character.SpawnPosition, RETURN_TO_IDLE_DISTANCE))
            return States.Idle;

        var target = _characterModel.Target.Value;
        if (target != null && IsInRange(target.position, _characterConfig.ChaseStopRange))
            return States.Chase;

        CalculateInput(_character.SpawnPosition);

        return Type;
    }
}