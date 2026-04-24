using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameAssetsControl
{
    [CreateAssetMenu]
    public class GunContainer : ScriptableObject
    {
        [SerializeField] private List<GunItem> _guns;

        public List<GunItem> Guns => _guns;

        [SerializeField] private List<GunItemTypeContainer> _gunItems;
    }
}