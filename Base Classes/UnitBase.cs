using UnityEngine;
using System.Collections.Generic;

namespace Aion.Bases
{
    using Aion.Enumeration;

    public class UnitBase : MonoBehaviour
    {
        // Summary:
        // This is the base class for all units. It contains the unit's stats, current health, and current mana.
        [Header("Units required data")]
        [SerializeField][Tooltip("Maximum Health of the unit")] int MaxHealth = 100;
        [SerializeField][Tooltip("Maximum Mana of the unit")] int MaxMana = 100;
        [SerializeField][Tooltip("Points given per turn")] int PointsPerTurn = 1;
        [SerializeField][Tooltip("The Current Aion")] AionBase Aion;
        [SerializeField][Tooltip("Affects physical attack damage dealt")][Range(1, 100)] int Strength = 10;
        [SerializeField][Tooltip("Affects magical attack damage dealt")][Range(1, 100)] int Magic = 10;
        [SerializeField][Tooltip("Affects both physical and magical damage taken")][Range(1, 100)] int Endurance = 10;
        [SerializeField][Tooltip("Affects accuracy and evasion and turn order")][Range(1, 100)] int Agility = 10;
        [SerializeField][Tooltip("Affects chances of dealing or taking critical damage")][Range(1, 100)] int Luck = 10;
        [Space]
        [SerializeField][Tooltip("Current Health of the unit")] int CurrentHealth;
        [SerializeField][Tooltip("Current Mana of the unit")] int CurrentMana;
        [SerializeField][Tooltip("List of currenly applied effects")] List<EffectBase> AppliedEffects = new List<EffectBase>();
        [SerializeField][Tooltip("Affects Attack (Physical and Magical) Runtime only, can be increased or decresed (-3,3) with buff and debuff effects")] int AttackBuff;
        [SerializeField][Tooltip("Affects Defense (Physical and Magical) Runtime only, can be increased or decresed (-3,3) with buff and debuff effects")] int DefenseBuff;
        [SerializeField][Tooltip("Affects Critical hit chance and evasion Runtime only, can be increased or decresed (-3,3) with buff and debuff effects")] int AgilityBuff;

        void Awake()
        {
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
            if (Aion == null)
                Debug.LogError("Aion is not set for " + gameObject.name + ". Please assign an AionBase to this unit.");
        }

