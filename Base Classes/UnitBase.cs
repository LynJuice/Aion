using UnityEngine;

namespace Aion.Bases
{
    public class UnitBase : MonoBehaviour
    {
        // Summary:
        // This is the base class for all units. It contains the unit's stats, current health, and current mana.
        [SerializeField][Tooltip("Maximum Health of the unit")] int MaxHealth = 100;
        [SerializeField][Tooltip("Maximum Mana of the unit")] int MaxMana = 100;
        [SerializeField][Tooltip("Points given per turn")] int PointsPerTurn = 1;
        [SerializeField][Tooltip("The Current Aion")] AionBase Aion;
        [Space]
        [SerializeField][Tooltip("Current Health of the unit")] int CurrentHealth;
        [SerializeField][Tooltip("Current Mana of the unit")] int CurrentMana;

        public UnitBase()
        {
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
        }
    }
}
