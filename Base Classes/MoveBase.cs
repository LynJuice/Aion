using UnityEngine;

namespace Aion.Bases
{
    using Aion.Enumeration;
    [CreateAssetMenu(fileName = "Move", menuName = "Aion/Move", order = 1)]
    public class MoveBase : ScriptableObject
    {
        public Elements Type = Elements.Slash;
        public string Name = "New Move";
        public string Description = "This is a new move.";
        [Tooltip("The Amount of HP to take away from the user")] public int HPCost = 0; // Health Points
        [Tooltip("The Amount of MP to take away from the user")] public int MPCost = 0; // Mana Points
        [Tooltip("The Amount of AP to take away from the user")] public int APCost = 0; // Action Points
        [Tooltip("Applied buff/debuff/effect from this moves hit")] public EffectBase AppliedEffect; // The effect that can be applied when this move hits

        [Tooltip("Minimum damage caused from this move")] public int MinimumDamage = 0; // the minimum ammount of damage dealt by this move if it hits
        [Tooltip("Maximum damage caused from this move")] public int MaximumDamage = 100; // the maximum ammount of damage dealt by this move if it hits

        [Tooltip("Does this move require ultimate counter to be at 100?")] public bool IsUltimate = false;

        [Header("Passive Only!")]
        [Tooltip("This allows for healing moves")] public int HealingAmount = 0; // The amount of HP to heal when this move is used as a passive
    }
}