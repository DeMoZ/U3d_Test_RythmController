using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InstallerConstants
{
    public const string PlayerFactoryId = "PlayerFactory";
    public const string BotFactoryId = "BotFactory";
}

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


    public override void InstallBindings()
    {
        var combatRepository = new CombatRepository(combatConfig);
        Container.Bind<ICombatRepository>().FromInstance(combatRepository).AsSingle();

        Container.Bind<GameBus>().AsSingle();
        Container.Bind<Camera>().FromInstance(mainCamera).AsSingle();

        Container.BindFactory<ICombatRepository, Camera, GameBus, Character, Character.Factory>()
            .WithId(InstallerConstants.PlayerFactoryId)
            .FromComponentInNewPrefab(playerPrefab);

        Container.BindFactory<ICombatRepository, Camera, GameBus, Character, Character.Factory>()
            .WithId(InstallerConstants.BotFactoryId)
            .FromComponentInNewPrefab(botPrefab)
            .UnderTransformGroup("Bots");
    }

    public override void Start()
    {
        base.Start();

        SpawnPlayer();
        SpawnBots();
    }

    private void SpawnPlayer()
    {
        var gameBus = Container.Resolve<GameBus>();

        var playerFactory = Container.ResolveId<Character.Factory>(InstallerConstants.PlayerFactoryId);
        var character = playerFactory.Create(
            Container.Resolve<ICombatRepository>(),
            Container.Resolve<Camera>(),
            gameBus);

        character.name = $"Player";
        character.Init(new PlayerInputStrategy(inputActionAsset, uiJoyStick), new CharacterModel());
        character.Transform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);
        gameBus.SetPlayer(character);
        inputActionAsset.Enable();
    }

    private void SpawnBots()
    {
        var gameBus = Container.Resolve<GameBus>();
        var botFactory = Container.ResolveId<Character.Factory>(InstallerConstants.BotFactoryId);
        var combatRepository = Container.Resolve<ICombatRepository>();
        var camera = Container.Resolve<Camera>();

        foreach (var spawnPoint in botSpawnPoints)
        {
            var character = botFactory.Create(combatRepository, camera, gameBus);
            character.name = $"Bot_{gameBus.Bots.Count}";
            gameBus.AddBot(character);
            character.Init(new BotInputStrategy(), new CharacterModel());
            character.Transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }
    }
}