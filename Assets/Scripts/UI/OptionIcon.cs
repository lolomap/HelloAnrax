using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	public class OptionIcon : RoundListElement
	{
		private Image _sprite;

		private readonly Color _disabledTint = new Color(1f, 1f, 1f, 0.5f);
		
		public Option Data;
		private bool _selected;

		public delegate void SelectOptionEventHandler(Option data);
		public static event SelectOptionEventHandler SelectOption;

		private void Awake()
		{
			_sprite = GetComponent<Image>();
			_sprite.color = _disabledTint;
		}

		public override void Select()
		{
			if (_selected) return;
			
			_selected = true;
			_sprite.color = Color.white;
			
			SelectOption?.Invoke(Data);
		}

		public override void Unselect()
		{
			if (!_selected) return;
			
			_selected = false;
			_sprite.color = _disabledTint;
		}
	}
}
