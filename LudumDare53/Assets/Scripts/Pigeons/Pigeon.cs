using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pigeon : MonoBehaviour
{
    public enum PigeonType
    {
        NONE = 0,
        BASIC = 1,
    }

    [SerializeField] protected float baseSpeed;

    protected RectTransform rect;

    protected virtual Vector3 firedDirection { get; set; }

    public abstract PigeonType Type { get; }

    public abstract void Fire(Vector2 direction);

    public abstract void Tick();

    protected virtual void Awake()
    {
        rect = transform as RectTransform;
    }

    protected Vector2 ClampToScreen(Vector2 newPosition)
    {
        int screenY = Screen.height/2;
        int screenX = Screen.width/2;
        float halfWidth = rect.rect.width * 0.5f;
        float halfHeight = rect.rect.height * 0.5f;

        newPosition.x = Mathf.Clamp(newPosition.x, -screenX + halfWidth, screenX - halfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenY + halfHeight, screenY - halfHeight);

        return newPosition;
    }
}