        #region Functions
        #region Add Remove HP/MP
        public int AddHP(int HealAmount)
        {
            int oldHealth = CurrentHealth;
            CurrentHealth += HealAmount;
            if (CurrentHealth > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }

            int healedAmount = CurrentHealth - oldHealth;
            return healedAmount;
        }
        public int AddMP(int MPAmount)
        {
            int oldMana = CurrentMana;
            CurrentMana += MPAmount;
            if (CurrentMana > MaxMana)
            {
                CurrentMana = MaxMana;
            }
            int addedMana = CurrentMana - oldMana;
            return addedMana;
        }
        public bool RemoveMP(int MPAmount)
        {
            if (!CanUseMP(MPAmount))
                return false;
            CurrentMana -= MPAmount;
            return true;
        }
        public bool RemoveHP(int HPAmount)
        {
            if (!CanUseHP(HPAmount))
                return false;
            CurrentHealth -= HPAmount;
            return true;
        }
        #endregion
        #region Checks
        public bool CanUseMP(int MPAmount)
        {
            return CurrentMana >= MPAmount;
        }
        public bool CanUseHP(int HPAmount)
        {
            // no equals because we dont want to let the unit use a move that would kill it
            return CurrentHealth > HPAmount;
        }
        public bool IsDead => CurrentHealth <= 0;
        bool isCriticalHit(bool isPhysical)
        {
            if (!isPhysical) return false; // remove this line if magical attacks get crits too

            int roll = Random.Range(1, 101);

            // Apply Luck curve
            float baseCurve = Mathf.Pow(Luck / 100f, Settings.CriticalHitCurveExponent);

            // Each point in AgilityBuff adds/subtracts 5% of baseCurve (customize as needed in settings)
            float buffModifier = 1f + (AgilityBuff * Settings.AgilityBuffMultiplier);
            
            float finalCurve = Mathf.Clamp01(baseCurve * buffModifier);

            return roll <= finalCurve * 100f;
        }
        bool DidEvade(UnitBase attacker)
        {
            float attackerAccuracy = attacker.Agility * Settings.AccuracyAgilityMultiplier
                                   + attacker.Luck * Settings.AccuracyLuckMultiplier;

            float buffedAgility = Agility * (1f + AgilityBuff * Settings.AgilityBuffMultiplier);

            float defenderEvasion = buffedAgility * Settings.EvasionAgilityMultiplier
                                   + Luck * Settings.EvasionLuckMultiplier;

            float evadeChance = Mathf.Clamp(100f - attackerAccuracy + defenderEvasion,
                                            Settings.EvasionClampMin,
                                            Settings.EvasionClampMax);

            float roll = Random.Range(0f, 100f);

            return roll <= evadeChance;
        }
        #endregion               
        Affinity GetAffinity(Elements element)
        {
            switch (element)
            {
                case Elements.Gun:
                    return Aion.Gun;
                case Elements.Slash:
                    return Aion.Slash;
                case Elements.Bash:
                    return Aion.Bash;
                case Elements.Fire:
                    return Aion.Fire;
                case Elements.Ice:
                    return Aion.Ice;
                case Elements.Electric:
                    return Aion.Electric;
                case Elements.Wind:
                    return Aion.Wind;
                case Elements.Light:
                    return Aion.Light;
                case Elements.Dark:
                    return Aion.Dark;
                default:
                    {
                        Debug.LogWarning("Unknown element: " + element);
                        return Affinity.Neutral;
                    }
            }
        }
        public bool UseMove(MoveBase move, UnitBase selectedUnit)
        {
            // Checks to see if the unit can use this move (Failsafe)
            if (!CanUseMP(move.MPCost))
                return false;
            if (!CanUseHP(move.HPCost))
                return false;
            if (AionManager.Instance.ActionPoints < move.APCost)
                return false;

            foreach (EffectBase effect in AppliedEffects)
            {
                effect.EveryTurn(this);
                if (effect.CurrentDuration <= 0)
                {
                    effect.OnRemove(); // Call the effect's OnRemove method
                    OnBuffEnded(effect); // Call the buff ended animation
                }
            }

            if (move.Type != Elements.Passive)
            {
                UseOffensiveMove(move, selectedUnit);
                return true;
            }
            else
                return UseDefensiveMove(move, selectedUnit);
        }
        bool UseDefensiveMove(MoveBase Move, UnitBase selectedUnit)
        {
            // Todo In the future, add defensive moves that can heal or buff the unit
            return true; 
        }

