using System;

/// <summary>
/// subscribe to inputs and invoke model events
/// </summary>
public interface IInputStrategy : IDisposable
{
    void Init(InputModel inputModel,  Character character, GameBus gameBus);
}