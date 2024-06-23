using System;

public interface IState
{
    void Enter();
    Type Update();
}
