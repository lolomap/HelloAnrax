using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(AudioSource))]
	public abstract class RoundListElement : MonoBehaviour
	{
		public RectTransform RTransform;
		public AudioSource AudioSource;

		public AudioClip SelectSound;

		public bool IsSelected;
		
		protected abstract void SelectEffect();
		protected abstract void UnselectEffect();

		public void Select(bool isMuted = false)
		{
			if (IsSelected) return;
			IsSelected = true;
			
			if (!isMuted)
				AudioSource.PlayOneShot(SelectSound);
			SelectEffect();
		}

		public void Unselect()
		{
			if (!IsSelected) return;
			IsSelected = false;
			
			UnselectEffect();
		}
	}

	[RequireComponent(typeof(RectTransform))]
	public class RoundList : MonoBehaviour
	{
		private RectTransform _rectTransform;

		private RoundListElement _preSelectedElement;
		private RoundListElement _selectedElement;
		private int _selectedIndex;
		private int _selectionDirection;
		
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

				if (_elements.Count <= 0) return;
				
				_elements[0].Select(true);
				_selectedElement = _elements[0];
				_selectedIndex = 0;
			}
		}

		public bool FitWidth;
		public bool FitHeight;
		public float ExtraRadius;
		public bool RotateByOne;
		public float ByOneSelectionAngle = 15f;

		public float AdjustSpeed;
		
		private float _radius;
		private float _elementStartAngle;
		private float _cursorStartAngle;
		private float _selectionAngle;
		private bool _isAdjusting;
		
		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			
			StartCoroutine(WaitUntilEndOfFrame());
		}

		public RoundListElement GetSelected() => _selectedElement;

		public void SelectNext()
		{
			if (_isAdjusting) return;
			_selectedIndex++;
			if (_selectedIndex > _elements.Count - 1) _selectedIndex = 0;
			
			if (_selectedElement != null)
				_selectedElement.Unselect();
			_selectedElement = _elements[_selectedIndex];
			_selectedElement.Select();
			
			_isAdjusting = true;
			StartCoroutine(Adjust(_selectedIndex));
		}

		public void SelectPrevious()
		{
			if (_isAdjusting) return;
			_selectedIndex--;
			if (_selectedIndex < 0) _selectedIndex = _elements.Count - 1;
			
			if (_selectedElement != null)
				_selectedElement.Unselect();
			_selectedElement = _elements[_selectedIndex];
			_selectedElement.Select();
			
			_isAdjusting = true;
			StartCoroutine(Adjust(_selectedIndex));
		}

		public void OnDrag()
		{
			if (_isAdjusting) return;

			Touch touch = Input.GetTouch(0);
			float angle = Vector2.SignedAngle(Vector2.up, (Vector2)Camera.main!.ScreenToWorldPoint(touch.position) - (Vector2)transform.position);
			if (angle < 0) angle = 360 + angle;

			angle -= _cursorStartAngle;
			angle += _elementStartAngle;
			
			float step = 360f / _elements.Count;

			for (int i = 0; i < _elements.Count; i++)
			{
				// Set proper rotation for element
				RectTransform element = _elements[i].RTransform;
				element.localPosition = Vector3.zero;

				_radius = ExtraRadius;
				if (FitWidth)
					_radius = (ExtraRadius + _rectTransform.rect.width) * 0.25f;
				else if (FitHeight)
					_radius = (ExtraRadius + _rectTransform.rect.height) * 0.25f;
				
				element.localPosition += Vector3.up * _radius;
				element.transform.RotateAround(transform.position, Vector3.forward, step * i);
				element.transform.RotateAround(transform.position, Vector3.forward, angle);
				element.transform.eulerAngles = Vector3.zero;
				
				// Calculate new selection
				if (!RotateByOne)
				{
					float elementAngle = Vector2.Angle(element.localPosition, Vector2.up);

					if (elementAngle > _selectionAngle || _selectedElement == _elements[i]) continue;

					if (_selectedElement != null)
						_selectedElement.Unselect();
					_selectedElement = _elements[i];

					_selectedElement.Select();
				}
			}
			if (RotateByOne)
				CalculateSelection();
		}

		public void OnBeginDrag()
		{
			if (_elements.Count < 1) return;

			Touch touch = Input.GetTouch(0);
			
			_elementStartAngle = Vector2.SignedAngle(Vector2.up, (Vector2)_elements[0].RTransform.transform.position - (Vector2)transform.position);
			if (_elementStartAngle < 0) _elementStartAngle = 360 + _elementStartAngle;
			
			_cursorStartAngle = Vector2.SignedAngle(Vector2.up, (Vector2)Camera.main!.ScreenToWorldPoint(touch.position) - (Vector2)transform.position);
			if (_cursorStartAngle < 0) _cursorStartAngle = 360 + _cursorStartAngle;

			if (_preSelectedElement == null) _preSelectedElement = _selectedElement;
		}

		public void OnEndDrag()
		{
			_elementStartAngle = 0;
			_cursorStartAngle = 0;

			if (!RotateByOne)
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
			else
			{
				CalculateSelection();
				_isAdjusting = true;
				_selectionDirection = 0;
				_preSelectedElement = _selectedElement;
				StartCoroutine(Adjust(_selectedIndex));
			}
		}

		private void CalculateSelection()
		{
			float selectedCurrentAngle = Vector2.SignedAngle(_preSelectedElement.RTransform.localPosition, Vector2.up);
			if (selectedCurrentAngle > ByOneSelectionAngle)
			{
				if (_selectionDirection != 0) return;
				_selectedIndex++;
				if (_selectedIndex > _elements.Count - 1) _selectedIndex = 0;
				_selectionDirection = 1;
			}
			else if (selectedCurrentAngle < -ByOneSelectionAngle)
			{
				if (_selectionDirection != 0) return;
				_selectedIndex--;
				if (_selectedIndex < 0) _selectedIndex = _elements.Count - 1;
				_selectionDirection = -1;
			}
			else
			{
				/*if (_selectionDirection == 0) return;
				_selectedIndex -= _selectionDirection;
				if (_selectedIndex > _elements.Count - 1) _selectedIndex = 0;
				if (_selectedIndex < 0) _selectedIndex = _elements.Count - 1;
				_selectionDirection = 0;*/
				
			}

			if (_selectedElement != null)
				_selectedElement.Unselect();
			_preSelectedElement = _selectedElement;
			_selectedElement = _elements[_selectedIndex];
			_selectedElement.Select();
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
				element.transform.SetParent(transform);
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

		private IEnumerator WaitUntilEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			ResetElementsPosition();
		}
	}
}
