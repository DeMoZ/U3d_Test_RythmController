public class ChaseState : NavMeshState
{
    public override BotStates Type { get; } = BotStates.Chase;

    public ChaseState(Character character, GameBus gameBus, InputModel inputModel) : base(character, gameBus, inputModel)
    {
    }

    public override BotStates Update()
    {
        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.meleAttackRange))
            return BotStates.Attack;

        if (!IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.chaseStopRange))
            return BotStates.Return;

        GetInput(_gameBus.Player.Transform.position);
        return Type;
    }
}
