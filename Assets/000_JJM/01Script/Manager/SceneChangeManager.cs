using Code.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoSingleton<SceneChangeManager>
{
    public void ChangeScene(int name)
    {
        SceneManager.LoadScene(name);
    }

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SceneChangeEvent()
    {

    }
}
