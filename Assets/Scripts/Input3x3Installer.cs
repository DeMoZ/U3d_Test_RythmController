using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Input3x3Installer : MonoInstaller
{
    [SerializeField] private InputActionAsset inputActionAsset;
    
    public override void InstallBindings()
    {
        Container.Bind<InputActionAsset>().FromInstance(inputActionAsset).AsSingle();
        Container.BindInterfacesTo<Input3x3Controller>().AsSingle();
    }
}