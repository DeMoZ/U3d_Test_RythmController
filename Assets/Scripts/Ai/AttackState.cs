public class AttackState : StateBase<BotStates>
{
    public override BotStates Type { get; } = BotStates.Attack;

    public AttackState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override BotStates Update()
    {
        if (!IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.meleAttackRange))
            return BotStates.Chase;

        return Type;
    }
}
