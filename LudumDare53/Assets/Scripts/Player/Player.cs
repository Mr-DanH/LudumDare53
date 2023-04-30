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
    [SerializeField] private RectTransform explodeVfx;

    public int PlayerLives { get; private set; }

    private Vector3 currentDirection = new Vector3();
    private RectTransform rect;

    void Awake()
    {
        rect = transform as RectTransform;
        explodeVfx.gameObject.SetActive(false);

        PlayerLives = 3;

        CollisionDetector.Instance.Register(CollidableObject.ColliderType.Player, rect);
        CollisionDetector.Instance.OnCollisionTriggered += HandleCollisionTriggered;
    }

    public void Tick()
    {
        UpdateMovement();
        pigeonManager.Tick();
    }

    public void Reset()
    {
        PlayerLives = 3;
        explodeVfx.gameObject.SetActive(false);
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
        Vector3 newPosition = rect.localPosition + (currentDirection * baseSpeed * Time.deltaTime);

        int screenY = Mathf.RoundToInt(Screen.height / (transform.lossyScale.x * 2));
        int screenX = Mathf.RoundToInt(Screen.width / (transform.lossyScale.y * 2));
        float halfWidth = rect.rect.width * 0.5f;
        float halfHeight = rect.rect.height * 0.5f;

        newPosition.x = Mathf.Clamp(newPosition.x, -screenX + halfWidth, screenX - halfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenY + halfHeight, screenY - halfHeight);

        rect.localPosition = newPosition;
    }

    private void HandleCollisionTriggered(List<CollidableObject> collidables)
    {
        bool playerInvolved = collidables.Exists(x=>x.Type == CollidableObject.ColliderType.Player);
        if (playerInvolved && PlayerLives > 0)
        {
            PlayerLives--;
            
            explodeVfx.gameObject.SetActive(false);
            var other = collidables.Find(x => x.Type != CollidableObject.ColliderType.Player);
            explodeVfx.transform.position = other.RectTransform.position;
            explodeVfx.gameObject.SetActive(true);
        }
    }
}
