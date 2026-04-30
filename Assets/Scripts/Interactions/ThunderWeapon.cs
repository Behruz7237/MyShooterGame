using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Interactions
{
    public class ThunderWeapon : Gun
    {
        [SerializeField] private LineRenderer line;
        [SerializeField] private Animator _animator;
        private Vector3 _cachedTarget;
        private Tweener _t;
        public override void Shoot()
        {

            RaycastHit hit;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward,
                out hit, 50f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                _cachedTarget = hit.point;
            }
            else
            {
                _cachedTarget = cameraTransform.position + cameraTransform.forward * 50f;
            }

            // rotate gun immediately toward target
            Vector3 dir = (_cachedTarget - firePoint.position).normalized;
            firePoint.rotation = Quaternion.LookRotation(dir);


            StopCoroutine(WaitCoroutine());
            _animator.Play("torThunder");
            StartCoroutine(WaitCoroutine());
        }

        private IEnumerator WaitCoroutine()
        {
            yield return new WaitForSeconds(0.18f);
            Shot();
        }

        private void Shot()
        {
            Vector3 start = firePoint.position;
            Vector3 endPoint = _cachedTarget;

            var points = GenerateLightning(start, endPoint, 12, 0.5f);

            line.enabled = true;
            line.positionCount = points.Count;
            line.SetPositions(points.ToArray());

            line.transform.DOScale(Vector3.one, 0.56f)
                .OnComplete(() => line.enabled = false);

            // spawn bullet safely
            var bullet = Instantiate(bulletPrefab, endPoint, Quaternion.identity);
            bullet.Init(_bulletVelocity);
        }

        List<Vector3> GenerateLightning(Vector3 start, Vector3 end, int segments, float chaos)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                Vector3 point = Vector3.Lerp(start, end, t);

                // add randomness except endpoints
                if (i != 0 && i != segments - 1)
                {
                    point += Random.insideUnitSphere * chaos;
                }

                points.Add(point);
            }

            return points;
        }

    }
}