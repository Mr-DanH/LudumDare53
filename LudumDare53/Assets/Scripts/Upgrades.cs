using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrade
{
    public enum UpgradeType
    {
        None = 0,
        ExtraLife = 1,
        ExtraPigeon = 2,
        IncreasePlayerSpeed = 3,
        IncreaseFiringSpeed = 4,
        IncreasePigeonSpeed = 5
    }

    public UpgradeType Type;
    public Sprite Icon;
    public string Description;

    public void Chosen()
    {
        switch (Type)
        {
            case UpgradeType.ExtraLife:
            {
                LevelDetails.Instance.IncreaseMaxLife();
                break;
            }
            case UpgradeType.ExtraPigeon:
            {
                LevelDetails.Instance.IncreaseMaxPigeons();
                break;
            }
            case UpgradeType.IncreasePlayerSpeed:
            {
                LevelDetails.Instance.IncreasePlayerSpeed();
                break;
            }
            case UpgradeType.IncreaseFiringSpeed:
            {
                LevelDetails.Instance.IncreaseFiringSpeed();
                break;
            }
            case UpgradeType.IncreasePigeonSpeed:
            {
                LevelDetails.Instance.IncreasePigeonSpeed();
                break;
            }
        }
    }
}

public class Upgrades : MonoBehaviour
{
    [SerializeField] private List<Upgrade> upgrades;

    private List<Upgrade> upgradePool = new List<Upgrade>();

    void Awake()
    {
        RefreshUpgradePool();
    }

    public List<Upgrade> GetRandomUpgrades(int amount)
    {
        if (upgradePool.Count <= amount*2)
        {
            RefreshUpgradePool();
        }

        List<Upgrade> possibleUpgrades = new List<Upgrade>();

        for(int i = 0; i < amount; i++)
        {
            int index = 0;
            Upgrade nextUpgrade = upgradePool[index];
            if(possibleUpgrades.Exists(x=>x.Type == nextUpgrade.Type))
            {
                index = upgradePool.FindIndex(1, x=>x.Type != nextUpgrade.Type);
                nextUpgrade = upgradePool[index];
            }

            upgradePool.RemoveAt(index);
            possibleUpgrades.Add(nextUpgrade);
        }

        return possibleUpgrades;
    }

    private void RefreshUpgradePool()
    {
        List<Upgrade> upgradesToAdd = new List<Upgrade>(upgrades);
        upgradesToAdd.Shuffle<Upgrade>();

        upgradePool.AddRange(upgradesToAdd);
    }
}
