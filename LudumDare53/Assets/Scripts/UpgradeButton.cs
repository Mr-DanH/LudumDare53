using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum InputChoice
{
    Right = 1,
    Left = 0
}

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;

    public Upgrade Upgrade { get; private set; }

    public void Setup(Upgrade upgrade)
    {
        icon.sprite = upgrade.Icon;
        description.SetText(upgrade.Description);
        Upgrade = upgrade;
    }

}
