using UnityEngine;
using Zenject;

public class Character3x3Installer : MonoInstaller
{
    [SerializeField] private Character characterPrefab;
    [SerializeField] private GameObject characterSpawnPoint;
    
    public override void InstallBindings()
    {
        var character = Instantiate(characterPrefab, characterSpawnPoint.transform.position, Quaternion.identity);
        
        Container.Bind<Character>().FromInstance(character);
        Container.BindInterfacesTo<Character3x3Animator>().AsSingle();
    }
}