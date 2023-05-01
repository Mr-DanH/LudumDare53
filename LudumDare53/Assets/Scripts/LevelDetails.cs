using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDetails : Singleton<LevelDetails>
{
    [Header("Pigeons")]
    [SerializeField] public int MaxNumPigeons = 2;
    [SerializeField] private float pigeonFiringDelay = 1;
    public float CurrentPigeonFiringDelay { get; private set; }
    [SerializeField] private float pigeonFiringDelayDecrease = 0.2f;
    [Header("Pigeon Speed")]
    [SerializeField] private float pigeonSpeedIncreaseAmount;
    public float PigeonSpeedOffset { get; private set; }

    [Space]

    [Header("Player")]
    [SerializeField] private int maxLives = 3;
    public int CurrentMaxLives { get; private set; }
    [Header("Player Speed")]
    [SerializeField] private float playerSpeed;
    public float CurrentPlayerSpeed { get; private set; }
    [SerializeField] private float playerSpeedIncreaseAmount = 20;

    public void Reset()
    {
        CurrentMaxLives = maxLives;
        CurrentPigeonFiringDelay = pigeonFiringDelay;
        PigeonSpeedOffset = 0;
        CurrentPlayerSpeed = playerSpeed;
    }

    public void IncreaseMaxLife()
    {
        CurrentMaxLives++;
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
        CurrentPlayerSpeed += playerSpeedIncreaseAmount;
    }

    public void IncreaseFiringSpeed()
    {
        CurrentPigeonFiringDelay -= pigeonFiringDelayDecrease;
        CurrentPigeonFiringDelay = Mathf.Max(CurrentPigeonFiringDelay, 0);
    }

    public void IncreasePigeonSpeed()
    {
        PigeonSpeedOffset += pigeonSpeedIncreaseAmount;
    }
}