        int CalculateDamage(MoveBase Move, UnitBase selectedUnit, bool criticalHit, bool isPhysical, out Affinity affinity)
        {
            int damage = 0;
            affinity = selectedUnit.GetAffinity(Move.Type);
            damage += Random.Range(Move.MinimumDamage, Move.MaximumDamage + 1);

            if (isPhysical)
            {
                damage *= Mathf.RoundToInt(1f + (Strength / 100f * Settings.StrenghtMagicAffectMultiplier));
            }
            else
            {
                damage *= Mathf.RoundToInt(1f + (Magic / 100f * Settings.StrenghtMagicAffectMultiplier));
            }

            if (criticalHit)
            {
                damage = Mathf.RoundToInt(damage * Settings.CriticalHitDamageMultiplier);
            }

            if (affinity == Affinity.Weak)
            {
                damage = Mathf.RoundToInt(damage * Settings.WeaknessMultiplier);
            }

            if (affinity == Affinity.Resist)
            {
                damage = Mathf.RoundToInt(damage * Settings.ResistanceMultiplier);
            }

            float enduranceReduction = selectedUnit.Endurance / (selectedUnit.Endurance + 100f);
            damage = Mathf.RoundToInt(damage * (1f - enduranceReduction));
            float AttackBuffMultiplier = 1f + ((float)AttackBuff * 0.25f);
            float DefenseBuffMultiplier = 1f + ((float)selectedUnit.DefenseBuff * 0.25f);
            DefenseBuffMultiplier = Mathf.Clamp(DefenseBuffMultiplier, 0.25f, 1.75f); // no acidental healing from buffs
            damage = Mathf.RoundToInt(damage * AttackBuffMultiplier / DefenseBuffMultiplier);

            return damage;
        }
        void UseOffensiveMove(MoveBase Move, UnitBase selectedUnit)
        {
            // Calculate the all the variables of this attack before we execute it and take away the costs

            bool evaded = DidEvade(selectedUnit);
            bool isPhysical = Move.Type == Elements.Slash || Move.Type == Elements.Gun || Move.Type == Elements.Bash;
            bool criticalHit = isCriticalHit(isPhysical);
            int damage = CalculateDamage(Move,selectedUnit,criticalHit,isPhysical,out Affinity affinity);
            

            if (Move.AppliedEffect != null)
            {
                EffectBase newEffect = Instantiate(Move.AppliedEffect);
                newEffect.OnApply(); // Call the effect's OnApply method
                newEffect.CurrentDuration = newEffect.Duration;

                selectedUnit.AgilityBuff += newEffect.AgilityModifier;
                selectedUnit.AttackBuff += newEffect.AttackModifier;
                selectedUnit.DefenseBuff += newEffect.DefenseModifier;
                
                selectedUnit.AgilityBuff = Mathf.Clamp(selectedUnit.AgilityBuff, -3, 3);
                selectedUnit.AttackBuff = Mathf.Clamp(selectedUnit.AttackBuff, -3, 3);
                selectedUnit.DefenseBuff = Mathf.Clamp(selectedUnit.DefenseBuff, -3, 3);


                selectedUnit.AppliedEffects.Add(newEffect); // Add the effect to the unit's list of applied effects
                selectedUnit.OnBuffStarted(newEffect); // Call the buff started animation
            }

            // now we can calculate the AP cost and remove the costs from the unit
            RemoveHP(Move.HPCost);
            RemoveMP(Move.MPCost);
            // Check if the move was evaded, critical hit or normal hit
            if (evaded)
            {
                AionManager.Instance.ActionPoints -= Mathf.RoundToInt(Move.APCost * Settings.APEvasionPunishmentMultiplier); // punish the player for evasion
                OnMoveMiss(Move, selectedUnit); // Call the miss animation
            }
            else
            if (criticalHit)
            {
                AionManager.Instance.ActionPoints -= Mathf.RoundToInt(Move.APCost * Settings.ApRewardMultiplier); // reward the player for a critical hit
                if (affinity == Affinity.Absorb)
                    selectedUnit.AddHP(damage); // Attack got absorbed 
                else
                    selectedUnit.RemoveHP(damage);

                OnMoveHit(Move, selectedUnit, criticalHit, affinity == Affinity.Absorb, damage); // Call the hit animation
            }
            else
            {
                AionManager.Instance.ActionPoints -= Move.APCost; // Normal AP cost
                if (affinity == Affinity.Absorb)
                    selectedUnit.AddHP(damage); // Attack got absorbed
                else
                    selectedUnit.RemoveHP(damage);

                OnMoveHit(Move, selectedUnit, criticalHit, affinity == Affinity.Absorb, damage); // Call the hit animation
            }
        }
        #endregion

        #region Animation Callbacks
        // These are used to call animations and special effects
        public virtual void OnMoveHit(MoveBase move, UnitBase target, bool critical,bool absorbed, int Damage)
        {

        }
        public virtual void OnMoveMiss(MoveBase move, UnitBase target)
        {

        }
        public virtual void OnBuffEnded(EffectBase effect)
        {

        }
        public virtual void OnBuffStarted(EffectBase effect)
        {

        }
        #endregion
    }
}
