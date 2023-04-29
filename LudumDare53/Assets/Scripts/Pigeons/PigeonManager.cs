using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonManager : Singleton<PigeonManager>
{
    [SerializeField] private Transform spawningParent;
    [SerializeField] private BasicPigeon basicPigeonTemplate;

    private List<Pigeon> firedPigeons = new List<Pigeon>();
    private List<Pigeon> availablePigeons = new List<Pigeon>();

    public override void Awake()
    {
        base.Awake();
        CollisionDetector.Instance.OnCollisionTriggered += HandleCollisionTriggered;
    }

    public void FireNext(Vector2 direction)
    {
        var pigeonTypes = System.Enum.GetValues(typeof(Pigeon.PigeonType));
        Pigeon.PigeonType pigeonType = (Pigeon.PigeonType)Random.Range(1, pigeonTypes.Length);

        Pigeon nextPigeon = GetPigeonInstance(pigeonType);
        nextPigeon.gameObject.SetActive(true);

        RectTransform nextPigeonTransform = nextPigeon.transform as RectTransform;
        CollisionDetector.Instance.Register(CollidableObject.ColliderType.Pigeon, nextPigeonTransform);

        nextPigeon.Fire(direction);
        firedPigeons.Add(nextPigeon);
    }

    public void Tick()
    {
        firedPigeons.ForEach(x=> x.Tick());
    }

    private Pigeon GetPigeonInstance(Pigeon.PigeonType type)
    {
        Pigeon chosenPigeon = availablePigeons.Find(x=>x.Type == type);
        if (chosenPigeon != null)
        {
            availablePigeons.Remove(chosenPigeon);
        }
        else
        {
            chosenPigeon = InstantiatePigeon(type);
        }

        return chosenPigeon;
    }

    private Pigeon InstantiatePigeon(Pigeon.PigeonType type)
    {
        switch(type)
        {
            case Pigeon.PigeonType.BASIC:
                return Instantiate<BasicPigeon>(basicPigeonTemplate, transform.position, transform.rotation, spawningParent);
        }

        return null;
    }

    private void PigeonReturned(Pigeon pigeon)
    {
        pigeon.gameObject.SetActive(false);
        firedPigeons.Remove(pigeon);
        availablePigeons.Add(pigeon);
    }

    private void HandleCollisionTriggered(List<CollidableObject> collidables)
    {
        CollidableObject collidable = collidables.Find(x=>x.Type == CollidableObject.ColliderType.Pigeon);

        if (collidable != null)
        {
            Pigeon returnedPigeon = collidable.RectTransform.GetComponent<Pigeon>();
            PigeonReturned(returnedPigeon);
            CollisionDetector.Instance.UnRegister(collidable);
        }
    }
}
