using UnityEngine;

namespace Aion.Bases
{
    using System.Collections.Generic;
    using Aion.Enumeration;
    [CreateAssetMenu(fileName = "Aion", menuName = "Aion/Aion", order = 1)]
    public class AionBase : ScriptableObject
    {
        // Summary:
        // This is the base class for all Aion objects. It contains the title and description of the object, as well as the affinities.
        [SerializeField] string Title;
        [SerializeField] string Description;
        public Affinity Slash = Affinity.Neutral;
        public Affinity Bash = Affinity.Neutral;
        public Affinity Gun = Affinity.Neutral;
        public Affinity Fire = Affinity.Neutral;
        public Affinity Ice = Affinity.Neutral;
        public Affinity Electric = Affinity.Neutral;
        public Affinity Wind = Affinity.Neutral;
        public Affinity Light = Affinity.Neutral;
        public Affinity Dark = Affinity.Neutral;

        // List of moves for this Aion
        public List<MoveBase> Moves = new List<MoveBase>();
    }
}

