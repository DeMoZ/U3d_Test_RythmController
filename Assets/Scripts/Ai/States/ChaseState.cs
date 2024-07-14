public class ChaseState : NavMeshState
{
    public override States Type { get; } = States.Chase;

    public ChaseState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override States Update(float deltaTime)
    {
        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.MeleAttackRange))
            return States.Attack;

        if (!IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.ChaseStopRange))
            return States.Return;

        CalculateInput(_gameBus.Player.Transform.position);
        return Type;
    }
}
