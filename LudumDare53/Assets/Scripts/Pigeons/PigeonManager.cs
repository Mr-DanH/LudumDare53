using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonManager : MonoBehaviour
{
    [SerializeField] private Transform spawningParent;
    [SerializeField] private BasicPigeon basicPigeonTemplate;

    private List<Pigeon> firedPigeons = new List<Pigeon>();
    private List<Pigeon> availablePigeons = new List<Pigeon>();

    public void FireNext(Vector2 direction)
    {
        var pigeonTypes = System.Enum.GetValues(typeof(Pigeon.PigeonType));
        Pigeon.PigeonType pigeonType = (Pigeon.PigeonType)Random.Range(1, pigeonTypes.Length);

        Pigeon nextPigeon = GetPigeonInstance(pigeonType);
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
        firedPigeons.Remove(pigeon);
        availablePigeons.Add(pigeon);
    }
}
