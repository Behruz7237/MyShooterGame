using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Assets.Scripts.EnemyActions
{
	public class EnemyController : MonoBehaviour
	{
		private Tweener _tweener;

        private void Start()
        {
            _tweener?.Kill();
            _tweener = transform.DOMoveX(transform.position.x + 25, 10);
        }
    }
}