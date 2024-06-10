using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private CombatConfig combatConfig;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Character playerPrefab;
    [SerializeField] private Character botPrefab;

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform[] botSpawnPoints;
    
    [Header("UI")]
    [SerializeField] private UiJoyStick uiJoyStick;

    private CombatRepository _combatRepository;

    public override void InstallBindings()
    {
        _combatRepository = new CombatRepository(combatConfig);
        Container.Bind<CombatRepository>().FromInstance(_combatRepository);

        SpawnPlayer();
        SpawnBots();
    }

    private void SpawnPlayer()
    {
        inputActionAsset.Enable();
        var playerInputStrategy = new PlayerInputStrategy(inputActionAsset, uiJoyStick);
        var combatModel = new CharacterModel();
        var player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        player.Init(playerInputStrategy, combatModel, _combatRepository, mainCamera);
    }

    private void SpawnBots()
    {
        foreach (var spawnPoint in botSpawnPoints)
        {
            var botInputStrategy = new BotInputStrategy();
            var combatModel = new CharacterModel();
            var bot = Instantiate(botPrefab, spawnPoint.position, spawnPoint.rotation);
            bot.Init(botInputStrategy, combatModel, _combatRepository, mainCamera);
        }
    }
}