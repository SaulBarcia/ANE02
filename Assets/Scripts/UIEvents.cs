using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIEvents : MonoBehaviour
{
    public PlayerControllerNewInput pc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void Resume()
    {
        pc.isPaused = false;
    }
  public void Restart()
    {
        SceneManager.LoadScene("Scene1");
        pc.isPaused = false;
    }
    public void Exit()
    {
        Application.Quit();
        pc.isPaused = false;
    }
}
