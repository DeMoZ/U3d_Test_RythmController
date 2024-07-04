using UnityEngine;

public abstract class AreaMeshDrawerBase : AreaDrawerBase
{
    [SerializeField] protected MeshFilter meshFilter;
    [SerializeField] protected int circleSegments = 20;

    protected abstract Mesh DrawMesh();

    public override void Show()
    {
        meshFilter.mesh = DrawMesh();
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
