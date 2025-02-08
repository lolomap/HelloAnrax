using TMPro;
using UnityEngine;
using DG.Tweening;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	public class GlossaryCard : MonoBehaviour
	{
		private TMP_Text _data;
		private RectTransform _rectTransform;
		
		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_data = GetComponentInChildren<TMP_Text>();
			
			gameObject.SetActive(false);
		}

		public void Show(string text)
		{
			_data.text = text;

			_rectTransform.localScale = Vector3.zero;
			gameObject.SetActive(true);
			_rectTransform.DOScale(Vector3.one, GameManager.Instance.UI.PopUpDuration);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}
