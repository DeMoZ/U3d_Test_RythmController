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

        _botBehaviour = new BotFSM(_character, _gameBus, OnStateSchanged);
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

    /// <summary>
    /// it is just for outside messages
    /// </summary>
    private void OnStateSchanged(States state)
    {
        _character.ShowLog(0, state.ToString());

        switch (state)
        {
            case States.Idle:
                break;
            case States.Chase:
                break;
            case States.Attack:
                break;
            case States.Return:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}