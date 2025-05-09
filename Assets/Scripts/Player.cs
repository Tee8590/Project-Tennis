using UnityEngine;

public class Player : MonoBehaviour
{
    //private int Colliders = 10;
    private GameObject ball;
    public float moveSpeed = 20.0f;
    [SerializeField]
    private BoxCollider HItboxCollider;
    void Start()
    {
       
    }
    void FixedUpdate()
    {
        PlayerMovement();
    }
    void PlayerMovement()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball != null)
        {
            Vector3 currentPosition = transform.position;
            float targetX = Mathf.Lerp(currentPosition.x, ball.transform.position.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(targetX, currentPosition.y, currentPosition.z);
        }
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
