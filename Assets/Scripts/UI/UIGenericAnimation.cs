using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class UIGenericAnimation : MonoBehaviour
	{
		private RectTransform _rectTransform;

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		public void ButtonScale()
		{
			DOTween.Sequence()
				.Append(_rectTransform.DOScale(GameManager.Instance.UI.AnimationScale,
					GameManager.Instance.UI.AnimationDuration))
				.Append(_rectTransform.DOScale(1f, GameManager.Instance.UI.AnimationDuration));
		}

		public void ButtonShake()
		{
			_rectTransform.DOShakePosition(GameManager.Instance.UI.AnimationDuration,
					GameManager.Instance.UI.AnimationShakeStrength);
		}
	}
}
