using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private MoveController moveController;
    private IInputStrategy _inputStrategy;

    private CharacterModel _characterModel;
    private CharacterAnimator _characterAnimator;
    private CombatController _combatController;
    private InputModel _inputModel;

    public void Init(IInputStrategy inputStrategy,
        CharacterModel characterModel,
        CombatRepository combatRepository,
        Camera mainCamera)
    {
        _inputStrategy = inputStrategy;
        _characterModel = characterModel;

        _inputModel = new InputModel();
        _characterAnimator = new CharacterAnimator(_characterModel, animator, combatRepository);
        _combatController = new CombatController(_inputModel, _characterModel, combatRepository);
        moveController.Init(_inputModel, _characterModel, characterController, mainCamera.transform);

        _inputStrategy.Init(_inputModel);
    }

    private void OnDestroy()
    {
        _characterAnimator.Dispose();
        _combatController.Dispose();
        _inputStrategy.Dispose();
        _inputModel.Dispose();
    }
}