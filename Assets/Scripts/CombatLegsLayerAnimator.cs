using UnityEngine;

public class CombatLegsLayerAnimator : CombatLayerAnimator
{
    protected override string Layer => AnimatorConstants.LegsCombatLayer;
    protected override string PreActionSpeed => AnimatorConstants.LegsPreActionSpeed;
    protected override string ActionSpeed => AnimatorConstants.LegsActionSpeed;
    protected override string PostActionSpeed => AnimatorConstants.LegsPostActionSpeed;

    public CombatLegsLayerAnimator(CharacterModel characterModel, Animator animator, ICombatRepository combatRepository)
        : base(characterModel, animator, combatRepository)
    {
        
    }
}