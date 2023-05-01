using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDetails : Singleton<LevelDetails>
{
    [Header("Pigeons")]
    [SerializeField] public int MaxNumPigeons = 2;
    [SerializeField] public float PigeonFiringDelay = 1;
    [SerializeField] private float pigeonFiringDelayDecrease = 0.2f;
    [Header("Pigeon Speed")]
    [SerializeField] private float pigeonSpeedIncreaseAmount;
    public float PigeonSpeedOffset { get; private set; }

    [Space]

    [Header("Player")]
    [SerializeField] public int MaxLives = 3;
    [Header("Player Speed")]
    [SerializeField] public float PlayerSpeed;
    [SerializeField] private float playerSpeedIncreaseAmount = 20;


    public void IncreaseMaxLife()
    {
        MaxLives++;
        Player.Instance.PlayerLives++;
    }

    public void IncreaseMaxPigeons()
    {
        PigeonManager.Instance.AddBasicPigeon();
    }

    public void UpgradeHomingPigeon()
    {
        PigeonManager.Instance.UpgradePigeon();
    }

    public void IncreasePlayerSpeed()
    {
        PlayerSpeed += playerSpeedIncreaseAmount;
    }

    public void IncreaseFiringSpeed()
    {
        PigeonFiringDelay -= pigeonFiringDelayDecrease;
    }

    public void IncreasePigeonSpeed()
    {
        PigeonSpeedOffset += pigeonSpeedIncreaseAmount;
    }
}
