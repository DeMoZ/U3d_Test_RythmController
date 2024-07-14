using Debug = DMZ.DebugSystem.DMZLogger;

public class AttackState : StateBase<States>
{
    private readonly InputModel _inputModel;
    private readonly CharacterModel _characterModel;
    private readonly AttackSubFSM _substateMachine;

    public override States Type { get; } = States.Attack;

    public AttackState(Character character, GameBus gameBus)
        : base(character, gameBus)
    {
        _inputModel = character.InputModel;
        _characterModel = character.CharacterModel;

        _substateMachine = new AttackSubFSM(character, gameBus, OnSubstateChanged);
    }

    private void OnSubstateChanged(AttackSubStates states)
    {
        Debug.Log($"{_character.gameObject.name} - Attack Sub State Changed: {states}");
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
