using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineScene : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public string CameraTest;

    void Start()
    {
        playableDirector.stopped += OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (playableDirector == aDirector)
            SceneManager.LoadScene(CameraTest);
    }

    void OnDestroy()
    {
        playableDirector.stopped -= OnPlayableDirectorStopped;
    }
}