public interface IState<T>
{
    T Type { get; }
    void Enter();
    T Update();
}
