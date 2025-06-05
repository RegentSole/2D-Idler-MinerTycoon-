using UnityEngine;

[CreateAssetMenu(fileName = "New Auto Production", menuName = "Auto Production Data")]
public class AutoProductionUpgradeData : ScriptableObject
{
    public ResourceType targetResource;
    public ResourceType currencyResource;
    public string upgradeName;
    [TextArea] public string description;
    public float baseCost = 100;
    public float costMultiplier = 1.5f;
    public float productionPerSecond = 1f;
    public int GetCurrentCost(int currentLevel)
    {
        return Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
    }
}