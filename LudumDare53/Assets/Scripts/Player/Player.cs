using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    // todo - pigeon manager
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
    }

    public void SetMovementDirection(Vector2 movement)
    {
        currentDirection = movement;
    }

    public void Fire()
    {

    }

    public void FireRight()
    {

    }

    public void FireLeft()
    {

    }

    private void UpdateMovement()
    {
        Vector3 newPosition = rect.localPosition + (currentDirection * baseSpeed);
        bool clamp = false;
        int screenY = Screen.height/2;
        int screenX = Screen.width/2;
        bool isTooHigh = rect.rect.yMin > screenY;
        bool isTooLow = rect.rect.yMax < -screenY;
        bool isTooRight = rect.rect.xMin > screenX;
        bool isTooLeft = rect.rect.xMax < -screenX;
        clamp = isTooHigh || isTooLow || isTooRight || isTooLeft;

        if (clamp)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, -screenX, screenX);
            newPosition.y = Mathf.Clamp(newPosition.y, -screenY, screenY);
        }

        rect.localPosition = newPosition;
    }
}
