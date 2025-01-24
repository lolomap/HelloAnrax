using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	public class OptionIcon : RoundListElement
	{
		private Image _sprite;

		private readonly Color _disabledTint = new(1f, 1f, 1f, 0.5f);

		private Option _data;
		public Option Data
		{
			get => _data;
			set
			{
				_data = value;
				_sprite.sprite = ResourceLoader.GetResource<Sprite>("Icons/Options/" + value.Category);
			}
		}

		public delegate void SelectOptionEventHandler(Option data);
		public static event SelectOptionEventHandler SelectOption;

		private void Awake()
		{
			_sprite = GetComponent<Image>();
			_sprite.color = _disabledTint;
		}

		protected override void SelectEffect()
		{
			_sprite.color = Color.white;
			
			SelectOption?.Invoke(Data);
		}

		protected override void UnselectEffect()
		{
			_sprite.color = _disabledTint;
		}
	}
}
