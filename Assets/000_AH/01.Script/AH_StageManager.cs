using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Code.Core;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class StageData
{
    public string stageName;
    public GameObject nodeObject;

    [Header("Popup Info")]
    public List<Info> infos = new();
}

public class AH_StageManager : MonoSingleton<AH_StageManager>
{
    [Header("StageList")]
    [field: SerializeField] public List<StageData> stages { get; set; }

    [Header("Design Setting")]
    [SerializeField] private float lineWidth = 0.5f;
    [SerializeField] private Color bgLineColor = Color.gray;
    [SerializeField] private Color fillLineColor = Color.yellow; 
    [SerializeField] private Material lineMaterial;

    [Header("Animation Setting")]
    [SerializeField] private float fillDuration = 1.0f;
    [SerializeField] private Ease fillEase = Ease.Linear;
    [SerializeField] private Color lockedNodeColor = Color.gray;
    [SerializeField] private Color unlockedNodeColor = Color.white;

    [SerializeField] private TextMeshProUGUI[] tmps;

    private class GeneratedLine
    {
        public LineRenderer bg;   
        public LineRenderer fill; 
    }
    private List<GeneratedLine> runtimeLines = new List<GeneratedLine>();
    [field: SerializeField, _JJM.Script.CustomEditor.ReadOnly] public int currentStageIndex { get; private set; } = 0;

    public int selectedStageIndex { get; private set; } = 0;

    private const string Key = "CurrentIndex";
    private const string SelectSceneName = "SceneSelect";
    private bool isClearAnimationPending = false;
    private bool IsSelectScene => SceneManager.GetActiveScene().name == SelectSceneName;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        currentStageIndex = PlayerPrefs.GetInt(Key, 0);
        selectedStageIndex = currentStageIndex;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerPrefs.SetInt(Key, currentStageIndex);
        ClearMapVisuals();

        if (IsSelectScene)
        {
            InitializeMap();
            if (isClearAnimationPending)
            {
                StartCoroutine(AnimatePendingClearEffect());
            }
        }

