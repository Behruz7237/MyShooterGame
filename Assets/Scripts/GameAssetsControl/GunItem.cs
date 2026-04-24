using Assets.Scripts.Interactions;
using System;
using UnityEngine;

namespace Assets.Scripts.GameAssetsControl
{
    [Serializable]
    public class GunItem
    {
        [SerializeField] private GunType _type;
        [SerializeField] private Gun _gunPrefab;
        [SerializeField] private int _ammo = 30;
        [SerializeField] private int _ammoPackage = 5;
        [SerializeField] private int _fireRate = 1; //ammo count per second
        [SerializeField] private float _velocity = 20; //bullet speed
        [Range(1, 5)] [SerializeField] private int _damageAmount = 1; 
        [SerializeField] private Sprite _aimSprite;

        public GunType Type => _type;
        public Gun GunPrefab => _gunPrefab;
        public int Ammo => _ammo;
        public int AmmoPackage => _ammoPackage;
        public int FireRate => _fireRate;
        public float Voltage => _velocity;

    }
}