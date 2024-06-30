public class ChaseState : NavMeshState
{
    public override BotStates Type { get; } = BotStates.Chase;

    public ChaseState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update(float deltaTime)
    {
        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.MeleAttackRange))
            return BotStates.Attack;

        if (!IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.ChaseStopRange))
            return BotStates.Return;

        GetInput(_gameBus.Player.Transform.position);
        return Type;
    }
}
