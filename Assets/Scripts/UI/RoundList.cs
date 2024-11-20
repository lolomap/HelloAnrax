using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	public abstract class RoundListElement : MonoBehaviour, ISelectable
	{
		public RectTransform RTransform;

		private void Awake()
		{
			RTransform = GetComponent<RectTransform>();
		}

		public abstract void Select();
	}

	[RequireComponent(typeof(RectTransform))]
	public class RoundList : MonoBehaviour
	{
		private RectTransform _rectTransform;
		
		private List<RoundListElement> _elements = new();
		public List<RoundListElement> Elements
		{
			get => _elements;
			set
			{
				_elements = value;
				ResetElementsPosition();
			}
		}

		public bool FitWidth = true;
		public bool FitHeight = false;
		public float Radius = 15f;

		private float _selectionAngle;
		
		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();

			OptionIcon prefab = Resources.Load<OptionIcon>("Prefabs/OptionIcon");
			OptionIcon o1 = Instantiate(prefab);
			o1.Data.Title = "1";
			OptionIcon o2 = Instantiate(prefab);
			o2.Data.Title = "2";
			OptionIcon o3 = Instantiate(prefab);
			o3.Data.Title = "3";
			OptionIcon o4 = Instantiate(prefab);
			o4.Data.Title = "4";
			Elements = new() {o1, o2, o3, o4};
		}

		public void OnDrag()
		{
			if (Input.touches.Length == 0) return;

			Touch touch = Input.touches[0];
			Vector2 velocity = touch.deltaPosition;
			float chord = Vector2.Distance(Vector2.zero, velocity);
			float angle = Mathf.Asin(chord / Radius / 2) * (180 / Mathf.PI) * 2;

			Vector3 direction = velocity.x < 0 ? Vector3.forward : Vector3.back;
			if (Camera.main!.ScreenToWorldPoint(touch.position).y < transform.position.y) direction *= -1f;
			
			foreach (RoundListElement element in _elements)
			{
				element.RTransform.transform.RotateAround(transform.position, direction, angle);

				float elementAngle = Vector2.Angle(element.RTransform.localPosition, Vector2.up);
				if (elementAngle < _selectionAngle)
					element.Select();
			}
		}

		private void ResetElementsPosition()
		{
			float step = 360f / _elements.Count;
			_selectionAngle = step / 2f;

			for (int i = 0; i < _elements.Count; i++)
			{
				RectTransform element = _elements[i].RTransform;
				element.transform.SetParent(transform.parent);
				element.localPosition = Vector3.zero;
				element.localScale = Vector3.one;

				if (FitWidth && FitHeight)
					Radius = _rectTransform.rect.width * 0.25f;
				else if (FitHeight)
					Radius = _rectTransform.rect.height * 0.25f;
				
				element.localPosition += Vector3.up * Radius;
				element.transform.RotateAround(transform.position, Vector3.forward, step * i);
			}
		}
	}
}
