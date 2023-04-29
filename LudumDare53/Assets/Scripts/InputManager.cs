using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        Player.Instance.UpdateMovement(movement);
    }

    private void OnFire(InputValue value)
    {
        bool fired = value.Get<bool>();
        if(fired)
        {
            Player.Instance.Fire();
        }
    }

    private void OnFireRight(InputValue value)
    {
        bool fired = value.Get<bool>();
        if(fired)
        {
            Player.Instance.FireRight();
        }  
    }

    private void OnFireLeft(InputValue value)
    {
        bool fired = value.Get<bool>();
        if(fired)
        {
            Player.Instance.FireLeft();
        }
    }

    // Todo Add Menu Input
}
