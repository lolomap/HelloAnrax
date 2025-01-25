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
	[RequireComponent(typeof(AudioSource))]
	public class RoundList : MonoBehaviour
	{
		private RectTransform _rectTransform;
		private AudioSource _audioSource;

		private RoundListElement _selectedElement;
		
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
					_elements[0].Select(true);
			}
		}

		public bool FitWidth;
		public bool FitHeight;
		public float ExtraRadius;

		public float AdjustSpeed;

		public AudioClip ScrollSound;
		
		private float _radius;
		private float _elementStartAngle;
		private float _cursorStartAngle;
		private float _selectionAngle;
		private bool _isAdjusting;
		
		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_audioSource = GetComponent<AudioSource>();
			
			StartCoroutine(WaitUntilEndOfFrame());
		}

		public void OnDrag()
		{
			if (Input.touches.Length == 0) return;
			if (_isAdjusting) return;

			if (!_audioSource.isPlaying)
				_audioSource.PlayOneShot(ScrollSound);
			
			Touch touch = Input.touches[0];
			float angle = Vector2.SignedAngle(Vector2.up, Camera.main!.ScreenToWorldPoint(touch.position));
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
				float elementAngle = Vector2.Angle(element.localPosition, Vector2.up);
				
				if (elementAngle > _selectionAngle || _selectedElement == _elements[i]) continue;

				if (_selectedElement != null)
					_selectedElement.Unselect();
				_selectedElement = _elements[i];

				_selectedElement.Select();
			}
		}

		public void OnBeginDrag()
		{
			if (_elements.Count < 1) return;
			
			Touch touch = Input.touches[0];
			
			_elementStartAngle = Vector2.SignedAngle(Vector2.up, _elements[0].RTransform.transform.position);
			if (_elementStartAngle < 0) _elementStartAngle = 360 + _elementStartAngle;
			
			_cursorStartAngle = Vector2.SignedAngle(Vector2.up, Camera.main!.ScreenToWorldPoint(touch.position));
			if (_cursorStartAngle < 0) _cursorStartAngle = 360 + _cursorStartAngle;
		}

		public void OnEndDrag()
		{
			_elementStartAngle = 0;
			_cursorStartAngle = 0;
			
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
				if (!_audioSource.isPlaying)
					_audioSource.PlayOneShot(ScrollSound);
				
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

		private IEnumerator WaitUntilEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			ResetElementsPosition();
		}
	}
}
