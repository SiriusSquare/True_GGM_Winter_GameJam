using UnityEngine;

public class StageClearCheck : MonoBehaviour
{
    private ObjectDetectionPad[] detectionPads;
    private bool isCleared = false;
    private void Awake()
    {
        detectionPads = GetComponentsInChildren<ObjectDetectionPad>();
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
        }
    }
}

