using UnityEngine;

namespace Assets.Scripts.Interactions
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private BulletController bulletPrefab;
        [SerializeField] private Transform firePoint;
        private Transform cameraTransform;
        private float _bulletVelocity;

        public void Init(Transform cameraTransform, float velocityBullet)
        {
            this.cameraTransform = cameraTransform;
            _bulletVelocity = velocityBullet;
        }

        public void Shoot()
        {
            RaycastHit hit;

            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 50f))
            {
                firePoint.LookAt(hit.point);
            }
            else
            {
                firePoint.LookAt(cameraTransform.position + cameraTransform.forward * 50f);
            }

            var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.Init(_bulletVelocity);
        }
    }
}