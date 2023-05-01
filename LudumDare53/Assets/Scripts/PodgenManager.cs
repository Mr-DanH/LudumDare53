using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodgenManager : MonoBehaviour
{
    [SerializeField] private Podgen template;
    [SerializeField] private RectTransform explodeVfx;

    List<Podgen> podgens = new List<Podgen>();

    void Awake()
    {
        CollisionDetector.Instance.OnCollisionTriggered += CollisionHandled;
    }

    public void CreatePodgen()
    {
        Podgen clone = Instantiate<Podgen>(template, transform);
        clone.Launch();
        podgens.Add(clone);
        CollisionDetector.Instance.Register(CollidableObject.ColliderType.Podgen, clone.Collidable);
    }

    public void Tick()
    {
        podgens.ForEach(x=>x.Tick());
    }

    public void Restart()
    {
        podgens.ForEach(x=>Destroy(x.gameObject));
        podgens.Clear();
    }

    public void ReLaunchAll()
    {
        podgens.ForEach(x=>
        {
           if( x.Status == Podgen.State.Resting)
           {
                x.Launch();
                CollisionDetector.Instance.Register(CollidableObject.ColliderType.Podgen, x.Collidable);
           }
        });
    }

    private void CollisionHandled(List<CollidableObject> collidables)
    {
        if (collidables.Exists(x=>x.Type == CollidableObject.ColliderType.Podgen))
        {
            explodeVfx.gameObject.SetActive(false);
            var other = collidables.Find(x => x.Type != CollidableObject.ColliderType.Podgen);
            explodeVfx.transform.position = other.RectTransform.position;
            explodeVfx.gameObject.SetActive(true);

            CollidableObject podgenCollidable = collidables.Find(x=>x.Type == CollidableObject.ColliderType.Podgen);
            CollisionDetector.Instance.UnRegister(podgenCollidable);
            Podgen podgen = podgens.Find(x=>x.Collidable == podgenCollidable.RectTransform);
            podgen.Injured();
        }
    }
}
