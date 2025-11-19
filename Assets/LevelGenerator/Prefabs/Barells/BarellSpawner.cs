using UnityEngine;

public class BarellSpawner : MonoBehaviour
{
    public GameObject barellPrefab;
    public float spawnInterval = 2.0f;
    public float barellSpeed = 0.1f;

    private float timer = 0.0f;



    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBarell();
            timer = 0.0f;
        }
    }

    void SpawnBarell()
    {
        GameObject barell = Instantiate(barellPrefab, transform.position, Quaternion.identity);
        BarellScript barellScript = barell.GetComponent<BarellScript>();
        barellScript.SetVelocity(barellSpeed);
    }
}
