using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [Header("Pigeons")]
    [SerializeField] private PigeonManager pigeonManager;
    [SerializeField] private Vector2 fireStraightDirection = new Vector2();
    [SerializeField] private Vector2 fireRightDirection = new Vector2(0.7f, 0.7f);    
    [SerializeField] private Vector2 fireLeftDirection = new Vector2(0.3f, 0.7f);

    [Header("Player")]
    [SerializeField] private float baseSpeed;

    private Vector3 currentDirection = new Vector3();
    private RectTransform rect;

    public override void Awake()
    {
        base.Awake();
        rect = transform as RectTransform;
    }

    public void Tick()
    {
        UpdateMovement();
        pigeonManager.Tick();
    }

    public void SetMovementDirection(Vector2 movement)
    {
        currentDirection = movement;
    }

    public void Fire()
    {
        pigeonManager.FireNext(fireStraightDirection);
    }

    public void FireRight()
    {
        pigeonManager.FireNext(fireRightDirection);
    }

    public void FireLeft()
    {
        pigeonManager.FireNext(fireLeftDirection);
    }

    private void UpdateMovement()
    {
        Vector3 newPosition = rect.localPosition + (currentDirection * baseSpeed);

        int screenY = Mathf.RoundToInt(Screen.height / (transform.lossyScale.x * 2));
        int screenX = Mathf.RoundToInt(Screen.width / (transform.lossyScale.y * 2));
        float halfWidth = rect.rect.width * 0.5f;
        float halfHeight = rect.rect.height * 0.5f;

        newPosition.x = Mathf.Clamp(newPosition.x, -screenX + halfWidth, screenX - halfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenY + halfHeight, screenY - halfHeight);

        rect.localPosition = newPosition;
    }
}
