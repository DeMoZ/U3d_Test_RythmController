using UnityEngine;
using Zenject;

namespace Attack3x3
{
    public class Attack3x3Installer : MonoInstaller
    {
        [SerializeField] private Attack3x3Config attackConfig;

        public override void InstallBindings()
        {
            var playerData = new Attack3x3PlayerData();
            Container.Bind<Attack3x3PlayerData>().FromInstance(playerData).AsSingle();

            Container.Bind<Attack3x3Config>().FromInstance(attackConfig).AsSingle();
            Container.Bind<Attack3x3Repository>().AsSingle();
            Container.Bind<Attack3x3Bus>().FromInstance(new Attack3x3Bus()).AsSingle();
            Container.BindInterfacesTo<Attack3x3Controller>().AsSingle();
        }
    }
}