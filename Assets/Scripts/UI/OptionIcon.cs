using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(UIGenericAnimation))]
	public class OptionIcon : RoundListElement
	{
		private Image _sprite;
		private UIGenericAnimation _animation;

		private bool _blocked;

		private readonly Color _selectedTint = Color.white;
		private readonly Color _disabledTint = new(1f, 1f, 1f, 0.5f);
		private readonly Color _blockedTint = new(1f, 0f, 0f, 0.5f);
		private readonly Color _blockedSelectedTint = Color.red;

		private Option _data;
		public Option Data
		{
			get => _data;
			set
			{
				_data = value;
				_sprite.sprite = ResourceLoader.GetResource<Sprite>("Icons/Options/" + value.Category);

				_blocked = !value.IsAvailable();
				if (_blocked)
					_sprite.color = _blockedTint;
			}
		}

		public delegate void SelectOptionEventHandler(Option data);
		public static event SelectOptionEventHandler SelectOption;

		private void Awake()
		{
			_animation = GetComponent<UIGenericAnimation>();
			_sprite = GetComponent<Image>();
			_sprite.color = _blocked ? _blockedTint : _disabledTint;
		}

		public void PlayAnimation()
		{
			if (_blocked)
				_animation.ButtonShake();
		}

		protected override void SelectEffect()
		{
			_sprite.color = _blocked ?  _blockedSelectedTint : _selectedTint;
			
			SelectOption?.Invoke(Data);
		}

		protected override void UnselectEffect()
		{
			_sprite.color = _blocked ? _blockedTint : _disabledTint;
		}
	}
}
