using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticleDataSO", menuName = "SO/ParticleDataSO")]
public class ParticleDataSO : ScriptableObject
{
    public ParticleSystem[] particleSystem;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (particleSystem == null || particleSystem.Length == 0)
            return;

        var path = Path.Combine(
            Application.dataPath,
            "000_JJM/01Script/ParticleEnum.cs"
        );

        var sb = new StringBuilder();
        sb.AppendLine("public enum ParticleEnum");
        sb.AppendLine("{");

        for (int i = 0; i < particleSystem.Length; i++)
        {
            if (particleSystem[i] == null) continue;
            sb.AppendLine($"    {particleSystem[i].name},");
        }

        sb.AppendLine("}");

        File.WriteAllText(path, sb.ToString());
        AssetDatabase.Refresh();
    }
#endif
}
