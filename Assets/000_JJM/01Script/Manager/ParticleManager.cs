using Code.Core;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParticleManager : MonoSingleton<ParticleManager>
{
    [SerializeField] private ParticleDataSO m_ParticleDataSO;
    [SerializeField] private int _poolCount = 5;

    private Dictionary<ParticleEnum, List<ParticleSystem>> _particlePools = new();

    protected override void Awake()
    {
        GameObject particles = new GameObject("Particles");

        foreach (var i in m_ParticleDataSO.particleSystem)
        {
            if (i == null) continue;

            ParticleEnum key = (ParticleEnum)System.Enum.Parse(typeof(ParticleEnum), i.name);

            if (!_particlePools.ContainsKey(key))
                _particlePools.Add(key, new List<ParticleSystem>());

            for (int j = 0; j < _poolCount; j++)
            {
                ParticleSystem par = Instantiate(i, particles.transform);
                par.name = i.name;
                par.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _particlePools[key].Add(par);
            }
        }
    }

    public void ParticlePlay(ParticleEnum e, Vector2 pos)
    {
        if (!_particlePools.ContainsKey(e)) return;

        var list = _particlePools[e];
        ParticleSystem par = null;

        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].IsAlive(true))
            {
                par = list[i];
                break;
            }
        }

        if (par == null)
            return;

        par.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        par.transform.position = pos;
        par.Play(true);
    }
}
