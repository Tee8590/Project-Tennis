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
      
    }
    private void OnDisable()
    {
        CollitionDetection.OnZoneHit -= HandleZoneHit;
        BallHitDetection.OnBallHit -= HandleServeRotation;
        SwipeControl.OnSwipe -= HandleSwipesRotation;
        BallHitDetection.OnPlayer2Hit += OpponentPlayer;
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

    void HandleZoneHit(CourtZoneType zoneType, Collision collition)
    {
       // Debug.Log($"Ball hit zone: {zoneType}");
        switch (zoneType)
        {
            case CourtZoneType.LeftServiceBox:
                Debug.Log("LeftServiceBox Valid serve zone");
                // TODO: Check serve legality, update serve count, rotate server if needed
                break;

            case CourtZoneType.Net:
                Debug.Log("Ball hit the net - fault or let");
                // TODO: Apply fault or let logic
                break;

            case CourtZoneType.Baseline:
                Debug.Log("Ball hit baseline - checking if in or out");
                // TODO: Determine if ballPrefab was in bounds
                break;

            case CourtZoneType.Sideline:
                Debug.Log("Ball hit sideline - checking for out call");
                // TODO: Determine if ballPrefab is out
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
                AwardPointToCurrentPlayer();
                SwitchServer();
                // TODO: Check if serve was legal
                break;
            case CourtZoneType.None:
                Debug.Log("None");
                break;
        }
        UpdateUI();
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
    private void SwitchServer()
    {
        isPlayerOneServing = !isPlayerOneServing;
        //Debug.Log($"Server switched. Now it's {(isPlayerOneServing ? "Player1SwingAction 1" : "Player1SwingAction 2")}'s serve.");
    }
    private void UpdateUI()
    {
        scoreText.text = "player1 : " + playerOneScore + "   player2 : " + playerTwoScore;
        // TODO: Implement UI update logic, like setting text fields or scoreboards
    }
   
}
