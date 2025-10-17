using UnityEngine;
using TMPro;
using System.Collections;

public class Duel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Keys (one button per player)")]
    [SerializeField] private KeyCode p1Key = KeyCode.A;     // Player 1 button
    [SerializeField] private KeyCode p2Key = KeyCode.L;     // Player 2 button

    [Header("Timing")]
    [Tooltip("Random delay before GO!")]
    [SerializeField] private float minReadyDelay = 1.5f;
    [SerializeField] private float maxReadyDelay = 4.0f;

    [Tooltip("Time window after GO! where a press is valid")]
    [SerializeField] private float reactionWindow = 1.0f;

    [Header("Scoring")]
    [SerializeField] private bool useScoring = true;
    [SerializeField] private int bestOf = 5; // first to win ceil(bestOf/2) rounds
    private int p1Score = 0, p2Score = 0;
    private int targetWins;

    private enum State { Idle, CountingDown, Go, RoundOver }
    private State state = State.Idle;

    private float goTime;
    private Coroutine roundRoutine;

    void Start()
    {
        targetWins = Mathf.CeilToInt(bestOf / 2f);
        UpdateScoreUI();
        StartRound();
    }

    void Update()
    {
        // Read inputs once per frame to avoid double-reading
        bool p1Down = Input.GetKeyDown(p1Key);
        bool p2Down = Input.GetKeyDown(p2Key);

        switch (state)
        {
            case State.CountingDown:
                // Any press before GO is a foul → that player loses immediately
                if (p1Down || p2Down)
                {
                    if (p1Down && p2Down)
                    {
                        EndRoundDraw("Both players fouled early!");
                    }
                    else if (p1Down)
                    {
                        EndRound(2, "P1 pressed too early!");
                    }
                    else // p2Down
                    {
                        EndRound(1, "P2 pressed too early!");
                    }
                }
                break;

            case State.Go:
                {
                    // First valid press after GO within reactionWindow wins
                    float t = Time.time - goTime;

                    if (t <= reactionWindow)
                    {
                        if (p1Down && p2Down)
                        {
                            // Same frame → treat as tie (or pick a winner if you prefer)
                            EndRoundDraw("Exact same time!");
                        }
                        else if (p1Down)
                        {
                            EndRound(1, "P1 fired first!");
                        }
                        else if (p2Down)
                        {
                            EndRound(2, "P2 fired first!");
                        }
                    }
                    else
                    {
                        // Window expired → nobody pressed in time = both late (lose)
                        EndRoundDraw("Too late! No one fired in time.");
                    }
                    break;
                }

            case State.RoundOver:
                // Press R or Space to start next round
                if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space))
                {
                    StartRound();
                }
                break;
        }
    }

    private void StartRound()
    {
        if (roundRoutine != null) StopCoroutine(roundRoutine);
        roundRoutine = StartCoroutine(RoundFlow());
    }

    private IEnumerator RoundFlow()
    {
        state = State.Idle;
        messageText.text = "Ready…";
        yield return new WaitForSeconds(0.6f);

        state = State.CountingDown;
        float wait = Random.Range(minReadyDelay, maxReadyDelay);
        yield return new WaitForSeconds(wait);

        messageText.text = "<color=#00FF66><b>GO!</b></color>";
        goTime = Time.time;
        state = State.Go;
    }

    private void EndRound(int winner, string reason)
    {
        state = State.RoundOver;

        // Update score
        if (useScoring)
        {
            if (winner == 1) p1Score++;
            else if (winner == 2) p2Score++;
        }

        messageText.text = $"{reason}\n<b>Player {winner} wins the round!</b>\n<alpha=#88>Press Space/R for next round";
        UpdateScoreUI();
        CheckMatchOver();
    }

    private void EndRoundDraw(string reason)
    {
        state = State.RoundOver;
        messageText.text = $"{reason}\n<b>Draw.</b>\n<alpha=#88>Press Space/R for next round";
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (!useScoring)
        {
            scoreText.text = "Quick-Draw: First valid shot after GO wins!";
            return;
        }

        scoreText.text = $"P1 [{KeyToLabel(p1Key)}]: {p1Score}  —  P2 [{KeyToLabel(p2Key)}]: {p2Score}  (First to {targetWins})";
    }

    private string KeyToLabel(KeyCode kc)
    {
        // Short nicer label for UI
        return kc.ToString().Replace("Alpha", "");
    }

    private void CheckMatchOver()
    {
        if (!useScoring) return;

        if (p1Score >= targetWins || p2Score >= targetWins)
        {
            int champ = p1Score > p2Score ? 1 : 2;
            messageText.text = $"<b>Player {champ} wins the match!</b>\n<alpha=#88>Press Space/R to play again";
            // Reset for a new match on next StartRound
            p1Score = 0;
            p2Score = 0;
            UpdateScoreUI();
        }
    }
}
