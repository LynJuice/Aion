using System.Collections.Generic;
using UnityEngine;

namespace Aion
{
    using Aion.Bases;

    [System.Serializable]
    public class EnemyPool
    {
        public List<UnitBase> Enemies;
    }

    public class AionManager : MonoBehaviour
    {
        // Singleton instance
        public static AionManager Instance { get; private set; }
        public List<EnemyPool> EncounterPools = new List<EnemyPool>();
        [SerializeField][Tooltip("Friendly Spawning Points")] List<Transform> FriendlySpawnPositions;
        [SerializeField][Tooltip("Enemy Spawning Points")] List<Transform> EnemySpawnPositions;
        // Units
        public List<UnitBase> FriendlyUnits = new List<UnitBase>();
        List<UnitBase> EnemyUnits = new List<UnitBase>();
        // Runtime data
        public int ActionPoints = 0; // ActionPoints This Turn
        bool IsPlayerTurn = false; // Is it the player's turn? // this should be set to the reverse of who we want to start with
        List<UnitBase> unitQueue = new List<UnitBase>(); // Queue of units to process this turn
        public UnitBase TurnsEndingUnit;
        #region Initalization 
        void Awake()
        {
            // Ensure that there is only one instance of AionManager in this scene
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject); // Destroy any duplicates
            }
            SummonCharacters();
            Continue();
        }
        int CalculateActionPoints()
        {
            int points = 0;
            if (IsPlayerTurn)
            {
                foreach (UnitBase unit in FriendlyUnits)
                {
                    points += unit.PointsPerTurn;
                }
            }
            else
            {
                foreach (UnitBase unit in EnemyUnits)
                {
                    points += unit.PointsPerTurn;
                }
            }
            return points;
        }
        void FillCurrentQueue()
        {
            unitQueue.Clear();

            if (IsPlayerTurn)
            {
                foreach (UnitBase unit in FriendlyUnits)
                {
                    if (!unit.IsDead)
                    {
                        unitQueue.Add(unit);
                    }
                }
            }
            else
            {
                foreach (UnitBase unit in EnemyUnits)
                {
                    if (!unit.IsDead)
                    {
                        unitQueue.Add(unit);
                    }
                }
            }

            // sort by Agility
            unitQueue.Sort((a, b) => b.Agility.CompareTo(a.Agility));
        }
        UnitBase FindCurrentlyActiveUnit()
        {
            UnitBase ActiveUnit = null;
            foreach (UnitBase unit in FriendlyUnits)
            {
                if (unit.IsActive)
                {
                    ActiveUnit = unit;
                    break;
                }
            }
            if (ActiveUnit == null) // then it must be in the enemies
                foreach (UnitBase unit in EnemyUnits)
                {
                    if (unit.IsActive)
                    {
                        ActiveUnit = unit;
                        break;
                    }
                }

            if (ActiveUnit == null)
                Debug.LogError("Something went wrong... no active units found");

            return ActiveUnit;
        }
        void SummonCharacters()
        {
            // Summon Friendly Units
            for (int i = 0; i < FriendlyUnits.Count; i++)
            {
                Instantiate(FriendlyUnits[i], FriendlySpawnPositions[i].position, Quaternion.identity);
            }

            // Summon Enemy Units
            int poolIndex = Random.Range(0, EncounterPools.Count);
            EnemyPool selectedPool = EncounterPools[poolIndex];

            for (int i = 0; i < selectedPool.Enemies.Count; i++)
            {
                if (i < EnemySpawnPositions.Count)
                {
                    Instantiate(selectedPool.Enemies[i], EnemySpawnPositions[i].position, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("Not enough enemy spawn positions for the selected pool.");
                }
            }
        }

        void Continue()
        {
            if (ActionPoints <= 0)
            {
                ActionPoints = CalculateActionPoints();
                IsPlayerTurn = !IsPlayerTurn;
                FillCurrentQueue();
                TurnsEndingUnit = FindCurrentlyActiveUnit();
            }

            UnitBase currentUnit = unitQueue[0];
            currentUnit.SetActive();
            unitQueue.RemoveAt(0);
        }
        #endregion
    }
}

