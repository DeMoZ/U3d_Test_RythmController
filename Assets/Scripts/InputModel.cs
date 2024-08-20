using System;
using DMZ.Events;
using UnityEngine;

public class InputModel : IDisposable
{
    public readonly DMZState<Vector3> OnMove = new ();
    public readonly DMZState<bool> IsRunning = new (false);
    public Action<bool> OnAttack;
    public Action<bool, BlockNames> OnBlock;

    public void Dispose()
    {
        OnMove.Dispose();
        IsRunning.Dispose();
    }
}