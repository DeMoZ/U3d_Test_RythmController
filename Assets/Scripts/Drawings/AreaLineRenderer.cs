using System.Collections.Generic;
using UnityEngine;

public abstract class AreaLineRenderer : AreaDrawerBase
{
    [SerializeField] protected LineRenderer lineRenderer;

    public override void Show()
    {
        gameObject.SetActive(true);
        Draw();
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Draw()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.widthMultiplier = 0.05f;

        List<Vector3> positions = new();
        positions.AddRange(DrawFigure());
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    protected abstract Vector3[] DrawFigure();
}
