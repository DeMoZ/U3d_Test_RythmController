using Debug = DMZ.DebugSystem.DMZLogger;

public class AttackState : StateBase<States>
{
    private readonly AttackSubFSM _substateMachine;

    public override States Type { get; } = States.Attack;

    public AttackState(Character character) : base(character)
    {
        _substateMachine = new AttackSubFSM(character);
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

        if (substateType == AttackSubStates.Countdown && !IsInAttackRange())
        {
            return States.Chase;
        }

        return Type;

        bool IsInAttackRange()
        {
            if (_characterModel.Target.Value == null)
                return false;

            return IsInRange(_characterModel.Target.Value.Transform.position, _characterConfig.MeleAttackRange);
        }
    }
}
