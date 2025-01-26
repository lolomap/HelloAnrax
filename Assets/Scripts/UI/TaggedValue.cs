using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class TaggedValue : MonoBehaviour
	{
		private delegate void UpdateEventHandler(string uiTag, object value);

		private static event UpdateEventHandler UpdateUI;
		
		public string Tag;
		
		//private Slider _slider;
		private AnimatedBar _slider;
		private SegmentBar _segmentBar;
		private TMP_Text _text;
		
		private void Awake()
		{
			UpdateUI += OnUpdate;

			_slider = GetComponent<AnimatedBar>();
			_segmentBar = GetComponent<SegmentBar>();
			_text = GetComponent<TMP_Text>();
		}

		private void OnUpdate(string uiTag, object value)
		{
			if (uiTag != Tag)
				return;
			
			switch (value)
			{
				case int:
					if (_segmentBar != null) _segmentBar.Set(Convert.ToInt32(value));
					break;
				
				case decimal:
				case float:
					if (_slider != null) _slider.Set(Convert.ToSingle(value));
					if (_segmentBar != null) _segmentBar.Set(Convert.ToInt32(value));
					break;
			
				case string:
					if (_text != null) _text.text = value.ToString();
					break;
			}
		}

		public static void UpdateAll(string uiTag, object value)
		{
			UpdateUI?.Invoke(uiTag, value);
		}
	}
}
