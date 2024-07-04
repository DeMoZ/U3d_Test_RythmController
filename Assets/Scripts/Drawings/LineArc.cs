using UnityEngine;

public class LineArc : AreaLineRenderer
{
    protected override Vector3[] DrawFigure()
    {
        var result = new Vector3[3];
        var angle = -_angle;
        var x = Mathf.Sin(Mathf.Deg2Rad * angle) * _radius;
        var z = Mathf.Cos(Mathf.Deg2Rad * angle) * _radius;

        result[0] = new Vector3(x, 0, z);
        
        angle = 0;
        x = Mathf.Sin(Mathf.Deg2Rad * angle) * _radius;
        z = Mathf.Cos(Mathf.Deg2Rad * angle) * _radius;
        result[1] = new Vector3(x, 0, z);

        angle = _angle;
        x = Mathf.Sin(Mathf.Deg2Rad * angle) * _radius;
        z = Mathf.Cos(Mathf.Deg2Rad * angle) * _radius;
        result[2] = new Vector3(x, 0, z);

        return result;
    }
}