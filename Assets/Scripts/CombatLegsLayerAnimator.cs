using UnityEngine;

public class CombatLegsLayerAnimator : CombatLayerAnimator
{
    protected override string Layer => "LegsCombatLayer";
    protected override string PreAttackSpeed => "LegsPreSpeed";
    protected override string AttackSpeed => "LegsAttackSpeed";
    protected override string PostAttackSpeed => "LegsPostSpeed";

    public CombatLegsLayerAnimator(CombatModel combatModel, Animator animator, CombatRepository combatRepository)
        : base(combatModel, animator, combatRepository)
    {
        
    }
}