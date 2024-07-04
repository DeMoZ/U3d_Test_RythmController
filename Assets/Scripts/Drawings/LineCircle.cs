using UnityEngine;

public class LineCircle : AreaLineRenderer
{
    private const int SEGMENTS = 20;

    protected override Vector3[] DrawFigure()
    {
        var result = new Vector3[SEGMENTS + 1];

        float angle = 0;
        for (int i = 0; i < SEGMENTS + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * _radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * _radius;
            result[i] = new Vector3(x, 0, z);
            angle += 360f / SEGMENTS;
        }

        return result;
    }
}