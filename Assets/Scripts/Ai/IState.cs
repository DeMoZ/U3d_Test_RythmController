using System.Threading;
using System.Threading.Tasks;

public interface IState<T>
{
    T Type { get; }
    Task EnterAsync(CancellationToken token);
    T Update(float deltaTime);
    Task ExitAsync(CancellationToken token);
}
