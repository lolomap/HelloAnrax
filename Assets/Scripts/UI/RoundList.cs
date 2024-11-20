using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	public abstract class RoundListElement : MonoBehaviour, ISelectable
	{
		public RectTransform RTransform;

		public abstract void Select();
		public abstract void Unselect();
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
				foreach (RoundListElement element in _elements)
				{
					Destroy(element.gameObject);
				}
				_elements = value;
				ResetElementsPosition();
				
				if (_elements.Count > 0)
					_elements[0].Select();
			}
		}

		public bool FitWidth;
		public bool FitHeight;
		public float ExtraRadius;

		public float AdjustSpeed;
		
		private float _radius;
		private float _selectionAngle;
		private bool _isAdjusting;
		
		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
		}

		public void OnDrag()
		{
			if (Input.touches.Length == 0) return;
			if (_isAdjusting) return;

			Touch touch = Input.touches[0];
			Vector2 velocity = touch.deltaPosition;
			float chord = Vector2.Distance(Vector2.zero, velocity);
			float angle = Mathf.Asin(chord / _radius / 2) * (180 / Mathf.PI) * 2;

			Vector3 direction = velocity.x < 0 ? Vector3.forward : Vector3.back;
			
			if (Camera.main!.ScreenToWorldPoint(touch.position).y < transform.position.y) direction *= -1f;
			
			foreach (RoundListElement element in _elements)
			{
				element.RTransform.transform.RotateAround(transform.position, direction, angle);
				element.RTransform.transform.eulerAngles = Vector3.zero;

				float elementAngle = Vector2.Angle(element.RTransform.localPosition, Vector2.up);
				
				if (elementAngle > _selectionAngle) continue;
				
				foreach (RoundListElement el in _elements) { el.Unselect(); }
				element.Select();
			}
		}

		public void OnEndDrag()
		{
			for (int i = 0; i < _elements.Count; i++)
			{
				RoundListElement element = _elements[i];
				float elementAngle = Vector2.Angle(element.RTransform.localPosition, Vector2.up);
				if (elementAngle > _selectionAngle) continue;

				_isAdjusting = true;
				StartCoroutine(Adjust(i));
			}
		}

		private IEnumerator Adjust(int id)
		{
			Vector3 direction = Vector3.back;
			if (_elements[id].RTransform.localPosition.x > _rectTransform.localPosition.x)
				direction = Vector3.forward;
			
			while (Vector2.Angle(_elements[id].RTransform.localPosition, Vector2.up) > AdjustSpeed * Time.deltaTime)
			{ 
				foreach (RoundListElement element in _elements)
				{
					element.RTransform.transform.RotateAround(transform.position, direction,
						AdjustSpeed * Time.deltaTime);
					element.RTransform.transform.eulerAngles = Vector3.zero;
				}

				yield return null;
			}

			foreach (RoundListElement element in _elements)
			{
				element.RTransform.transform.RotateAround(transform.position, direction,
					AdjustSpeed * Time.deltaTime);
				element.RTransform.transform.eulerAngles = Vector3.zero;
			}
			_isAdjusting = false;
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

				_radius = ExtraRadius;
				if (FitWidth)
					_radius = (ExtraRadius + _rectTransform.rect.width) * 0.25f;
				else if (FitHeight)
					_radius = (ExtraRadius + _rectTransform.rect.height) * 0.25f;
				
				element.localPosition += Vector3.up * _radius;
				element.transform.RotateAround(transform.position, Vector3.forward, step * i);
				element.transform.eulerAngles = Vector3.zero;
			}
		}
	}
}
