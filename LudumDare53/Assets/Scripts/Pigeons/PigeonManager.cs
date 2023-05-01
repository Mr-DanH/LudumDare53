using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PigeonManager : Singleton<PigeonManager>
{
    [SerializeField] private Transform spawningParent;
    [SerializeField] private BasicPigeon basicPigeonTemplate;
    [SerializeField] private HomingPigeon homingPigeonTemplate;


    private List<Pigeon> firedPigeons = new List<Pigeon>();
    private List<Pigeon> availablePigeons = new List<Pigeon>();
    private List<Pigeon> pooledPigeons = new List<Pigeon>();

    private float fireDelay;
    int m_count;

    void Awake()
    {
        CollisionDetector.Instance.OnCollisionTriggered += HandleCollisionTriggered;
        fireDelay = 0;
    }

    public void NewGame()
    {
        pooledPigeons.Clear();
        pooledPigeons.AddRange(availablePigeons);
        pooledPigeons.AddRange(firedPigeons);

        availablePigeons.Clear();
        firedPigeons.Clear();

        for(int i = 0; i < LevelDetails.Instance.MaxNumPigeons; ++i)
            availablePigeons.Add(GetPigeonInstance(Pigeon.PigeonType.BASIC));

        foreach(var pigeon in availablePigeons)
            pigeon.gameObject.SetActive(false);

        foreach(var pigeon in pooledPigeons)
            pigeon.gameObject.SetActive(false);
    }

    public void FireNext(Vector2 direction)
    {
        if(fireDelay > 0)
            return;

        if(availablePigeons.Count == 0)
            return;

        Pigeon nextPigeon = availablePigeons[0];
        nextPigeon.transform.position = transform.position;
        availablePigeons.RemoveAt(0);

        nextPigeon.gameObject.SetActive(true);

        RectTransform nextPigeonTransform = nextPigeon.transform as RectTransform;
        CollisionDetector.Instance.Register(CollidableObject.ColliderType.Pigeon, nextPigeonTransform);

        nextPigeon.ReturnState = Pigeon.eReturnState.NONE;
        nextPigeon.Fire(direction);
        firedPigeons.Add(nextPigeon);

        fireDelay = LevelDetails.Instance.CurrentPigeonFiringDelay;
    }

    public void Tick()
    {
        firedPigeons.ForEach(x=> x.Tick());

        for(int i = firedPigeons.Count - 1; i >= 0; --i)
        {
            if(firedPigeons[i].ReturnState == Pigeon.eReturnState.RETURNED)
                PigeonReturned(firedPigeons[i]);
        }

        fireDelay -= Time.deltaTime;
    }

    private Pigeon GetPigeonInstance(Pigeon.PigeonType type)
    {
        Pigeon chosenPigeon = pooledPigeons.Find(x=>x.Type == type);
        if (chosenPigeon != null)
        {
            pooledPigeons.Remove(chosenPigeon);
        }
        else
        {
            chosenPigeon = InstantiatePigeon(type);
        }

        chosenPigeon.name = $"{m_count++}-Pigeon";

        return chosenPigeon;
    }

    private Pigeon InstantiatePigeon(Pigeon.PigeonType type)
    {
        switch(type)
        {
            case Pigeon.PigeonType.BASIC:
                return Instantiate<BasicPigeon>(basicPigeonTemplate, transform.position, transform.rotation, spawningParent);
            case Pigeon.PigeonType.HOMING:
                return Instantiate<HomingPigeon>(homingPigeonTemplate, transform.position, transform.rotation, spawningParent);
        }

        return null;
    }

    private void PigeonReturned(Pigeon pigeon)
    {
        pigeon.gameObject.SetActive(false);
        firedPigeons.Remove(pigeon);
        availablePigeons.Add(pigeon);
        CollisionDetector.Instance.UnRegister(pigeon.transform as RectTransform);
    }

    private void HandleCollisionTriggered(List<CollidableObject> collidables)
    {
        CollidableObject collidable = collidables.Find(x=>x.Type == CollidableObject.ColliderType.Pigeon);

        if (collidable != null)
        {
            Pigeon returnedPigeon = collidable.RectTransform.GetComponent<Pigeon>();
            PigeonReturned(returnedPigeon);
        }
    }

    public List<Image> GetPigeonQueue(out int availableCount)
    {
        var list = new List<Image>();
        availableCount = availablePigeons.Count;

        foreach(var pigeon in availablePigeons)
            list.Add(pigeon.Image);
            
        foreach(var pigeon in firedPigeons)
            list.Add(pigeon.Image);

        return list;
    }

    public void AddBasicPigeon()
    {
        Pigeon pigeon = GetPigeonInstance(Pigeon.PigeonType.BASIC);
        availablePigeons.Add(pigeon);
        pigeon.gameObject.SetActive(false);
    }

    public void UpgradePigeon()
    {
        StartCoroutine(UpgradePigeonCo());
    }

    IEnumerator UpgradePigeonCo()
    {
        while(availablePigeons.Count == 0)
            yield return null;

        for(int i = 0; i < availablePigeons.Count; ++i)
        {
            if(availablePigeons[i].Type == Pigeon.PigeonType.HOMING)
                continue;

            pooledPigeons.Add(availablePigeons[i]);
            availablePigeons.RemoveAt(i);

            Pigeon pigeon = GetPigeonInstance(Pigeon.PigeonType.HOMING);
            availablePigeons.Insert(i, pigeon);
            pigeon.gameObject.SetActive(false);
            yield break;
        }
    }
}
