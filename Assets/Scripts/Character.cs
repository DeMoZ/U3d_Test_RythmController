using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private IInputStrategy _inputStrategy;

    private CombatModel _combatModel;
    private CharacterAnimator _characterAnimator;
    private CombatController _combatController;

    public void Init(IInputStrategy inputStrategy,
        CombatModel combatModel,
        CombatRepository combatRepository)
    {
        _inputStrategy = inputStrategy;
        _combatModel = combatModel;

        var inputModel = new InputModel();
        _characterAnimator = new CharacterAnimator(_combatModel, animator, combatRepository);
        _combatController = new CombatController(inputModel, combatModel, combatRepository);

        _inputStrategy.Init(inputModel);
    }

    private void OnDestroy()
    {
        _characterAnimator.Dispose();
        _combatController.Dispose();
        _inputStrategy.Dispose();
    }
}