using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	public class GlossaryCard : MonoBehaviour
	{
		private TMP_Text[] _data;
		private RectTransform _rectTransform;
		private ScrollRect _scrollRect;

		private GameObject _panel;
		
		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_data = GetComponentsInChildren<TMP_Text>();
			_scrollRect = GetComponentInChildren<ScrollRect>();

			_panel = transform.parent.gameObject;
			
			_panel.SetActive(false);
		}

		public void Show(string id)
		{
			foreach (TMP_Text element in _data)
				element.text = ResourceLoader.GetGlossaryText(id);
			_scrollRect.DOVerticalNormalizedPos(1f, 0.75f);

			_rectTransform.localScale = Vector3.zero;
			_panel.SetActive(true);
			_rectTransform.DOScale(Vector3.one, GameManager.Instance.UI.PopUpDuration);
		}

		public void Hide()
		{
			DOTween.Sequence()
				.Append(_rectTransform.DOScale(Vector3.zero, GameManager.Instance.UI.PopUpDuration))
				.AppendCallback(() => _panel.SetActive(false));
		}
	}
}
