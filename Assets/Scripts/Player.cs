
using UnityEngine;

public class Player : MonoBehaviour
{
    //private int Colliders = 10;
    private GameObject ball;
    public float moveSpeed = 20.0f;
    [SerializeField]
    //private BoxCollider HItboxCollider;
    private Rigidbody ballRb;
    [SerializeField]
    private BallHitDetection ballHitDetection;
    public Vector3 predictedBallPosition;
    private void OnEnable()
    {
        Ball.OnBallLandingPoint += PlayerMovement;
        BallHitDetection.OnPlayer1Hit += Player1Movement;
    }
    private void OnDisable()
    {
        Ball.OnBallLandingPoint -= PlayerMovement;
        BallHitDetection.OnPlayer1Hit -= Player1Movement;
    }
    void Start()
    {
       
    }
    void FixedUpdate()
    {
       if( ball = GameObject.FindGameObjectWithTag("Ball"))
              ballRb = ball.GetComponent<Rigidbody>();
        
        StartFollowTheBall(predictedBallPosition);
        
    }
    public void Player1Movement(Collider collider)
    {
       ball = collider.gameObject;
        //predictedBallPosition = target;
        if (ball == null) Debug.Log("ball is null");
        if (ball != null)
        {

            // if (target.x >= 289f && target.x <= 304f && target.z >= 8f && target.z <= 20f)
            //if (IsTargetInRange(target))
            {
                Debug.Log("ball not null22" + ball);
                Vector3 currentPosition = transform.position;
                //float targetZ = target.z;


                //targetZ = Mathf.Lerp(currentPosition.z, targetZ, moveSpeed * Time.deltaTime);
                float targetX = Mathf.Lerp(currentPosition.x, ball.transform.position.x, moveSpeed * Time.deltaTime);

                transform.position = new Vector3(targetX, currentPosition.y, currentPosition.z);
            }
        }

    }
    public void PlayerMovement(Vector3 target)
    {
        predictedBallPosition = target;
        if (ball == null) Debug.Log("ball is null");
        if (ball != null)
        {
            
           // if (target.x >= 289f && target.x <= 304f && target.z >= 8f && target.z <= 20f)
            if (IsTargetInRange(target))
            {
                Debug.Log("ball not null22" + ball);
                Vector3 currentPosition = transform.position;
            float targetZ = target.z;

            
                targetZ = Mathf.Lerp(currentPosition.z, targetZ, moveSpeed * Time.deltaTime);
            float targetX = Mathf.Lerp(currentPosition.x, ball.transform.position.x, moveSpeed * Time.deltaTime);
            
            transform.position = new Vector3(targetX, currentPosition.y, targetZ);
            }
        }
        
   }
    public bool IsTargetInRange(Vector3 target)
    {
        Vector3 center = transform.position;

        float minX = center.x - 10f;
        float maxX = center.x + 10f;
        float minZ = center.z - 10f;
        float maxZ = center.z + 10f;

        if (target.x >= minX && target.x <= maxX &&
                target.z >= minZ && target.z <= maxZ)
            return true;
        return false;
    }
    public void StartFollowTheBall(Vector3 target)
    {
        
        if (ball == null) Debug.Log("ball is null");
        if (ballRb != null)  
      {
            Debug.Log("ball not null11"+ ball);
            //// Get velocity direction
            Vector3 ballDirection = ballRb.linearVelocity.normalized;
        //// Calculate direction FROM BALL TO PLAYER(collider)
        Vector3 directionToPlayer = (transform.position - ball.transform.position).normalized;
        ////  directions
        float dot = Vector3.Dot(ballDirection, directionToPlayer);

            ////Ball is moving toward the player
            if (dot > 0)
            {
                PlayerMovement(predictedBallPosition);
                //Debug.Log("Ball is moving toward the player");
            }
            else if (dot < 0)
            {
               // Debug.Log("Ball is moving away from the player");
            }
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
