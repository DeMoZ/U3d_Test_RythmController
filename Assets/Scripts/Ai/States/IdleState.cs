public class IdleState : StateBase<States>
{
    public override States Type { get; } = States.Idle;

    public IdleState(Character character) : base(character)
    {
    }

    public override States Update(float deltaTime)
    {
        var target = _characterModel.Target.Value;
        if (target != null && IsInRange(target.position, _characterConfig.ChaseRange))
            return States.Chase;

        return Type;
    }
}
