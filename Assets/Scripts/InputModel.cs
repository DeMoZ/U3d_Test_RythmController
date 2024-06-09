using System;
using DMZ.Events;
using UnityEngine;

public class InputModel : IDisposable
{
    public readonly DMZState<Vector3> OnMove = new ();
    public Action<bool> OnAttack;

    public void Dispose()
    {
        OnMove?.Dispose();
    }
}