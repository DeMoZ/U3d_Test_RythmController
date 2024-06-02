using UnityEngine;
using Zenject;

namespace Attack3x3
{
    public class CharacterInstaller : MonoInstaller
    {
        [SerializeField] private Character characterPrefab;
        [SerializeField] private GameObject characterSpawnPoint;

        public override void InstallBindings()
        {
            var character = Instantiate(characterPrefab, characterSpawnPoint.transform.position, Quaternion.identity);

            Container.Bind<Character>().FromInstance(character);
            Container.BindInterfacesTo<CharacterAnimator>().AsSingle();
        }
    }
}