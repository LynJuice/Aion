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
        #region Initalization 
        void Awake()
        {
            // Ensure that there is only one instance of AionManager
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}

