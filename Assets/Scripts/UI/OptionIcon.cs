using UnityEngine;

namespace UI
{
	public class OptionIcon : RoundListElement
	{
		public Option Data;
		private bool _selected;
		
		public override void Select()
		{
			if (!_selected)
			{
				_selected = true;
				Debug.LogWarning(Data.Title);
			}
		}
	}
}
