using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public float gravity = 10f;
    [HideInInspector] public List<Attractable> attractedObjects = new();
    public SphereCollider attractorCollider;

    private void FixedUpdate()
    {
        foreach (var attractable in attractedObjects)
        {
            attractable.Attract(this);
        }
    }
}
