using System;
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
    private CharacterModel _characterModel;
    private ICombatRepository _combatRepository;
    private Camera _mainCamera;
    private GameBus _gameBus;
    private CharacterAnimator _characterAnimator;
    private CombatController _combatController;
    private InputModel _inputModel;

    public Transform Transform { get; private set; }
    public Vector3 SpawnPosition { get; private set; }
    public CharacterConfig CharacterConfig => characterConfig;
    public NavMeshAgent NavMeshAgent => navMeshAgent;

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

    public void Init(IInputStrategy inputStrategy, CharacterModel characterModel, CharacterConfig characterConfig = null)
    {
        Transform = transform;
        SpawnPosition = Transform.position;

        if (characterConfig != null)
            this.characterConfig = characterConfig;

        _inputStrategy = inputStrategy;
        _characterModel = characterModel;
        botBehaviourUI.Init(_mainCamera);

        _inputModel = new InputModel();
        _characterAnimator = new CharacterAnimator(_characterModel, animator, _combatRepository);
        _combatController = new CombatController(_inputModel, _characterModel, _combatRepository);
        moveController.Init(_inputModel, _characterModel, characterController, _mainCamera.transform, this.characterConfig);
        _inputStrategy.Init(_inputModel, this, _gameBus);
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
        _inputModel?.Dispose();
        _characterModel?.Dispose();
    }

    public class Factory : PlaceholderFactory<ICombatRepository, Camera, GameBus, Character>
    {
    }
}
