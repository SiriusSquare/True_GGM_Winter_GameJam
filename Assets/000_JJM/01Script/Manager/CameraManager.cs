using _JJM.Script.CustomEditor;
using Code.Core;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoSingleton<CameraManager>
{
    [field: SerializeField] public CinemachineCamera Cinemachine { get; private set; }
    [field: SerializeField, ReadOnly] public CinemachineImpulseSource CinemachineImpulse { get; private set; }


    protected override void Awake()
    {
        CinemachineImpulse = GetComponent<CinemachineImpulseSource>();
    }

    public void CameraShaking()
    {
        CinemachineImpulse.GenerateImpulse();
    }
}
