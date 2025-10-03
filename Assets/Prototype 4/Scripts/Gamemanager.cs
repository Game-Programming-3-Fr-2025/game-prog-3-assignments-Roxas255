using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance { get; private set; }
    [SerializeField] private float reloadDelay = 0f; // 0 = instant restart
    bool reloading;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void GameOver()
    {
        if (reloading) return;
        reloading = true;

        if (reloadDelay <= 0f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else
            StartCoroutine(ReloadAfterDelay());
    }

    IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSecondsRealtime(reloadDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
