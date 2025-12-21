using UnityEngine;


namespace Code.Core.Pooling
{
    [CreateAssetMenu(fileName = "PoolSetupListSO", menuName = "Scriptable Object/Pool/Setup ListSO", order = 1)]
    public class PoolSetupListSO : ScriptableObject
    {

        public PoolItem[] poolList;
    }
}
