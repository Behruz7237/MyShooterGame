using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Assets.Scripts.Interactions.Items
{
	public class LootHandler : MonoBehaviour
	{
		[SerializeField] private Transform _item;
        [SerializeField] private GameObject _fullObject;

        private Tweener _rotateTweener;

        private void Start()
        {
            RotateItem();
        }

        private void RotateItem()
        {
            _rotateTweener?.Kill();
            _rotateTweener = _item
            .DORotate(new Vector3(0, 360, 0), 3f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
        }

        public void Destroy()
        {
            Destroy(_fullObject);
        }
    }
}