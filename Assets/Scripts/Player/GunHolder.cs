using Assets.Scripts.GameAssetsControl;
using Assets.Scripts.Interactions;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class GunHolder : MonoBehaviour
    {
        [SerializeField] private Transform _gunHoldingPoint;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Animator _animator;

        private Gun _currentGun = null;
        private List<Gun> _instantiatedGuns = new List<Gun>();
        private int _currentGunID = 0;

        private List<GunItem> _availableGuns = new List<GunItem>();

        public event Action<int> OnGunChanged;
        private Tweener _waitTweener;

        private void Awake()
        {
            var gunContainer = Resources.Load<GunContainer>(GameConstants.GunHolderResource);
            foreach (var gun in gunContainer.Guns)
            {
                _availableGuns.Add(gun);
            }
        }

        public void Shot()
        {
            if (_currentGun == null) return;
            _currentGun.Shoot();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                if (_currentGun == null)
                {
                    ChangeGunCompletely(() =>
                        {
                            CreateAGun(_availableGuns[_currentGunID]);
                        });

                }
                else if (_instantiatedGuns.Count < _availableGuns.Count)
                {
                    ChangeGunCompletely(
                        () =>
                        {
                            HideTheGun();
                            _currentGunID++;
                            CreateAGun(_availableGuns[_currentGunID]);
                        });
                }
                else
                {
                    ChangeGunCompletely(SwitchGuns);
                }
        }

        private void CreateAGun(GunItem gun)
        {
            var item = Instantiate(gun.GunPrefab, _gunHoldingPoint.position, _gunHoldingPoint.rotation, _gunHoldingPoint);
            item.Init(_cameraTransform, gun.Voltage);
            _currentGun = item;
            _instantiatedGuns.Add(item);
            GunChanged(gun);
        }

        private void HideTheGun()
        {
            _currentGun.gameObject.SetActive(false);
        }

        private void SwitchGuns()
        {
            HideTheGun();
            _currentGunID++;
            if (_currentGunID >= _instantiatedGuns.Count) _currentGunID = 0;
            _currentGun = _instantiatedGuns[_currentGunID];
            _currentGun.gameObject.SetActive(true);
        }

        private void GunChanged(GunItem gun)
        {
            OnGunChanged?.Invoke(gun.FireRate);
        }

        private void ChangeGunCompletely(Action changingAction)
        {
            _animator.SetTrigger(GameConstants.GunHideTrigger);
            _waitTweener?.Kill();
            _waitTweener = transform.DOMoveX(0.1f, 0.5f).OnComplete(() =>
            {
                changingAction?.Invoke();
                _animator.SetTrigger(GameConstants.GunShowTrigger);
            });
        }
    }
}