using System;

public class BotInputStrategy : IInputStrategy
{
    private InputModel _inputModel;
    private Character _character;
    private GameBus _gameBus;
    private StateMachineBase<BotStates> _botBehaviour;

    public BotInputStrategy()
    {
    }

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _inputModel = inputModel;
        _character = character;
        _gameBus = gameBus;

        _botBehaviour = new BotBehaviour(_character, _gameBus, _inputModel, OnStateSchanged);
        _botBehaviour.RunStateMachine();

        // todo remove
        _inputModel.IsRunning.Value = false;
    }

    public void Dispose()
    {
        _botBehaviour.Dispose();
    }

    /// <summary>
    /// it is just for outside messages
    /// </summary>
    private void OnStateSchanged(BotStates state)
    {
        _character.ShowLog(0, state.ToString());

        switch (state)
        {
            case BotStates.Idle:
                break;
            case BotStates.Chase:
                break;
            case BotStates.Attack:
                break;
            case BotStates.Return:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}