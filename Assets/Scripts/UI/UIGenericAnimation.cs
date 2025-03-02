using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class UIGenericAnimation : MonoBehaviour
	{
		private RectTransform _rectTransform;
		// You should not play some animations while last is not done bc of relative values
		private bool _isBusy;

		public enum Animation
		{
			ButtonScale,
			ButtonShake
		}

		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		public void Play(Animation anim)
		{
			switch (anim)
			{
				case Animation.ButtonScale:
					ButtonScale();
					break;
				case Animation.ButtonShake:
					ButtonShake();
					break;
			}
		}
		
		public void ButtonScale()
		{
			if (_isBusy) return;

			_isBusy = true;
			DOTween.Sequence()
				.Append(_rectTransform.DOScale(GameManager.Instance.UI.AnimationScale,
					GameManager.Instance.UI.AnimationDuration))
				.Append(_rectTransform.DOScale(1f, GameManager.Instance.UI.AnimationDuration))
				.AppendCallback(() => { _isBusy = false; });
		}

		public void ButtonShake()
		{
			if (_isBusy) return;

			_isBusy = true;
			DOTween.Sequence()
				.Append(_rectTransform.DOShakePosition(GameManager.Instance.UI.AnimationDuration,
                        					GameManager.Instance.UI.AnimationShakeStrength))
				.AppendCallback(() => { _isBusy = false; });
		}
	}
}
