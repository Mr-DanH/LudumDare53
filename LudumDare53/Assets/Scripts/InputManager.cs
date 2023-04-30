using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private bool turnOnLogs = true;

    private void OnMove(InputValue value)
    {
        Vector2 movement = value.Get<Vector2>();
        Player.Instance.SetMovementDirection(movement);
        Log($"Movement: {movement}");
    }

    private void OnFire(InputValue value)
    {
        bool fired = value.isPressed;
        if(fired)
        {
            Player.Instance.Fire();
        }
        Log($"Fire: {fired}");
    }

    private void OnFireRight(InputValue value)
    {
        bool fired = value.isPressed;
        if(fired)
        {
            Player.Instance.FireRight();
        }
        Log($"FireRight: {fired}");
    }

    private void OnFireLeft(InputValue value)
    {
        bool fired = value.isPressed;
        if(fired)
        {
            Player.Instance.FireLeft();
        }
        Log($"FireLeft: {fired}");
    }

    public void OnButtonConfirm()
    {
        Log("Confirm!");
    }

    private void Log(string inputType)
    {
        if (turnOnLogs)
        {
            Debug.Log($"Input Received! {inputType}");
        }
    }
}
