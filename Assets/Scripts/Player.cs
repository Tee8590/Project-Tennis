using UnityEngine;

public class Player : MonoBehaviour
{
    //private int Colliders = 10;
    private GameObject ball;
    public float moveSpeed = 20.0f;
    //[SerializeField]
    //private BoxCollider HItboxCollider;
    private Rigidbody ballRb;
    [SerializeField]
    private BallHitDetection ballHitDetection;
    public Vector3 predictedBallPosition;
    void Start()
    {
       
    }
    void FixedUpdate()
    {

        if (Ball.Instance != null)
        {
            ball = Ball.Instance.ballRb.gameObject;
            ballRb = Ball.Instance.ballRb;
        }
        else Debug.LogError("Null Ball.Instance");

        StartFollowTheBall();
        
    }
    public void StartFollowTheBall()
    {
      if(ballRb != null)  
      {  
       Vector3 ballDirection = ballRb.linearVelocity.normalized;
        Vector3 directionToPlayer = (transform.position - ball.transform.position).normalized;
        float dot = Vector3.Dot(ballDirection, directionToPlayer);

            ////Ball is moving toward the player
            if (dot > 0)
            {
                PlayerMovement();
                //Debug.Log("Ball is moving toward the player");
            }
            else if (dot < 0)
            {
               // Debug.Log("Ball is moving away from the player");
            }
        }
    }
    void PlayerMovement()
    {
        if(!GameManager.Instance.isBallInPlay) return;

        predictedBallPosition = Ball.Instance.landingPos;
        if(transform.gameObject.name == "Player1") predictedBallPosition.z /= 6;

        if (IsTargetInRange(predictedBallPosition) & ball != null)
        {
            Vector3 currentPosition = transform.position;
            float targetZ = predictedBallPosition.z-2f;

           targetZ = Mathf.Lerp(currentPosition.z, targetZ, moveSpeed * Time.deltaTime);
            float targetX = Mathf.Lerp(currentPosition.x, ball.transform.position.x, moveSpeed * Time.deltaTime);
            transform.position = new Vector3(targetX, currentPosition.y, targetZ);
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


}
