public class ChaseState : NavMeshState<States>
{
    public override States Type { get; } = States.Chase;

    public ChaseState(Character character) : base(character)
    {
    }

    public override States Update(float deltaTime)
    {
        var target = _characterModel.Target.Value;
        if (target == null || !IsInRange(target.position, _characterConfig.ChaseStopRange))
            return States.Return;

        if (IsInRange(target.position, _characterConfig.MeleAttackRange))
            return States.Attack;

        CalculateInput(target.position);
        return Type;
    }
}
