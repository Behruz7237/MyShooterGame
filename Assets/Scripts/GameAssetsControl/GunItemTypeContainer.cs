using Assets.Scripts.Interactions;
using System;
using UnityEngine;

namespace Assets.Scripts.GameAssetsControl
{
    [Serializable]
    public class GunItemTypeContainer
    {
        [SerializeField] private GunType _type;
        [SerializeField] private GunItem _item;

        public GunType GunType => _type;
        public GunItem Item => _item;
    }
}