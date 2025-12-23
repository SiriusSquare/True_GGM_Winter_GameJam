using UnityEngine;

[CreateAssetMenu(fileName = "InfoSO", menuName = "SO/InfoSO")]
public class InfoSO : ScriptableObject
{
    public string Title;
    [TextArea] public string Description;
    public Sprite Image;
}
