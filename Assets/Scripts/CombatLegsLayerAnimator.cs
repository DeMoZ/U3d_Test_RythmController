using UnityEngine;

public class CombatLegsLayerAnimator : CombatLayerAnimator
{
    protected override string Layer => "LegsCombatLayer";
    protected override string PreActionSpeed => "LegsPreActionSpeed";
    protected override string ActionSpeed => "LegsActionSpeed";
    protected override string PostActionSpeed => "LegsPostActionSpeed";

    public CombatLegsLayerAnimator(CharacterModel characterModel, Animator animator, ICombatRepository combatRepository)
        : base(characterModel, animator, combatRepository)
    {
        
    }
}