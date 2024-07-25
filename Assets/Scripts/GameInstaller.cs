using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Character playerPrefab;
    [SerializeField] private Character botPrefab;

    [SerializeField] private CharacterConfig playerConfig;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private Transform[] botSpawnPoints;

    [Header("UI")]
    [SerializeField] private UiJoyStick uiJoyStick;


    public override void InstallBindings()
    {
        var combatRepository = new CombatRepository(combatConfig);
        Container.Bind<ICombatRepository>().FromInstance(combatRepository).AsSingle();

        Container.Bind<GameBus>().AsSingle();
        Container.Bind<Camera>().FromInstance(cameraController.MainCamera).AsSingle();

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
        SetupCinemachine();
        SetTargets();
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
        var cameraTranform = cameraController.MainCamera.transform;
        character.Init(
            new PlayerInputStrategy(inputActionAsset, uiJoyStick),
            new PlayerRotateStrategy(cameraTranform),
            new PlayerMoveStrategy(cameraTranform),
            playerConfig);
        character.Transform.SetPositionAndRotation(playerSpawnPoint.position, playerSpawnPoint.rotation);
        character.NavMeshAgent.enabled = false;
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
            if (spawnPoint.gameObject.activeSelf == false)
                continue;

            var character = botFactory.Create(combatRepository, camera, gameBus);
            character.name = $"Bot_{gameBus.Bots.Count}";
            character.Init(
                new BotInputStrategy(),
                new BotRotateStrategy(),
                new BotMoveStrategy(),
                spawnPosition: spawnPoint.position);
            character.Transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            character.NavMeshAgent.enabled = false;
            gameBus.AddBot(character);
        }
    }

    private void SetupCinemachine()
    {
        var player = Container.Resolve<GameBus>().Player;
        cameraController.Init(player.Transform, player.CharacterModel.Target);
    }

    private void SetTargets()
    {
        var gameBus = Container.Resolve<GameBus>();
        gameBus.Player.SetTargets(gameBus.Bots.Select(b => b.Transform).ToList());
        gameBus.Bots.ForEach(b => b.SetTargets(new List<Transform> { gameBus.Player.Transform }));
    }
}