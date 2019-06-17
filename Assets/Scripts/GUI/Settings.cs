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

    /// <summary>
    /// Load the Level_1 scene.
    /// </summary>
    public void Play() => LoadScene("Level_1");

    /// <summary>
    /// Set if the sound is played or muted.
    /// </summary>
    /// <param name="active">If <see langword="true"/> sound will be played. On <see langword="false"/> sound is muted.</param>
    public void SetSound(bool active) => IsSoundActive = active;

    /// <summary>
    /// Close game.
    /// </summary>
    public void Exit() => Application.Quit();

    /// <summary>
    /// Reload the current scene.
    /// </summary>
    public void Restart() => LoadScene(SceneManager.GetActiveScene().name);

    /// <summary>
    /// Load the main menu scene.
    /// </summary>
    public void MainMenu() => LoadScene("Main_Menu");

    /// <summary>
    /// Load an scene.\n
    /// Equivalent to <c><seealso cref="SceneManager.LoadScene"/>(<paramref name="scene"/>, <seealso cref="LoadSceneMode.Single"/>);</c>
    /// </summary>
    /// <seealso cref="SceneManager.LoadScene(string)"/>
    /// <param name="scene">Scene name to load.</param>
    private void LoadScene(string scene) => SceneManager.LoadScene(scene, LoadSceneMode.Single);
}
