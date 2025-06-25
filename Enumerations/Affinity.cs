namespace Aion.Enumeration
{
    public enum Affinity
    {
        Neutral = 0, // No change
        Weak = 1, // 1.5x damage taken and only half of points taken
        Resist = 2, // 0.5x damage taken and double points taken
        Repel = 3, // 0 damage taken and repels the attack back to the attacker, all points are taken
        Absorb = 4 // damage turns into health and all points are taken
    }
}