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
        // Units
        List<UnitBase> FriendlyUnits = new List<UnitBase>();
        List<UnitBase> EnemyUnits = new List<UnitBase>();
        // Runtime data
        public int ActionPoints = 0; // ActionPoints This Turn
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
        }


        #endregion
    }
}

