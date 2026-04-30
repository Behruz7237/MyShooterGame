using UnityEngine;

namespace Assets.Scripts.Interactions
{
    public class Weapon : Gun
    {
        [SerializeField] private Animator _animator;
        public override void Shoot()
        {
            _animator.Play("sword");
        }
    }
}