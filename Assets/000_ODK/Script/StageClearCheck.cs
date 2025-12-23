using System.Collections;
using UnityEngine;

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
            Debug.Log("Stage Clear!");
            isCleared = true;
            AH_StageManager.Instance.ClearStageAndFillLine();
            StartCoroutine(ReturnScene());
        }
    }
    public IEnumerator ReturnScene()
    {
        yield return new WaitForSeconds(.7f);
        SceneChangeManager.Instance.ChangeScene("SceneSelect");
    }
}

