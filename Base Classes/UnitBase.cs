using UnityEngine;

namespace Aion.Bases
{
    using Enumeration;

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
        bool isCriticalHit()
        {
            int roll = Random.Range(1, 101);
            float curve = Mathf.Pow(Luck / 100f, Settings.CriticalHitCurveExponent);
            return roll <= curve * 100f;
        }
        bool DidEvade(UnitBase attacker)
        {
            float attackerAccuracy = attacker.Agility * Settings.AccuracyAgilityMultiplier + attacker.Luck * Settings.AccuracyLuckMultiplier;
            float defenderEvasion = Agility * Settings.EvasionAgilityMultiplier + Luck * Settings.EvasionLuckMultiplier;

            float evadeChance = Mathf.Clamp(100f - attackerAccuracy + defenderEvasion, Settings.EvasionClampMin, Settings.EvasionClampMax);
            float roll = Random.Range(0f, 100f);

            return roll < evadeChance;
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

            if (move.Type != Elements.Passive)
                return UseOffensiveMove(move, selectedUnit);
            else
                return UseDefensiveMove(move, selectedUnit);
        }
        bool UseDefensiveMove(MoveBase Move, UnitBase selectedUnit)
        {
            // Todo In the future, add defensive moves that can heal or buff the unit
            return true; 
        }
        bool UseOffensiveMove(MoveBase Move, UnitBase selectedUnit)
        {
            // Calculate the all the variables of this attack before we execute it and take away the costs
            int damage = 0;
            bool criticalHit = isCriticalHit();
            bool evaded = DidEvade(selectedUnit);
            bool isPhysical = Move.Type == Elements.Slash || Move.Type == Elements.Gun || Move.Type == Elements.Bash;
            Affinity affinity = selectedUnit.GetAffinity(Move.Type);
            damage += Random.Range(Move.MinimumDamage, Move.MaximumDamage);

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

            // now we can calculate the AP cost and remove the costs from the unit
            RemoveHP(Move.HPCost);
            RemoveMP(Move.MPCost);
            if (evaded)
            {
                AionManager.Instance.ActionPoints = Mathf.RoundToInt(Move.APCost * Settings.APEvasionPunishmentMultiplier); // punish the player for evasion
                OnMoveMiss(Move, selectedUnit); // Call the miss animation
                return true;
            }
            else
            if (criticalHit)
            {
                AionManager.Instance.ActionPoints = Mathf.RoundToInt(Move.APCost * Settings.ApRewardMultiplier); // reward the player for a critical hit
                if (affinity == Affinity.Absorb)
                    selectedUnit.AddHP(damage); // Attack got absorbed 
                else
                    selectedUnit.RemoveHP(damage);

                OnMoveHit(Move, selectedUnit, criticalHit, affinity == Affinity.Absorb, damage); // Call the hit animation
                return true;
            }
            else
            {
                AionManager.Instance.ActionPoints = Move.APCost; // Normal AP cost
                if (affinity == Affinity.Absorb)
                    selectedUnit.AddHP(damage); // Attack got absorbed
                else
                    selectedUnit.RemoveHP(damage);

                OnMoveHit(Move, selectedUnit, criticalHit, affinity == Affinity.Absorb, damage); // Call the hit animation
                return true;
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
        #endregion
    }
}
