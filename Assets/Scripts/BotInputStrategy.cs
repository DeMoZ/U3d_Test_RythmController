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
        RunInputSpamming(_cancellationTokenSource.Token);
    }

    private async void RunInputSpamming(CancellationToken token)
    {
        try
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await Task.Delay((int)(GetRandomTime(3, 6) * 1000), _cancellationTokenSource.Token);
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                _inputModel.OnAttackTouchStarted?.Invoke();

                await Task.Delay((int)(GetRandomTime(0.01f, 3f) * 1000), _cancellationTokenSource.Token);
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                _inputModel.OnAttackTouchEnded?.Invoke();
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

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}