using System;
using System.Threading;
using System.Threading.Tasks;
using Debug = DMZ.DebugSystem.DMZLogger;

public class BotInputStrategy : IInputStrategy
{
    private InputModel _inputModel;
    private Character _character;
    private GameBus _gameBus;
    private CancellationTokenSource _cancellationTokenSource;
    private StateMachineBase _botBehaviour;

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _inputModel = inputModel;
        _character = character;
        _gameBus = gameBus;

        _cancellationTokenSource = new CancellationTokenSource();
        _botBehaviour = new BotBehaviour(_character, _gameBus, OnStateSchanged);
        _botBehaviour.RunStateMachine();

        RunCombatSpamming(_cancellationTokenSource.Token);
        // DELETE AND METHOD TOO RunRandomMoving(_cancellationTokenSource.Token);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _botBehaviour.Dispose();
    }

    private void OnStateSchanged(Type type)
    {
        //Debug.Log($"{_character.name} state {type}");
        _character.SetStatus(type);
    }
    private async void RunCombatSpamming(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay((int)(GetRandomTime(3, 6) * 1000), token);
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                _inputModel.OnAttack?.Invoke(true);

                await Task.Delay((int)(GetRandomTime(0.01f, 3f) * 1000), token);
                if (token.IsCancellationRequested)
                    return;

                _inputModel.OnAttack?.Invoke(false);
            }
        }
        catch (TaskCanceledException)
        {
        }

        double GetRandomTime(float min, float max)
        {
            return new Random().NextDouble() * (max - min) + min;
        }
    }
}