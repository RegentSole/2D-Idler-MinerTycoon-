using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public ResourceType targetResource;
    public ResourceType currencyResource;
    public string upgradeName;
    [TextArea] public string description;
    public float baseCost = 10;
    public float costMultiplier = 1.5f;
    public float productionBonusPerLevel = 0.5f;

    public int GetCurrentCost(int currentLevel)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }
}