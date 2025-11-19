using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneStateManager : MonoBehaviour
{
    private void Awake()
    {
        SetNightStateForCurrentScene();
    }
    private void SetNightStateForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Night TEST")
        {
            // GameService.ActualState = GameService.State.NightScene;
        }
    }
}
