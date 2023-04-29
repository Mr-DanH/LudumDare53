using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPigeon : Pigeon
{
    public override PigeonType Type { get { return PigeonType.BASIC; } }

    public override void Fire(Vector2 direction)
    {
        firedDirection = direction;
    }

    public override void Tick()
    {
        Vector3 newPosition = transform.localPosition + firedDirection * baseSpeed * Time.deltaTime;
        transform.localPosition = ClampToScreen(newPosition);
    }
}
