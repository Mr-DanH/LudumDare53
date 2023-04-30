using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicPigeon : Pigeon
{
    public override PigeonType Type { get { return PigeonType.BASIC; } }

    public override void Fire(Vector2 direction)
    {
        GetComponentInChildren<Image>().transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

        firedDirection = direction;
    }

    public override void Tick()
    {
        if(ReturnState == Pigeon.eReturnState.NONE)
        {
            Vector3 newPosition = transform.localPosition + firedDirection * baseSpeed * Time.deltaTime;
            Vector3 cappedPosition = ClampToScreen(newPosition, 0.9f);
            transform.localPosition = cappedPosition;

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

            float dist = Mathf.Min(baseSpeed * Time.deltaTime, toPlayer.magnitude);
            
            Vector3 newPosition = transform.localPosition + firedDirection * dist;
            transform.localPosition = ClampToScreen(newPosition);

            if(baseSpeed * Time.deltaTime > toPlayer.magnitude)
                ReturnState = Pigeon.eReturnState.RETURNED;
        }
    }
}
