using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	[RequireComponent(typeof(TMP_Text))]
	[RequireComponent(typeof(EventTrigger))]
	public class GlossaryHandler : MonoBehaviour
	{
		private TMP_Text _text;

		public GlossaryCard Glossary;

		private void Awake()
		{
			_text = GetComponent<TMP_Text>();
		}

		public void OnPointerClick()
		{
			int linkIndex =
				TMP_TextUtilities.FindIntersectingLink(_text, Input.GetTouch(0).position, Camera.main);

			string text = linkIndex > -1
				? ResourceLoader.GetGlossaryText(_text.textInfo.linkInfo[linkIndex].GetLinkID())
				: "Нет информации";
			
			Glossary.Show(text);
		}
	}
}