using UnityEngine;
using UnityEngine.SceneManagement;


public class Settings : MonoBehaviour
{
    public static bool IsSoundActive = false;

    /*private void Start()
    {
        // Make a persistent game object through scenes
        DontDestroyOnLoad(gameObject);
    }*/
    public void Play() => SceneManager.LoadScene("Level_1", LoadSceneMode.Single);

    public void SetSound(bool active) => IsSoundActive = active;

    public void Exit() => Application.Quit();
}
