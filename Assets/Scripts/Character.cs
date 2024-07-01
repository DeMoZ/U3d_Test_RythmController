using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class Character : MonoBehaviour
{
    [SerializeField] private BotBehaviourUI botBehaviourUI;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private MoveController moveController;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private CharacterConfig characterConfig;

    private IInputStrategy _inputStrategy;
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

    public void Init(IInputStrategy inputStrategy, CharacterConfig characterConfig = null)
    {
        Transform = transform;
        SpawnPosition = Transform.position;

        if (characterConfig != null)
            this.characterConfig = characterConfig;

        _inputStrategy = inputStrategy;
        botBehaviourUI.Init(_mainCamera);

        CharacterModel = new CharacterModel();
        InputModel = new InputModel();

        _characterAnimator = new CharacterAnimator(CharacterModel, animator, _combatRepository);
        _combatController = new CombatController(InputModel, CharacterModel, _combatRepository);
        moveController.Init(InputModel, CharacterModel, characterController, _mainCamera.transform, this.characterConfig);
        _inputStrategy.Init(InputModel, this, _gameBus);
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
        InputModel?.Dispose();
        CharacterModel?.Dispose();
    }

    public class Factory : PlaceholderFactory<ICombatRepository, Camera, GameBus, Character>
    {
    }
}
