using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClearCheck : MonoBehaviour
{
    private ObjectDetectionPad[] detectionPads;
    private bool isCleared = false;
    private void Awake()
    {
        isCleared = false;
        detectionPads = FindObjectsByType<ObjectDetectionPad>(FindObjectsSortMode.None);
    }

    public bool IsStageClear()                                                      
    {
        foreach (var pad in detectionPads)
        {
            if (!pad.isClear)
            {
                return false;
            }
        }
        return true;
    }

    private void Update()
    {
        if (isCleared) return;
        if (IsStageClear())
        {
            isCleared = true;
            AH_StageManager.Instance.ClearStageAndFillLine();
            if (SceneManager.GetActiveScene().buildIndex != 12)
            {
                Debug.Log("Stage Clear!");
                StartCoroutine(ReturnScene());
            }
            else
            {
                SceneChangeManager.Instance.ChangeScene(13);
            }
        }
    }
    public IEnumerator ReturnScene()
    {
        yield return new WaitForSeconds(.7f);
        SceneChangeManager.Instance.ChangeScene("SceneSelect");
    }
}

