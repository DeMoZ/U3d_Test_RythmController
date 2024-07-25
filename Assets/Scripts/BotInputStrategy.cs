public class BotInputStrategy : IInputStrategy
{
    private Character _character;
    private FSMUpdateBase<States> _botBehaviour;

    public BotInputStrategy()
    {
    }

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _character = character;
        _botBehaviour = new BotFSM(_character);
    }

    public void Dispose()
    {
        _botBehaviour.Dispose();
    }

    public void OnUpdate(float deltaTime)
    {
        _botBehaviour.Update(deltaTime);
    }
}