using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollidableObject
{
    public enum ColliderType
    {
        Player = 1,
        Pigeon = 2,
        Enemy = 3,
        Building = 4,
    }

    public ColliderType Type { get; private set; }
    public RectTransform RectTransform { get; private set; }

    public CollidableObject(ColliderType type, RectTransform objectRect)
    {
        Type = type;
        RectTransform = objectRect;
    }
}

public class CollisionDetector : Singleton<CollisionDetector>
{
    public System.Action<List<CollidableObject>> OnCollisionTriggered;

    private List<CollidableObject> collidableObjects = new List<CollidableObject>();

    public void Register(CollidableObject.ColliderType type, RectTransform objectRect)
    {
        collidableObjects.Add(new CollidableObject(type, objectRect));
    }

    public void UnRegister(CollidableObject collidable)
    {
        collidableObjects.Remove(collidable);
    }
    public void UnRegister(RectTransform collidable)
    {
        collidableObjects.RemoveAll(x=>x.RectTransform == collidable);
    }

    public void Tick()
    {
        CheckForCollisions();
    }

    private void CheckForCollisions()
    {
        var collidableObjectsA = new List<CollidableObject>(collidableObjects);
        var collidableObjectsB = new List<CollidableObject>(collidableObjects);

        foreach (CollidableObject collidable in collidableObjectsA)
        {
            collidableObjectsB.Remove(collidable);
            
            Rect collidableRect = collidable.RectTransform.rect;
            collidableRect.position += (Vector2)collidable.RectTransform.position;
            CollidableObject.ColliderType collidableType = collidable.Type;

            foreach (CollidableObject otherCollidable in collidableObjectsB)
            {
                if (ShouldCheckCollision(collidableType, otherCollidable.Type))
                {
                    Rect otherCollidableRect = otherCollidable.RectTransform.rect;
                    otherCollidableRect.position += (Vector2)otherCollidable.RectTransform.position;

                    if(collidableRect.Overlaps(otherCollidableRect))
                    {
                        OnCollisionTriggered(new List<CollidableObject>(){collidable, otherCollidable});

                    }
                }
            }
        }
    }

    private bool ShouldCheckCollision(CollidableObject.ColliderType firstType, CollidableObject.ColliderType secondType)
    {
        switch (firstType)
        {
            case CollidableObject.ColliderType.Player:
            {
                return secondType == CollidableObject.ColliderType.Enemy;
            }
            case CollidableObject.ColliderType.Pigeon:
            {
                return secondType == CollidableObject.ColliderType.Enemy || secondType == CollidableObject.ColliderType.Building;
            }
            case CollidableObject.ColliderType.Enemy:
            {
                return secondType == CollidableObject.ColliderType.Player || secondType == CollidableObject.ColliderType.Pigeon;                
            }
            case CollidableObject.ColliderType.Building:
            {
                return secondType == CollidableObject.ColliderType.Pigeon;
            }
        }
        return false;
    }
}
