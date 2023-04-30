using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicPigeon : Pigeon
{
    public override PigeonType Type { get { return PigeonType.BASIC; } }

    const float SPAWN_SIZE = 1;
    const float TARGET_SIZE = 0.5f;

    public override void Fire(Vector2 direction)
    {
        firedDirection = direction;
        GetComponentInChildren<Image>().transform.localScale = new Vector3(Mathf.Sign(firedDirection.x), 1, 1);

        transform.localScale = Vector3.one * SPAWN_SIZE;
    }

    public override void Tick()
    {
        if(ReturnState == Pigeon.eReturnState.NONE)
        {
            Vector3 newPosition = transform.localPosition + firedDirection * baseSpeed * Time.deltaTime;
            Vector3 cappedPosition = ClampToScreen(newPosition, 0.9f);
            transform.localPosition = cappedPosition;

            transform.localScale = Vector3.one * Mathf.MoveTowards(transform.localScale.x, TARGET_SIZE, Time.deltaTime * 0.5f);

            if(cappedPosition != newPosition)
                ReturnState = Pigeon.eReturnState.RETURNING;
        }
        else if(ReturnState == Pigeon.eReturnState.RETURNING)
        {
            Vector3 toPlayer = Player.Instance.transform.localPosition - transform.localPosition;
            Vector3 toPlayerDir = toPlayer.normalized;

            Vector3 cross = Vector3.forward;
            float angle = Vector3.Angle(firedDirection, toPlayerDir);

            if(angle != 180)
                cross = Vector3.Cross(firedDirection, toPlayerDir);

            firedDirection = Quaternion.AngleAxis(Mathf.Min(angle, 180 * Time.deltaTime), cross) * firedDirection; 
            GetComponentInChildren<Image>().transform.localScale = new Vector3(Mathf.Sign(firedDirection.x), 1, 1);           

            float dist = Mathf.Min(baseSpeed * Time.deltaTime, toPlayer.magnitude);
            
            Vector3 newPosition = transform.localPosition + firedDirection * dist;
            transform.localPosition = ClampToScreen(newPosition);
            
            transform.localScale = Vector3.one * Mathf.Lerp(TARGET_SIZE, SPAWN_SIZE, Mathf.InverseLerp(100, 0, toPlayer.magnitude));

            if(baseSpeed * Time.deltaTime > toPlayer.magnitude)
                ReturnState = Pigeon.eReturnState.RETURNED;
        }
    }
}
