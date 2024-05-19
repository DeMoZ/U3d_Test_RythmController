using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputInstaller : MonoInstaller
{
    [SerializeField] private InputActionAsset inputActionAsset;
    
    public override void InstallBindings()
    {
        Container.Bind<InputActionAsset>().FromInstance(inputActionAsset).AsSingle();
        Container.BindInterfacesTo<InputController>().AsSingle();
    }
}