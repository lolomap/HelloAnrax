using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	[RequireComponent(typeof(TMP_Text))]
	public class GlossaryHandler : MonoBehaviour, IPointerClickHandler
	{
		private TMP_Text _text;

		public GlossaryCard Glossary;

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
			
			
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			int linkIndex =
				TMP_TextUtilities.FindIntersectingLink(_text, Input.GetTouch(0).position, Camera.main);

			if (linkIndex > -1)
				Glossary.Show(_text.textInfo.linkInfo[linkIndex].GetLinkID());
		}
	}
}