public interface IState<T>
{
    T Type { get; }
    void Enter();
    T Update(float deltaTime);
    void Exit();
}
