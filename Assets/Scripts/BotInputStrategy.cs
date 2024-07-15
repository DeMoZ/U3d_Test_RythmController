using System;

public class BotInputStrategy : IInputStrategy
{
    private Character _character;
    private GameBus _gameBus;
    private FSMUpdateBase<States> _botBehaviour;

    public BotInputStrategy()
    {
    }

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _character = character;
        _gameBus = gameBus;

        _botBehaviour = new BotFSM(_character, _gameBus);
        //_botBehaviour.RunStateMachine();
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