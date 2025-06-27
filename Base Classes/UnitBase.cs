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
        [Tooltip("Points given per turn")] public int PointsPerTurn = 1;
        [SerializeField][Tooltip("The Current Aion")] AionBase Aion;
        [Tooltip("Affects physical attack damage dealt")][Range(1, 100)] public int Strength = 10;
        [Tooltip("Affects magical attack damage dealt")][Range(1, 100)] public int Magic = 10;
        [Tooltip("Affects both physical and magical damage taken")][Range(1, 100)] public int Endurance = 10;
        [Tooltip("Affects accuracy and evasion and turn order")][Range(1, 100)] public int Agility = 10;
        [Tooltip("Affects chances of dealing or taking critical damage")][Range(1, 100)] public int Luck = 10;
        [SerializeField][Tooltip("Ultimate power move of the unit")] MoveBase UltimateMove = new MoveBase();
        [SerializeField][Tooltip("Ultimate moves charge percentage")][Range(0, 100)] int ChargePercentage = 0; 
        public List<EquipmentBase> Equipment = new List<EquipmentBase>();
        [Space]
        [SerializeField][Tooltip("Current Health of the unit")] int CurrentHealth;
        [SerializeField][Tooltip("Current Mana of the unit")] int CurrentMana;
        [SerializeField][Tooltip("List of currenly applied effects")] List<EffectBase> AppliedEffects = new List<EffectBase>();
        [SerializeField][Tooltip("Affects Attack (Physical and Magical) Runtime only, can be increased or decresed (-3,3) with buff and debuff effects")] int AttackBuff;
        [SerializeField][Tooltip("Affects Defense (Physical and Magical) Runtime only, can be increased or decresed (-3,3) with buff and debuff effects")] int DefenseBuff;
        [SerializeField][Tooltip("Affects Critical hit chance and evasion Runtime only, can be increased or decresed (-3,3) with buff and debuff effects")] int AgilityBuff;
        [SerializeField][Tooltip("A copy of Aions moves and equipment moves because there is a possibily a same Aion could be in the same battle as another with a differnt moveset")] List<MoveBase> Moves;
        public bool IsActive;
        void Awake()
        {
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
            if (Aion == null)
                Debug.LogError("Aion is not set for " + gameObject.name + ". Please assign an AionBase to this unit.");
            EvaluateMoveSet();
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
        public bool UseMove(MoveBase move, List<UnitBase> selectedUnits)
        {
            // Checks to see if the unit can use this move (Failsafe)
            if (!CanUseMP(move.MPCost))
                return false;   // Not enought MP
            if (!CanUseHP(move.HPCost))
                return false;   // Not enought HP
            if (AionManager.Instance.ActionPoints < move.APCost)
                return false;   // Not enough Action Points
            if (!IsActive)
                return false;   // Unit is not active
            if (move.IsUltimate && ChargePercentage != 100 || UltimateMove != move.IsUltimate)
                return false;   // Ultimate move is not charged or the move is a ultimate move but for the wrong user...

            IsActive = false; // Set the unit to inactive after using a move 

            foreach (EffectBase effect in AppliedEffects)
            {
                effect.EveryTurn(this);
                if (effect.CurrentDuration <= 0)
                {
                    effect.OnRemove(); // Call the effect's OnRemove method
                    OnBuffEnded(effect); // Call the buff ended animation
                    AppliedEffects.Remove(effect); // Remove the effect from the unit's list of applied effects
                }
            }

            if (move.Type != Elements.Passive)
            {
                UseOffensiveMove(move, selectedUnits);
                ChargePercentage = Mathf.Clamp(ChargePercentage, 0, 100); // Clamp the charge percentage to 0-100
                return true;
            }
            else
            {
                ChargePercentage = Mathf.Clamp(ChargePercentage, 0, 100); // Clamp the charge percentage to 0-100
                UseDefensiveMove(move, selectedUnits);
                return true;
            }
        }
        void UseDefensiveMove(MoveBase Move, List<UnitBase> selectedUnit)
        {
            foreach (UnitBase unit in selectedUnit)
            {
                if (Move.AppliedEffect != null)
                {
                    AddEffect(Move, unit);
                }

                unit.CurrentHealth = Mathf.Clamp(unit.CurrentHealth + Move.HealingAmount, 0, unit.MaxHealth); // healing
                OnMoveHit(Move,unit,false,false,0);
            }

            RemoveHP(Move.HPCost);
            RemoveMP(Move.MPCost);
            AionManager.Instance.ActionPoints -= Move.APCost; // Normal AP cost
        }
        void EvaluateMoveSet()
        {
            Moves.AddRange(Aion.Moves);

            foreach (EquipmentBase equipment in Equipment)
            {
                if (equipment != null)
                {
                    Strength += equipment.StrenghtModifier;
                    Magic += equipment.MagicModifier;
                    Endurance += equipment.EnduraceModifier;
                    Agility += equipment.AgilityModifier;
                    Luck += equipment.LuckModifier;
                    // Add the moves from the equipment to the unit's moves
                    foreach (MoveBase move in equipment.ExtraMoves)
                    {
                        if (move != null && !Moves.Contains(move))
                        {
                            Moves.Add(move);
                        }
                    }
                }
            }
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
                ChargePercentage += Settings.ChargePercentageFromWeaknessHit; // Charge the ultimate move for hitting a weak unit
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
        void UseOffensiveMove(MoveBase Move, List<UnitBase> selectedUnits)
        {
            // Calculate the all the variables of this attack before we execute it and take away the costs

            bool AtleastOneEvaded = false;
            bool AtleastOneCriticalHit = false;
            bool isPhysical = Move.Type == Elements.Slash || Move.Type == Elements.Gun || Move.Type == Elements.Bash;


            foreach (UnitBase unit in selectedUnits)
            {
                bool evaded = DidEvade(unit);
                bool criticalHit = isCriticalHit(isPhysical);
                int damage = CalculateDamage(Move, unit, criticalHit, isPhysical, out Affinity affinity);

                if (evaded)
                    AtleastOneEvaded = true;
                if (criticalHit)
                    AtleastOneCriticalHit = true;

                if (Move.AppliedEffect != null && !evaded)
                {
                    AddEffect(Move, unit);
                }

                if (evaded)
                {
                    OnMoveMiss(Move,unit);
                }
                else if (criticalHit)
                {
                    if (affinity == Affinity.Absorb)
                        unit.AddHP(damage); // Attack got absorbed 
                    else
                        unit.RemoveHP(damage);

                    OnMoveHit(Move, unit,criticalHit,affinity == Affinity.Absorb,damage);
                }
                else
                {
                    if(affinity == Affinity.Absorb)
                        unit.AddHP(damage);
                    else
                        unit.RemoveHP(damage);

                    OnMoveHit(Move, unit, criticalHit, affinity == Affinity.Absorb, damage);
                }
            }

            // now we can calculate the AP cost and remove the costs from the unit
            RemoveHP(Move.HPCost);
            RemoveMP(Move.MPCost);

            if (Move.IsUltimate)
            {
                ChargePercentage = 0;// reset it
            }

            // Check if the move was evaded, critical hit or normal hit
            if (AtleastOneEvaded && !Move.IsUltimate)
            {
                AionManager.Instance.ActionPoints -= Mathf.RoundToInt(Move.APCost * Settings.APEvasionPunishmentMultiplier); // punish the player for evasion
            }
            else
            if (AtleastOneCriticalHit)
            {
                AionManager.Instance.ActionPoints -= Mathf.RoundToInt(Move.APCost * Settings.ApRewardMultiplier); // reward the player for a critical hit
                ChargePercentage += Settings.ChargePercentageFromCriticalHit;
            }
            else
            {
                AionManager.Instance.ActionPoints -= Move.APCost; // Normal AP cost
            }
        }
        void AddEffect(MoveBase Move, UnitBase unit)
        {
            EffectBase newEffect = Instantiate(Move.AppliedEffect);
            newEffect.OnApply(); // Call the effect's OnApply method
            newEffect.CurrentDuration = newEffect.Duration;

            unit.AgilityBuff += newEffect.AgilityModifier;
            unit.AttackBuff += newEffect.AttackModifier;
            unit.DefenseBuff += newEffect.DefenseModifier;

            unit.AgilityBuff = Mathf.Clamp(unit.AgilityBuff, -3, 3);
            unit.AttackBuff = Mathf.Clamp(unit.AttackBuff, -3, 3);
            unit.DefenseBuff = Mathf.Clamp(unit.DefenseBuff, -3, 3);


            unit.AppliedEffects.Add(newEffect); // Add the effect to the unit's list of applied effects
            unit.OnBuffStarted(newEffect); // Call the buff started animation
        }
        public void SetActive()
        {
            if (Aion == null)
            {
                Debug.LogError("Aion is not set for " + gameObject.name + ". Please assign an AionBase to this unit.");
                return;
            }
            IsActive = true;
            OnActivated();
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
        public virtual void OnActivated()
        {
        }
        #endregion
    }
}
