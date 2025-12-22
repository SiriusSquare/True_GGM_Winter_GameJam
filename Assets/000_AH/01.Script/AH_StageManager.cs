using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Code.Core;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StageData
{
    public string stageName;
    public GameObject nodeObject;
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

    private class GeneratedLine
    {
        public LineRenderer bg;   
        public LineRenderer fill; 
    }
    private List<GeneratedLine> runtimeLines = new List<GeneratedLine>();
    public int currentStageIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
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
    private void UpdateVisibility()
    {
        bool shouldActive = (SceneManager.GetActiveScene().name == "SceneSelect");

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(shouldActive);
        }
    }
    public string GetCurrentStageName()
    {
        if (currentStageIndex >= 0 && currentStageIndex < stages.Count)
        {
            return stages[currentStageIndex].stageName;
        }

        return "All Clear!";
    }
    private void InitializeMap()
    {
        runtimeLines.Clear();
        
        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].nodeObject != null)
            {
                SetNodeColor(stages[i].nodeObject, (i == 0) ? unlockedNodeColor : lockedNodeColor);
            }

            if (i >= stages.Count - 1) continue;

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
            newLine.fill.SetPosition(1, startPos);

            runtimeLines.Add(newLine);
        }
    }


    public int GetCurrentStageIndex()
    {
        return currentStageIndex;
    }
    public void ClearStageAndFillLine()
    {
        if (currentStageIndex >= runtimeLines.Count) return;

        StartCoroutine(AnimateSliderEffect());
    }
    public bool IsStageLocked(int stageIndex)
    {
        return stageIndex > currentStageIndex;
    }
    public bool IsStageUnlocked(int stageIndex)
    {
        return stageIndex <= currentStageIndex;
    }
    private IEnumerator AnimateSliderEffect()
    {
        LineRenderer targetFillLine = runtimeLines[currentStageIndex].fill;
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

        if (nextNode != null)
        {
            SetNodeColor(nextNode, unlockedNodeColor);
            nextNode.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f);
        }
    }

    private LineRenderer CreateLine(string name, int order, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(this.transform);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = true;
        lr.sortingOrder = order;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ClearStageAndFillLine();
    }
}