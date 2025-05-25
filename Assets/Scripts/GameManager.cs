using TMPro;
using UnityEngine;

public enum CourtZoneType
{
    None,
    LeftServiceBox,
    RightServiceBox,
    Net,
    Baseline,
    Sideline,
    Backcourt
}
public class GameManager : MonoBehaviour
{
    private int playerOneScore = 0;
    private int playerTwoScore = 0;
    [SerializeField]
    public static bool isPlayerOneServing = true;
    [SerializeField]
    private GameObject plane;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    
    private void OnEnable()
    {
        CollitionDetection.OnZoneHit += HandleZoneHit;
        BallHitDetection.OnBallHit   += HandleServeRotation;
        SwipeControl.OnSwipe         += HandleSwipesRotation;
        BallHitDetection.OnPlayer2Hit += OpponentPlayer;
        //Ball.OnBallLandingPoint += CalculateLandingPoint;


    }
    private void OnDisable()
    {
        CollitionDetection.OnZoneHit -= HandleZoneHit;
        BallHitDetection.OnBallHit -= HandleServeRotation;
        SwipeControl.OnSwipe -= HandleSwipesRotation;
        BallHitDetection.OnPlayer2Hit -= OpponentPlayer;
    }
    private Transform Player1Position;
    private Transform Player2Position;
    private Transform player1BallPosition;
    private Transform player2BallPosition;
    private void Start()
    {
        
    }
    private void HandleSwipesRotation(SwipeControl control)
    {
       
    }
    public void OpponentPlayer(Collider collider)
    {
        Rigidbody ballRb = collider.gameObject.GetComponent<Rigidbody>();
        //Todo Randomize Swing Direction
        Vector3 enemy = new Vector3(-9, 45, -100).normalized;
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.useGravity = true;
        ballRb.AddForce(enemy * 3f, ForceMode.Impulse);
        isPlayerOneServing = false;


    }
   
    private void HandleServeRotation(BallHitDetection detection, Collider collider)
    {
        
        
    }
    private int serveCount = 1;         
    private bool isBallInPlay = false;  
    private bool serverIsRightSide = true;
    void HandleZoneHit(CourtZoneType zoneType, Collision collition)
    {
       // Debug.Log($"Ball hit zone: {zoneType}");
        switch (zoneType)
        {
            case CourtZoneType.LeftServiceBox:
                Debug.Log("LeftServiceBox Valid serve zone");
                // Ball landed in left service box (typically for a serve from right side).
                if (!CheckServeLegality(zoneType))
                {
                    // Serve fault: update serve count and check for double fault.
                    UpdateServeCount();
                    if (serveCount > 2)
                    {
                        // Double fault: opponent wins the point.
                        AwardPointToOpponent();
                        ResetServeCount();
                        SwitchServer();
                    }
                    // If first serve fault, wait for second serve (no point yet).
                }
                else
                {
                    // Valid serve: reset serve count and start rally.
                    ResetServeCount();
                    isBallInPlay = true;
                }
                break;

            case CourtZoneType.Net:
                Debug.Log("Ball hit the net - fault or let");
                // Ball hit the net: fault or point for opponent.
                AwardPointToOpponent();
                ResetServeCount();
                SwitchServer();
                break;

            case CourtZoneType.Baseline:
                Debug.Log("Ball hit baseline - checking if in or out");
                // Ball landed beyond the baseline (out-of-bounds): point to opponent.
                AwardPointToOpponent();
                SwitchServer();
                break;

            case CourtZoneType.Sideline:
                Debug.Log("Ball hit sideline - checking for out call");
                // Ball landed beyond the sidelines (out-of-bounds): point to opponent.
                AwardPointToOpponent();
                SwitchServer();
                break;

            case CourtZoneType.Backcourt:
                Debug.Log("Out of bounds - point to opponent");
                AwardPointToOpponent();
                SwitchServer();
                Debug.Log("Ball hit backcourt - checking for in bounds");
                // TODO: Apply point logic based on rally
                break;

            case CourtZoneType.RightServiceBox:
                Debug.Log("RightServiceBox Valid serve zone");
                // Ball landed in right service box (typically for a serve from left side).
                if (!CheckServeLegality(zoneType))
                {
                    // Serve fault: increment serve count and check for double fault.
                    UpdateServeCount();
                    if (serveCount > 2)
                    {
                        // Double fault: opponent wins the point.
                        AwardPointToOpponent();
                        ResetServeCount();
                        SwitchServer();
                    }
                }
                else
                {
                    // Valid serve: reset serve count and start rally.
                    ResetServeCount();
                    isBallInPlay = true;
                }
                break;
            case CourtZoneType.None:
            default:
                // Ball did not hit any recognized zone: treat as out-of-bounds.
                AwardPointToOpponent();
                SwitchServer();
                break;
        }
        UpdateUI();
    }
    // Stub: Check if the serve landed in the correct service box.
    private bool CheckServeLegality(CourtZoneType zoneType)
    {
        // Example logic: if server is on right side, serve must go to left service box, and vice versa.
        if (serverIsRightSide && zoneType == CourtZoneType.LeftServiceBox) return true;
        if (!serverIsRightSide && zoneType == CourtZoneType.RightServiceBox) return true;
        return false;
    }
    private void SwitchServer()
    {
        serverIsRightSide = !serverIsRightSide;
        isBallInPlay = false;
        Debug.Log("Server switched");
    }
    private void ResetServeCount()
    {
        serveCount = 1;
    }
    private void UpdateServeCount()
    {
        serveCount++;
    }

    void AwardPointToCurrentPlayer()
    {
        if (isPlayerOneServing)
            playerOneScore++;
        else
            playerTwoScore++;

       Debug.Log($"Score - Player1SwingAction 1: {playerOneScore}, Player1SwingAction 2: {playerTwoScore}");
       
    }
    private void AwardPointToOpponent()
    {
        if (isPlayerOneServing)
            playerTwoScore++;
        else
            playerOneScore++;

        //Debug.Log($"Score - Player1SwingAction 1: {playerOneScore}, Player1SwingAction 2: {playerTwoScore}");
    }
    private void UpdateUI()
    {
        scoreText.text = "player1 : " + playerOneScore + "   player2 : " + playerTwoScore;
        // TODO: Implement UI update logic, like setting text fields or scoreboards
    }
   
}
