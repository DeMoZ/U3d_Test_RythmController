using UnityEngine;

public class LineForward : AreaLineRenderer
{
    protected override Vector3[] DrawFigure()
    {
        var bodyStart = transform.forward * (_radius * 0.7f);
        var bodyForward = transform.forward * _radius;

        return new Vector3[] { bodyStart, bodyForward };
    }
}