using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Attack3x3
{
    public class InputInstaller : MonoInstaller
    {
        [SerializeField] private InputActionAsset inputActionAsset;

        public override void InstallBindings()
        {
            Container.Bind<InputActionAsset>().FromInstance(inputActionAsset).AsSingle();
            Container.BindInterfacesTo<InputController>().AsSingle();
        }
    }
}