using UnityEngine;

namespace Aion.Bases
{
    public abstract class EffectBase : MonoBehaviour
    {
        public virtual string Name { get; set; } = "New Effect";
        public virtual string Description { get; set; } = "This is a new effect.";
        public virtual int Duration { get; set; } = 0; // Duration in turns
        [Range(-3, 3)] public int AttackModifier { get; set; } = 0; 
        [Range(-3, 3)] public int DefenseModifier { get; set; } = 0; 
        [Range(-3, 3)] public int AgilityModifier { get; set; } = 0; 

        public int CurrentDuration { get; set; } = 0; // Current duration in turns, used to track how many turns the effect has been active

        public virtual void EveryTurn(UnitBase unit)
        {
            CurrentDuration--;
        }
        public virtual void OnApply()
        {
            // This method can be overridden in derived classes to implement effects that occur when the effect is applied
        }
        public virtual void OnRemove()
        {
            // This method can be overridden in derived classes to implement effects that occur when the effect is removed
        }
    }
}
