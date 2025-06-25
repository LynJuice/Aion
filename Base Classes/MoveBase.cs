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
        public int HPCost = 0; // Health Points
        public int MPCost = 0; // Mana Points
        public int APCost = 0; // Action Points

        public int MinimumDamage = 0; // the minimum ammount of damage dealt by this move if it hits
        public int MaximumDamage = 100; // the maximum ammount of damage dealt by this move if it hits
    }
}