using System;
using System.Threading;
using System.Threading.Tasks;

public class BotInputStrategy : IInputStrategy
{
    private InputModel _inputModel;

    private CancellationTokenSource _cancellationTokenSource;

    public void Init(InputModel inputModel)
    {
        _inputModel = inputModel;
        _cancellationTokenSource = new CancellationTokenSource();
        RunCombatSpamming(_cancellationTokenSource.Token);
        RunRandomMoving(_cancellationTokenSource.Token);
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

    private async void RunRandomMoving(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1, _cancellationTokenSource.Token);
            }
        }
        catch (TaskCanceledException)
        {
        }
    }
    
    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}