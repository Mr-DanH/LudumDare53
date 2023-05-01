using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Podgen : MonoBehaviour
{
    [SerializeField] private List<float> possibleRotationSpeeds;
    [SerializeField] private RectTransform image;

    public RectTransform Collidable { get { return image; } }

    private float rotationSpeed;
    private int direction;

    void Awake()
    {
        direction = Random.value > 0.5 ? 1 : -1;
        rotationSpeed = possibleRotationSpeeds[Random.Range(0, possibleRotationSpeeds.Count)] * direction;

        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        image.localRotation = Quaternion.Euler(0,0,50*direction);
        image.localScale = new Vector3(direction, 1, 1);
    }

    public void Tick()
    {
        float rotationZ = transform.localRotation.eulerAngles.z + rotationSpeed*Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, 0, rotationZ);   
    }
}
