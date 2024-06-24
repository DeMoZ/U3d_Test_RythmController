using System;
using UnityEngine;
using Zenject;

public class Character : MonoBehaviour
{
    [SerializeField] private BotBehaviourUI botBehaviourUI;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private MoveController moveController;

    #region TODO Move To Config Settings
    [SerializeField] private float sightRange = 10;
    [SerializeField] private float meleAttackRange = 2;
    [SerializeField] private float stopChaseRange = 12;

    #endregion

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
    public float SightRange => sightRange;
    public float MeleAttackRange => meleAttackRange;
    public float StopChaseRange => stopChaseRange;

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

    public void Init(IInputStrategy inputStrategy, CharacterModel characterModel)
    {
        Transform = transform;
        SpawnPosition = Transform.position;

        _inputStrategy = inputStrategy;
        _characterModel = characterModel;
        botBehaviourUI.Init(_mainCamera);

        _inputModel = new InputModel();
        _characterAnimator = new CharacterAnimator(_characterModel, animator, _combatRepository);
        _combatController = new CombatController(_inputModel, _characterModel, _combatRepository);
        moveController.Init(_inputModel, _characterModel, characterController, _mainCamera.transform);
        _inputStrategy.Init(_inputModel, this, _gameBus);
    }

    internal void SetStatus(BotStates state)
    {
        botBehaviourUI.SetStatus(state);
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
