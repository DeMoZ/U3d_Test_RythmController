using UnityEngine;

public class PathLine : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    public void Enable(bool enable)
    {
        gameObject.SetActive(enable);
    }

    public void Draw(Vector3[] positions)
    {
        lineRenderer.widthMultiplier = 0.03f;

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    public void DrawFigure()
    {
        //    var bodyStart = transform.forward * (_radius * 0.7f);
        //    var bodyForward = transform.forward * _radius;

        //    return new Vector3[] { bodyStart, bodyForward };
    }
}