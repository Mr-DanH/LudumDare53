using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeArea : MonoBehaviour
{
    public System.Action<Upgrade> OnUpgradeChosen;

    [SerializeField] private UpgradeButton template;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Upgrades upgradeConfig;

    const int NUM_UPGRADES = 2;

    private List<UpgradeButton> buttons = new List<UpgradeButton>();

    public void Setup()
    {
        InputManager.Instance.OnChoiceMade += ChoiceHandled;
        CreateUpgrades();
    }

    private void CreateUpgrades()
    {
       List<Upgrade> upgrades = upgradeConfig.GetRandomUpgrades(NUM_UPGRADES);
       for(int i = 0; i < upgrades.Count; i++)
       {
            UpgradeButton clone = Instantiate<UpgradeButton>(template, buttonContainer);
            clone.Setup(upgrades[i]);
            buttons.Add(clone);
       }
    }

    private void ClearUpgrades()
    {
        buttons.ForEach(x=> Destroy(x.gameObject));
        buttons.Clear();
    }

    private void ChoiceHandled(InputChoice choice)
    {
        InputManager.Instance.OnChoiceMade -= ChoiceHandled;
        UpgradeButton chosenButton = buttons[(int)choice];
        chosenButton.Upgrade.Chosen();
        // todo fancy 
        OnUpgradeChosen.Invoke(chosenButton.Upgrade);
        ClearUpgrades();
    }

}
