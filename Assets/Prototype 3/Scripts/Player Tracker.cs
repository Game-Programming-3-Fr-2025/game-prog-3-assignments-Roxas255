using UnityEngine;
using TMPro;

public class PlayerTracker : MonoBehaviour
{
    public enum TrackMode { Closest, Farthest }
    public TrackMode mode = TrackMode.Closest;

    [Header("References")]
    public Transform arrowVisual;
    public TMP_Text distanceText;

    [Header("Settings")]
    public float arrowDistanceFromPlayer = 0.7f;
    public float refreshRate = 0.2f;

    private Humans currentTarget;
    private float nextRefresh;

    void Update()
    {
        if (Time.time >= nextRefresh)
        {
            nextRefresh = Time.time + refreshRate;
            currentTarget = FindTarget();
        }

        UpdateArrowAndText();
    }

    Humans FindTarget()
    {
        Humans[] all = FindObjectsByType<Humans>(FindObjectsSortMode.None);

        Humans best = null;
        float bestMetric = (mode == TrackMode.Closest)
            ? float.MaxValue
            : float.MinValue;

        foreach (var h in all)
        {
            if (!h) continue; 
            if (h.Current != Humans.State.Healthy) continue;

            float d = ((Vector2)h.transform.position - (Vector2)transform.position).sqrMagnitude;

            if (mode == TrackMode.Closest)
            {
                if (d < bestMetric) { bestMetric = d; best = h; }
            }
            else
            {
                if (d > bestMetric) { bestMetric = d; best = h; }
            }
        }

        return best;
    }

    void UpdateArrowAndText()
    {
        if (!currentTarget)
        {
            if (arrowVisual) arrowVisual.gameObject.SetActive(false);
            if (distanceText) distanceText.gameObject.SetActive(false);
            return;
        }

        Vector2 playerPos = transform.position;
        Vector2 targetPos = currentTarget.transform.position;
        Vector2 dir = (targetPos - playerPos).normalized;

        if (arrowVisual)
        {
            arrowVisual.gameObject.SetActive(true);
            arrowVisual.position = playerPos + dir * arrowDistanceFromPlayer;
            arrowVisual.up = dir; 
        }

        if (distanceText)
        {
            distanceText.gameObject.SetActive(true);
            float dist = Vector2.Distance(playerPos, targetPos);
            distanceText.text = dist.ToString("0.0") + "m";
        }
    }
}