using UnityEngine;

public interface IAreaDrawer
{
    void Init(float raduis, float anble);
    void Show();
    void Hide();
}

public abstract class AreaDrawerBase : MonoBehaviour, IAreaDrawer
{
    protected float _radius;
    protected float _angle;

    public void Init(float raduis, float anble)
    {
        _radius = raduis;
        _angle = anble;
    }

    public abstract void Hide();
    public abstract void Show();
}
