using Debug = DMZ.DebugSystem.DMZLogger;

public class AttackState : StateBase<States>
{
    private readonly AttackSubFSM _substateMachine;

    public override States Type { get; } = States.Attack;

    public AttackState(Character character, GameBus gameBus) : base(character, gameBus)
    {
        _substateMachine = new AttackSubFSM(character, gameBus);
    }

    public override void Enter()
    {
        _substateMachine.OnEnter();
    }

    public override void Exit()
    {
        _substateMachine.OnExit();
    }

    public override States Update(float deltaTime)
    {
        var substateType = _substateMachine.Update(deltaTime);

        if (substateType == AttackSubStates.Idle && !IsInAttackRange())
        {
            return States.Chase;
        }

        return Type;

        bool IsInAttackRange() => IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.MeleAttackRange);
    }
}
