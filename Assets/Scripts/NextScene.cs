using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            int nextScene = currentScene + 1;

            if (nextScene >= SceneManager.sceneCountInBuildSettings)
            {
                nextScene = 0;
            }

            SceneManager.LoadScene(nextScene);
        }
    }
}
