using UnityEngine;

public class MobileWebGLSettings : MonoBehaviour
{
    private void Start()
    {

#if UNITY_WEBGL && !UNITY_EDITOR
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        SetupWebGLMobile();
#endif
    }



    private void SetupWebGLMobile()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        RequestFullscreen();
    }

    private void RequestFullscreen()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Inject JavaScript to handle fullscreen
        Application.ExternalEval(@"
            if (window.mobileAndTabletCheck) {
                document.addEventListener('click', function() {
                    if (!document.fullscreenElement) {
                        document.documentElement.requestFullscreen().catch(err => {});
                    }
                });
                
                // Auto-attempt fullscreen after delay
                setTimeout(function() {
                    document.documentElement.requestFullscreen().catch(err => {});
                }, 2000);
            }
        ");
#endif
    }

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // Escape key handler for testing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.ExternalEval("if(document.fullscreenElement) document.exitFullscreen();");
        }
#endif
    }
}
