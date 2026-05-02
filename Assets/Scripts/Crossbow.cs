using System.Collections;
using UnityEngine;
using Assets.Scripts.Interactions;

namespace Assets.Scripts.Interactions
{
    public class Crossbow : Gun
    {
        [Header("Crossbow Settings")]
        [Tooltip("Drag the 'Arrow' bone from StylizedCrossbowRig here")]
        [SerializeField] private Transform _arrowBone;

        [Tooltip("How long before a new arrow appears (seconds) - match this to your animation length!")]
        [SerializeField] private float _reloadTime = 1f;

        [Header("Animations")]
        [Tooltip("The animator on the crossbow model")]
        [SerializeField] private Animator _crossbowAnimator;

        private bool _canShoot = true;
        private Vector3 _originalArrowScale;

        private void Start()
        {
            if (_arrowBone != null)
                _originalArrowScale = _arrowBone.localScale;

            if (_crossbowAnimator == null)
                _crossbowAnimator = GetComponent<Animator>();
        }

        public override void Shoot()
        {
            if (!_canShoot) return;

            // 1. Spawn the clone arrow that flies forward
            base.Shoot();

            // 2. Play the Firing animation using your "Fire" trigger!
            if (_crossbowAnimator != null)
            {
                _crossbowAnimator.SetTrigger("Fire");
            }

            // 3. Hide the arrow on the bow
            if (_arrowBone != null)
            {
                _arrowBone.localScale = Vector3.zero;
                _canShoot = false;
                StartCoroutine(ReloadArrowRoutine());
            }
        }

        private IEnumerator ReloadArrowRoutine()
        {
            yield return new WaitForSeconds(_reloadTime);

            // 4. Make the arrow reappear on the bow!
            if (_arrowBone != null)
            {
                _arrowBone.localScale = _originalArrowScale;
            }
            _canShoot = true;
        }
    }
}