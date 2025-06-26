using UnityEngine;

public static class Settings
{
    // Move function Settings
    public static float WeaknessMultiplier = 1.5f;  // The bigger value, the more damage is dealt to a weak unit
    public static float ResistanceMultiplier = 0.25f; // The smaller value, the less damage is dealt to a resistant unit
    public static float StrenghtMagicAffectMultiplier = 0.5f; // The multiplier for the effect of Strength and Magic on damage dealt
    public static float CriticalHitDamageMultiplier = 2f; // The multiplier for the damage dealt on a critical hit
    public static float APEvasionPunishmentMultiplier = 2f; // default 2 doubles the Action points cost of evaded move
    public static float ApRewardMultiplier = 0.5f; // The multiplier for the Action Points rewarded for a successful move
    public static float CriticalHitCurveExponent = 2f; // 2.0f = exponential, 1.0f = linear
    public static float AccuracyAgilityMultiplier = 1.2f;
    public static float AccuracyLuckMultiplier = 0.3f;
    public static float EvasionAgilityMultiplier = 1.5f;
    public static float EvasionLuckMultiplier = 0.5f;
    public static float EvasionClampMin = 5f;
    public static float EvasionClampMax = 95f;
    public static float AgilityBuffMultiplier = 0.05f; // The multiplier for the Agility buff effect on Evasion and crical hit chance
}
