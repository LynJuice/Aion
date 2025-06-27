using UnityEngine;

namespace Aion.Bases
{
    using Aion.Enumeration;
    [CreateAssetMenu(fileName = "New Equipment", menuName = "Aion/Equipment")]
    public class EquipmentBase : ScriptableObject
    {
        public string Name;
        public string Description;
        public EquipmentType equipmentType;
        [Header("Stat Attributes")]
        [Tooltip("Increases the damage by physical attacks")] [Range(0,100)] public int StrenghtModifier;
        [Tooltip("Increases the damage by magical attacks")] [Range(0,100)] public int MagicModifier;
        [Tooltip("Decreases the damage by physical and magical attacks")] [Range(0,100)] public int EnduraceModifier;
        [Tooltip("Increases the chance to evade and increases accuracy")] [Range(0,100)] public int AgilityModifier;
        [Tooltip("Increases the chance to make critcal hits")] [Range(0,100)] public int LuckModifier;
        [Tooltip("Moves that don't depend on the Aion")] public MoveBase[] ExtraMoves;

    }
}