        UpdateVisibility();
    }

    private void Start()
    {
        if (lineMaterial == null)
            lineMaterial = new Material(Shader.Find("Sprites/Default"));

        if (stages != null)
        {
            InitializeMap();
        }
        UpdateVisibility();
    }
    private void ClearMapVisuals()
    {
        foreach (var line in runtimeLines)
        {
            if (line.bg != null) Destroy(line.bg.gameObject);
            if (line.fill != null) Destroy(line.fill.gameObject);
        }
        runtimeLines.Clear();
    }
    private void UpdateVisibility()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(IsSelectScene);
        }
    }
    public StageData GetStageData(int index)
    {
        if (stages != null && index >= 0 && index < stages.Count)
        {
            return stages[index];
        }
        return null;
    }
    public StageData GetCurrentStageData()
    {
        return GetStageData(currentStageIndex);
    }
    public string GetCurrentStageName()
    {
        if (selectedStageIndex >= 0 && selectedStageIndex < stages.Count)
        {
            return stages[selectedStageIndex].stageName;
        }

        return "Unknown Stage";
    }
    private void InitializeMap()
    {
        if (!IsSelectScene) return;
        ClearMapVisuals();

        int visualIndex = isClearAnimationPending ? currentStageIndex - 1 : currentStageIndex;
        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].nodeObject != null)
            {
                SetNodeColor(stages[i].nodeObject,
                    (i <= currentStageIndex) ? unlockedNodeColor : lockedNodeColor);
            }

            if (i >= stages.Count - 1) continue;

            if (stages[i].nodeObject == null || stages[i + 1].nodeObject == null) continue;

            Vector3 startPos = stages[i].nodeObject.transform.position;
            Vector3 endPos = stages[i + 1].nodeObject.transform.position;

            GeneratedLine newLine = new GeneratedLine();

            newLine.bg = CreateLine($"Line_BG_{i}", 0, bgLineColor);
            newLine.bg.positionCount = 2;
            newLine.bg.SetPosition(0, startPos);
            newLine.bg.SetPosition(1, endPos);

            newLine.fill = CreateLine($"Line_Fill_{i}", 1, fillLineColor);
            newLine.fill.positionCount = 2;
            newLine.fill.SetPosition(0, startPos);

            if (i < visualIndex)
            {
                newLine.fill.SetPosition(1, endPos);
            }
            else
            {
                newLine.fill.SetPosition(1, startPos);
            }

            runtimeLines.Add(newLine);
        }
    }

    public void ClearStageAndFillLine()
    {
        if (selectedStageIndex < currentStageIndex)
        {
            Debug.Log("이미 클리어한 스테이지입니다. 진척도는 오르지 않습니다.");
            return;
        }

        if (currentStageIndex >= stages.Count) return;

        if (IsSelectScene)
        {
            if (currentStageIndex < runtimeLines.Count)
            {
                StartCoroutine(AnimateSliderEffectImmediate());
            }
            else
            {
                SaveProgressOnly();
            }
        }
        else
        {
            SaveProgressOnly();
            isClearAnimationPending = true;
        }
    }
    private void SaveProgressOnly()
    {
        currentStageIndex++;
        PlayerPrefs.SetInt(Key, currentStageIndex);
        PlayerPrefs.Save();

        selectedStageIndex = currentStageIndex;
    }
    public void SelectStage(int index)
    {
        if (index >= 0 && index < stages.Count)
        {
            selectedStageIndex = index;
            Debug.Log($"스테이지 선택됨: {stages[index].stageName} (Index: {index})");
        }
        else
        {
            Debug.LogError($"잘못된 스테이지 번호입니다: {index}");
        }
    }
    private IEnumerator AnimatePendingClearEffect()
    {
        int targetLineIndex = currentStageIndex - 1;

        if (targetLineIndex < 0 || targetLineIndex >= runtimeLines.Count)
        {
            isClearAnimationPending = false;
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        LineRenderer targetFillLine = runtimeLines[targetLineIndex].fill;
        GameObject nextNode = stages[targetLineIndex + 1].nodeObject;
        Vector3 endPos = nextNode.transform.position;

        yield return DOTween.To(
            () => targetFillLine.GetPosition(1),
            (pos) => targetFillLine.SetPosition(1, pos),
            endPos,
            fillDuration
        ).SetEase(fillEase).WaitForCompletion();

        targetFillLine.SetPosition(1, endPos);

        if (nextNode != null)
        {
            SetNodeColor(nextNode, unlockedNodeColor);
            nextNode.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f);
        }

        isClearAnimationPending = false;
    }

    private IEnumerator AnimateSliderEffectImmediate()
    {
        if (currentStageIndex >= runtimeLines.Count) yield break;

        LineRenderer targetFillLine = runtimeLines[currentStageIndex].fill;
        if (currentStageIndex + 1 >= stages.Count) yield break;

        GameObject nextNode = stages[currentStageIndex + 1].nodeObject;
        Vector3 endPos = nextNode.transform.position;

        yield return DOTween.To(
            () => targetFillLine.GetPosition(1),
            (pos) => targetFillLine.SetPosition(1, pos),
            endPos,
            fillDuration
        ).SetEase(fillEase).WaitForCompletion();

        targetFillLine.SetPosition(1, endPos);

        currentStageIndex++;
        PlayerPrefs.SetInt(Key, currentStageIndex);
        PlayerPrefs.Save();
        selectedStageIndex = currentStageIndex; 
        if (nextNode != null)
        {
            SetNodeColor(nextNode, unlockedNodeColor);
            nextNode.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f);
        }
    }
    public int GetCurrentStageIndex() => currentStageIndex;
    public bool IsStageLocked(int stageIndex)
    {
        return stageIndex > currentStageIndex;
    }
    public bool IsStageUnlocked(int stageIndex)
    {
        return stageIndex <= currentStageIndex;
    }
    /*private IEnumerator AnimateSliderEffect()
    {
        if (currentStageIndex >= runtimeLines.Count) yield break;

        LineRenderer targetFillLine = runtimeLines[currentStageIndex].fill;

        if (currentStageIndex + 1 >= stages.Count) yield break;
        if (stages[currentStageIndex + 1].nodeObject == null) yield break;

        GameObject nextNode = stages[currentStageIndex + 1].nodeObject;
        Vector3 endPos = nextNode.transform.position;

        yield return DOTween.To(
            () => targetFillLine.GetPosition(1),
            (pos) => targetFillLine.SetPosition(1, pos),
            endPos,
            fillDuration
        ).SetEase(fillEase).WaitForCompletion();

        targetFillLine.SetPosition(1, endPos);

        SaveProgressOnly();

        if (nextNode != null)
        {
            SetNodeColor(nextNode, unlockedNodeColor);
            nextNode.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f);
        }
    }*/

    private LineRenderer CreateLine(string name, int order, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(this.transform);
        //go.layer = gameObject.layer;
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;
        lr.sortingOrder = order;
        lr.sortingLayerName = "Ground";
        return lr;
    }

    private void SetNodeColor(GameObject node, Color color)
    {
        var sprite = node.GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = color;
        else
        {
            var mesh = node.GetComponent<Renderer>();
            if (mesh != null) mesh.material.color = color;
        }
    }
    private void OnValidate()
    {
        if (tmps != null && stages != null)
        {
            for(int i = 0; i < tmps.Length; i++)
            {
                if (stages[i].stageName == null) continue;
                if (i < stages.Count && tmps[i] != null)
                {
                    tmps[i].text = stages[i].stageName;
                }
            }
        }
    }
    [ContextMenu("ResetProgress")]
    public void ResetProgress()
    {
        currentStageIndex = 0;
        isClearAnimationPending = false;
        PlayerPrefs.SetInt(Key, 0);
        PlayerPrefs.Save();
        if (IsSelectScene) InitializeMap();
    }
    [ContextMenu("ASD")]
    public void Test()
    {
        ClearStageAndFillLine();
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (IsSelectScene && Input.GetKeyDown(KeyCode.Space)) ClearStageAndFillLine();
#endif
    }
}