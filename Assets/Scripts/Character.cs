using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Character : MonoBehaviour
{
    [SerializeField] private BotBehaviourUI botBehaviourUI;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private CharacterConfig characterConfig;
    [SerializeField] private List<AreaDrawerBase> areaDrawers;
    [SerializeField] private PathLine pathLine;

    private IInputStrategy _inputStrategy;
    private IMoveStrategy _moveStrategy;

    private ICombatRepository _combatRepository;
    private Camera _mainCamera;
    private GameBus _gameBus;
    private CharacterAnimator _characterAnimator;
    private CombatController _combatController;

    public Transform Transform { get; private set; }
    public Vector3 SpawnPosition { get; private set; }
    public CharacterConfig CharacterConfig => characterConfig;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public CharacterModel CharacterModel { get; private set; }
    public InputModel InputModel { get; private set; }

    [Inject]
    public void Construct(
            ICombatRepository combatRepository,
            Camera mainCamera,
            GameBus gameBus)
    {
        _combatRepository = combatRepository;
        _mainCamera = mainCamera;
        _gameBus = gameBus;
    }

    public void Init(IInputStrategy inputStrategy, IMoveStrategy moveStrategy, CharacterConfig charConfig = null, Vector3? spawnPosition = null)
    {
        Transform = transform;

        if (spawnPosition != null)
            SpawnPosition = spawnPosition.Value;

        if (charConfig != null)
            characterConfig = charConfig;

        _inputStrategy = inputStrategy;
        _moveStrategy = moveStrategy;
        botBehaviourUI.Init(_mainCamera);

        CharacterModel = new CharacterModel();
        InputModel = new InputModel();

        _characterAnimator = new CharacterAnimator(CharacterModel, animator, _combatRepository);
        _combatController = new CombatController(InputModel, CharacterModel, _combatRepository);
        _moveStrategy.Init(InputModel, CharacterModel, characterController, characterConfig);
        _inputStrategy.Init(InputModel, this, _gameBus);

        DrawArea();
        CharacterModel.OnMovePath += pathLine.Draw;
        CharacterModel.OnMovePathEnable += pathLine.Enable;
    }

    private void Update()
    {
        _moveStrategy?.OnUpdate(Time.deltaTime);
    }

    private void DrawArea()
    {
        foreach (var areaDrawer in areaDrawers)
        {
            areaDrawer.Init(characterConfig.MeleAttackRange, characterConfig.AttackRotationAngle);
            areaDrawer.Show();
        }
    }

    /// <summary>
    /// The character is in attack phase CombatPhase.Attack, CombatPhase.PreAttack, CombatPhase.AfterAttack
    /// </summary>
    /// <returns></returns>
    public bool IsInAttackPhase()
    {
        return CharacterModel.AttackSequenceState.Value is CombatPhase.Attack or CombatPhase.Pre or CombatPhase.After;
    }

    internal void ShowLog(int index, string status)
    {
        botBehaviourUI.ShowLog(index, status);
    }

    private void OnDestroy()
    {
        _characterAnimator?.Dispose();
        _combatController?.Dispose();
        _inputStrategy?.Dispose();
        _moveStrategy?.Dispose();
        InputModel?.Dispose();

        CharacterModel.OnMovePath -= pathLine.Draw;
        CharacterModel.OnMovePathEnable -= pathLine.Enable;
        CharacterModel?.Dispose();
    }

    public class Factory : PlaceholderFactory<ICombatRepository, Camera, GameBus, Character>
    {
    }
}
