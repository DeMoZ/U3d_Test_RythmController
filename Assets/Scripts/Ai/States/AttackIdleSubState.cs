/// <summary>
/// In that state wait for some time between actions
/// </summary>
public class AttackIdleSubState : StateBase<AttackSubStates>
{
    public override AttackSubStates Type { get; } = AttackSubStates.Idle;

    private float _timer = 0f;

    public AttackIdleSubState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override void Enter()
    {
        base.Enter();

        _timer = GetRandomTime(0.01f, 0.4f);
    }

    public override AttackSubStates Update(float deltaTime)
    {
        _timer -= deltaTime;

        if (_timer <= 0)
            return AttackSubStates.Hit;

        return Type;
    }
}