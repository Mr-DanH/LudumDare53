using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Podgen : MonoBehaviour
{
    public enum State
    {
        None = 0,
        Flying = 1,
        Resting = 2
    }

    [SerializeField] private List<float> possibleRotationSpeeds;
    [SerializeField] private RectTransform image;

    public RectTransform Collidable { get { return image; } }

    private float rotationSpeed;
    private int direction;
    public State Status { get; private set; }

    public void Tick()
    {
        if (Status == State.Flying)
        {
            float rotationZ = transform.localRotation.eulerAngles.z + rotationSpeed*Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
        }
    }

    public void Injured()
    {
        Status = State.Resting;
        gameObject.SetActive(false);
    }

    public void Launch()
    {
        SetFlightDirection();
        Status = State.Flying;
    }

    private void SetFlightDirection()
    {
        direction = Random.value > 0.5 ? 1 : -1;
        rotationSpeed = possibleRotationSpeeds[Random.Range(0, possibleRotationSpeeds.Count)] * direction;

        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        image.localRotation = Quaternion.Euler(0,0,50*direction);
        image.localScale = new Vector3(direction, 1, 1);

        gameObject.SetActive(true);
    }
}
