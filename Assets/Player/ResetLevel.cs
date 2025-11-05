using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour
{

    public bool resetOff = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !resetOff)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (transform.position.y < -6f && !resetOff)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /*
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snake") && !resetOff)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    */
}
