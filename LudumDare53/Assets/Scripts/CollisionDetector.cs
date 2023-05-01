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
        Podgen = 5,
        Boss = 6
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
    List<CollidableObject.ColliderType> CollisionPairings = new List<CollidableObject.ColliderType>()
        {
            CollidableObject.ColliderType.Player, CollidableObject.ColliderType.Enemy,
            CollidableObject.ColliderType.Pigeon, CollidableObject.ColliderType.Building,
            CollidableObject.ColliderType.Enemy, CollidableObject.ColliderType.Podgen,
            CollidableObject.ColliderType.Player, CollidableObject.ColliderType.Boss,
            CollidableObject.ColliderType.Pigeon, CollidableObject.ColliderType.Boss,
        };

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
        for(int i = 0; i < CollisionPairings.Count; i += 2)
        {
            if ((firstType == CollisionPairings[i] && secondType == CollisionPairings[i + 1])
                ||
                (secondType == CollisionPairings[i] && firstType == CollisionPairings[i + 1]))
                return true;
        }

        return false;
    }
}
