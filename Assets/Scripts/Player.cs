using UnityEngine;

public class Player : MonoBehaviour
{
    //private int Colliders = 10;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void SwingRange(Vector3 center, float radius)
    {
        int MaxColliders = 10;
        Collider[] hitColliders = new Collider[MaxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(center, radius, hitColliders);
        for (int i = 0; i < numColliders; i++)
        {

        }

    }
}
