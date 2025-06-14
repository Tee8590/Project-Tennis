using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeControl : MonoBehaviour
{
    public static event Action<SwipeControl> OnSwipe;
    [SerializeField]
    private InputManager inputManager;

    private Vector2 direction2D;
    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    public Vector3 points;
    public List<Vector3> path = new List<Vector3>();
    /// <summary>
     private int currentPathIndex = 0;
    private float moveSpeed = 14f; // Adjust as needed
    private bool isMoving = false;
    /// </summary>

    private Rigidbody ballrb;
    [SerializeField]
    private float minimumDistance =.2f;
    [SerializeField]
    private float maxTime = 1f;
    [SerializeField]
    private float directionalThreshold = 0.9f;
    [SerializeField]
    private GameObject trail;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private GameObject player;
    private Coroutine coroutine;
    private Coroutine slowBallCoroutine;
    private BallHitDetection ballHitDetection;
    private InputAction fireAction;
    private GameObject createdBallPrefab = null;
    private float swipeTime;

    private Vector3 direction;
    private Vector3 middlePosition;
    private Vector3 velocity;
    private Vector3 landingPos;
    private Vector2 swipStart;
    private Vector2 swipEnd;
    private Vector3 target;
    private float swipeDistance;
    //[SerializeField]
    //private Camera mainCamera;
    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
        BallHitDetection.OnPlayer1Hit += Player1SwingAction;
        fireAction = inputManager.FireAction;
        //  Ball.BallStartAndEndpositions += MakeBallMovement;
        BallHitDetection.OnPlayer2Hit += OpponentPlayerServing;
    }
    private void OnDisable()
    {

        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
        BallHitDetection.OnPlayer1Hit -= Player1SwingAction;
        // Ball.BallStartAndEndpositions -= MakeBallMovement;
        BallHitDetection.OnPlayer2Hit -= OpponentPlayerServing;
    }
    private void Awake()
    {
       ballHitDetection =  player.GetComponentInChildren<BallHitDetection>();

    }
    private void Start()
    {
        CreateBall();
        /* ballPrefab = Ball.Instance.ballObj;*/
    }
    public void Player1SwingAction(Collider collider)
    {
        
        ballrb = collider.GetComponent<Rigidbody>();
    }
   
    private void SwipeStart(Vector2 position, float time)
    {
        startPosition = position;
        startTime = time;
        trail.SetActive(true);
        trail.transform.position = position;
        coroutine =  StartCoroutine(Trail());
    }
    private void Update()
    {
     
         if (fireAction != null && fireAction.WasPressedThisFrame())
        {
           /* CreateBall();*/
            /*//SimulateSwipeRHS();*/
            //OpponentBallDebug();
        }
    }
    private IEnumerator Trail()
    {
        while (true)
        {
            trail.transform.position = inputManager.PrimaryPosition(10f);
            yield return null;
        }
        
    }
    private void SwipeEnd(Vector2 position, float time)
    {
      
        trail.SetActive(false);
        StopCoroutine(coroutine);
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }
    public void OpponentPlayerServing(Collider collider)
    {
        if (Vector2.Distance(swipStart, swipEnd) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
            float swipeTime = (endTime - startTime);
            direction = swipEnd - swipStart;
            direction2D = new Vector2(direction.x, direction.y).normalized;

           
           //  swipeDistance = Vector2.Distance(startPosition, endPosition);

            Ball.Instance.ballRb.useGravity = true;
            velocity = Ball.Instance.CreateBallVelocity(swipStart, direction, swipeTime, swipeDistance);
            landingPos = Ball.Instance.CalculateLandingPoint(swipStart, velocity, 18.24f);
            landingPos.z = landingPos.z / 8;
            Ball.Instance.BallLandingPositionMarker(landingPos);
            MakeBallMovement(Ball.Instance.transform.position,  landingPos);
        }
    }
   
    public bool DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
            float swipeTime = (endTime - startTime);
            swipStart = startPosition;
            swipEnd = endPosition;
            //direction of the ballPrefab in 2D, z is 0 currently
           direction = endPosition - startPosition;
          // direction2D = new Vector2(direction.x, direction.y).normalized;

            swipeDistance = Vector2.Distance(startPosition, endPosition);
            SwipeDirection(direction2D); 

            //ballRb.useGravity = true;
            velocity = Ball.Instance.CreateBallVelocity(startPosition, direction, swipeTime, swipeDistance);
            landingPos = Ball.Instance.CalculateLandingPoint(startPosition, velocity, 18.24f);

            MakeBallMovement(Ball.Instance.transform.position,  landingPos);
           
            return true;
        }
        else {
            Debug.Log("This Cndition is not Met =>> Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maxTime");
            return false; 
        }
    }
    public void MakeBallMovement( Vector3 startPoint,   Vector3 landingPoint)
    {
        /* swipeTime = (endTime - startTime);
        //direction of the ballPrefab in 2D, z is 0 currently
        direction = endPosition - startPosition;*/
      //  direction2D = new Vector2(direction.x, direction.y).normalized;
        if (GameManager.isPlayerOneServing)
        {
            //CreateBall();
            // BallMovement(ballrb, direction, swipeTime);
            DrawQuadraticBezierPoint(startPoint,  landingPoint);
            GameManager.isPlayerOneServing = false;
        }
        if (!GameManager.isPlayerOneServing)
        {
            //CreateBall();
            // BallMovement(ballrb, direction, swipeTime);
            DrawQuadraticBezierPoint(startPoint,  landingPoint);
            GameManager.isPlayerOneServing = true;
        }
       
    }
    //public void SimulateSwipeLHS()
    //{
    //    UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    //    Vector2 simStartSwipe = new Vector2(UnityEngine.Random.Range(291, 294.5f), UnityEngine.Random.Range(11f, 14f));
    //    Vector2 simEndSwipe   = new Vector2(UnityEngine.Random.Range(293, 296.5f), UnityEngine.Random.Range(0.5f, .75f));
    //    float simSwipeLength = UnityEngine.Random.Range(0.12f, 0.16f);

    //    SwipeStart(simStartSwipe, simSwipeLength);
    //    SwipeEnd(simEndSwipe, simSwipeLength);

    //}

    public void DrawQuadraticBezierPoint(Vector3 start, Vector3 end)
    {

       List<Vector3> path = new List<Vector3>();
        int noOfPoints = 40;

        float distance2D = Vector2.Distance(start, end);
        float arcFactor = 0.7f; //  control arc height 
        float swipeDistance = Vector2.Distance(startPosition, endPosition);
       
        middlePosition = (start + end)  / 2f + Vector3.up * swipeDistance * arcFactor;
         Debug.Log("middlePosition" + middlePosition);
        for (int i = 0; i <= noOfPoints; i++)
        {
            float t = i / (float)noOfPoints;

            //Vector3 point
            path.Add(CalculateQuadraticBezierPoint(start, middlePosition, end, t));

        }
        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1], path[i], Color.red, 2f);
        }

        currentPathIndex = 0;
        isMoving = true;
       StartCoroutine(MoveAlongPath(ballPrefab, path, 1.0f));
    }

    public Rigidbody CreateBall()
    {
        if (Ball.Instance == null)
        {
            Debug.Log("ballPrefab is  null");
            ballPrefab.transform.position = new Vector3(player.transform.position.x,
                player.transform.position.y +1f,
                player.transform.position.z + -0.1f);
            ballPrefab = Instantiate(ballPrefab); ballPrefab.name = "Ball";

            ballrb = ballPrefab.GetComponent<Rigidbody>();
            /*BallPhysicsOff();*/
            return ballrb;
        }
        else
        {
            //if(ballrb != null & ballPrefab != null) 
            {
                Debug.Log("ball Instance is not null");
         /*       ballrb.transform.position = new Vector3(player.transform.position.x,
                  player.transform.position.y,
                  player.transform.position.z + 2f);*/
               /* BallPhysicsOff();*/
            }
 
            return ballrb;
        }
    }
    /* public Rigidbody BallPhysicsOff()
     {
         ballrb.linearVelocity = Vector3.zero;
         ballrb.angularVelocity = Vector3.zero;
         ballrb.useGravity = false; return ballrb;
     }*/
    /*private void FixedUpdate()
    {

        if (DetectSwipe() && currentPathIndex >= path.Count)
        {
            Vector3 targetPosition = path[currentPathIndex];
            Vector3 currentPosition = Ball.Instance.ballRb.position;
            Vector3 direction = (targetPosition - currentPosition).normalized;
            float distance = Vector3.Distance(currentPosition, targetPosition);
            // Calculate movement step
            float step = 14f * Time.fixedDeltaTime;
            if (step >= distance)
            {
                // Move directly to the target position and advance to the next point
                Ball.Instance.ballRb.MovePosition(targetPosition);
                currentPathIndex++;

                if (currentPathIndex >= path.Count)
                {
                    isMoving = false;
                    // Optionally, call AddForceAtTheEnd or any other method here
                }
            }
            else
            {
                // Move towards the target position
                Vector3 newPosition = currentPosition + direction * step;
                Ball.Instance.ballRb.MovePosition(newPosition);
            }
        }
    }*/
    /*public void MoveAlongThePath2(GameObject ball, List<Vector3> path, float duration)
    {

        float totalLength = path.Count - 1;
        float elapsed = 0f;
        while (elapsed < duration)
        {

            float t = elapsed / duration * totalLength;
            int i = Mathf.FloorToInt(t);
            float u = t - i;              // local interpolation between path[i] -> path[i+1]

            Rigidbody rb = Ball.Instance.ballRb;
            rb.useGravity = true;
            if (i < path.Count - 1)
                //rb.MovePosition(Vector3.Lerp(path[i], path[i + 1], u));
                target = path[i + 1];
            Vector3 direction = (target - rb.position).normalized;
            rb.AddForce(direction * 2, ForceMode.VelocityChange);

            elapsed += Time.deltaTime;

        }

        AddForceAtTheEnd(path, ball);
    }*/
    public IEnumerator MoveAlongPath(GameObject ball, List<Vector3> path, float duration)
    {
        Rigidbody rb = Ball.Instance.ballRb;
        rb.useGravity = true;

        float totalLength = path.Count - 1;
        float elapsed = 0f;
        while (elapsed < duration)
        {

            float t = elapsed / duration * totalLength;
            int i = Mathf.FloorToInt(t);
            float u = t - i;              // local interpolation between path[i] -> path[i+1]

           
            if (i < path.Count - 1)
                  rb.MovePosition(Vector3.Lerp(path[i], path[i + 1], u));
            //    target = path[i + 1];
            //Vector3 direction = (target - rb.position).normalized;
            //rb.AddForce(direction * 2, ForceMode.VelocityChange);

            elapsed += Time.deltaTime;
            yield return new WaitForFixedUpdate(); ;
        }

        AddForceAtTheEnd(path);

    }
    private Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //B(t) = (1 - t)²P0  +   2(1 - t)tP1        + t²P2 
        //          u               u                 tt
        //          uu * p0  +   u * 2 * t * p1     + tt * p2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

         points = uu * p0;
        points += u * 2 * t * p1;
        points += tt * p2;
        //point.z = point.z * t; //adding the z axis for depth
        return points;
    }
    public void AddForceAtTheEnd(List<Vector3> path)
    {
        Vector3 start = path[path.Count - 10];
        Vector3 end = path[path.Count - 1];
        Vector3 ballPos = Ball.Instance.transform.position;
       Vector3 direction = (end - start);
        //  Vector3 direction = ballPrefab.transform.LookAt(ballPos, Vector3.forward ); //addforce tryed but stil shit

        Ball.Instance.ballRb.WakeUp();
      // Ball.Instance.ballRb.rotation = Quaternion.LookRotation(direction, Vector3.down);
        Debug.Log("Force direction: " + direction);
        Ball.Instance.ballRb.AddForce(direction * 4f, ForceMode.VelocityChange);
        /* Debug.Log("Force direction: " + direction);*/
    }
   /* public void BallMovement(Rigidbody ballRb,Vector3 direction, float swipeTime)
    {
        ballRb = ballrb;
        if (ballRb != null)
        {
            //if(!ballHitDetection == true)
            //    StopCoroutine(slowBallCoroutine);
            // ballRb.gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 2f);
            ballRb.useGravity = true;
            ballRb.gameObject.GetComponent<Ball>().CreateBallVelocity(startPosition, direction, swipeTime);
        }
    }*/
    
    private void SwipeDirection(Vector2 direction)
    {

        if(Vector3.Dot(Vector3.up, direction) > directionalThreshold)
        {
          //  Debug.Log("swipedUp");
        }
        else if(Vector3.Dot(Vector3.down, direction) > directionalThreshold)
        {
            //Debug.Log("swipedDown");
        }
        else if(Vector3.Dot(Vector3.left, direction) > directionalThreshold)
        {
            //Debug.Log("swipedLeft");
        }
        else if(Vector3.Dot(Vector3.right, direction) > directionalThreshold)
        {
            //Debug.Log("swipedRight");
        }
    }
   
    
}

