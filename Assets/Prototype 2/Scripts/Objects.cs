using UnityEngine;

public class Objects : MonoBehaviour

{
    public GameObject obstaclePrefab;
    public Transform cam;              
    public Vector2 xRange = new Vector2(-4.5f, 4.5f);
    public float spawnInterval = 0.6f;
    public float objspawn = -10f;    // when objects spawn below the camera
    public float spacing = -4f;        // distance between rows 
    public int rowspawner = 1;       
    public float diffTime = 15f;       // increase difficulty

    float timer;
    float lastDiff;
    float Nextrow;

    void Start()
    {
        if (!cam) cam = Camera.main.transform;
        Nextrow = cam.position.y + objspawn;
        lastDiff = Time.time;
    }

    void Update()
    {
        // difficulty increase
        if (Time.time - lastDiff > diffTime)
        {
            rowspawner = Mathf.Min(rowspawner + 1, 4);
            lastDiff = Time.time;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;

            // keep spawn line below the camera
            float minStart = cam.position.y + objspawn;
            if (Nextrow > minStart)
                Nextrow = minStart;

            for (int r = 0; r < rowspawner; r++)
            {
                SpawnRow(Nextrow);
                Nextrow += spacing; 
            }
        }
    }
    // respwning objects 
    void SpawnRow(float y)
    {
        int count = Random.Range(1, 4); 
        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(xRange.x, xRange.y);
            Instantiate(obstaclePrefab, new Vector3(x, y, 0f), Quaternion.identity);
        }
    }
}