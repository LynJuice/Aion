using UnityEngine;

namespace Aion.Bases
{
    using Aion.Enumeration;
    [CreateAssetMenu(fileName = "Move", menuName = "Aion/Move", order = 1)]
    public class MoveBase : ScriptableObject
    {
        // Summary:
        // This is the base class for all moves in Aion. It contains the name, description, type, and chance of working.
        public int Chance = 100;
        public Elements Type = Elements.Slash;
        public string Name = "New Move";
        public string Description = "This is a new move.";
    }
}