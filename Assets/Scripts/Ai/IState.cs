using System.Threading;
using System.Threading.Tasks;

public interface IState<T>
{
    T Type { get; }
    Task EnterAsync(CancellationToken token);
    T Update(); // todo probably Task too
    Task ExitAsync(CancellationToken token);
}
