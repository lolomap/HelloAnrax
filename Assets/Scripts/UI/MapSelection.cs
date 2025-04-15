using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(RectTransform))]
	public class MapSelection : MonoBehaviour
	{
		private Image _image;
		private Texture2D _texture;
		private RectTransform _rectTransform;

		public Color SelectedColor;
		public GlossaryCard Glossary;
		public GameObject Panel;
		public float ScrollSpeed = 1f;
		private static readonly int _selectedColor = Shader.PropertyToID("_SelectedColor");

		private Dictionary<string, string> _glossaryBindings;
		private bool _isDragging;

		private void Awake()
		{
			_image = GetComponent<Image>();
			_texture = _image.sprite.texture;
			Material material = new(_image.material); // Instantiate new material to change it without affecting asset
			_image.material = material;
			_rectTransform = GetComponent<RectTransform>();

			_glossaryBindings =
				JsonConvert.DeserializeObject<Dictionary<string, string>>(
					ResourceLoader.GetResource<TextAsset>("Map").text);
		}

		public void OnBeginDrag() => _isDragging = true;
		public void OnEndDrag() => _isDragging = false;
		
		public void OnDrag()
		{
			Touch touch = Input.GetTouch(0);
			
			switch (touch.deltaPosition.x)
			{
				case > 0 when Camera.main!.orthographicSize * Camera.main!.aspect
				              < _rectTransform.localPosition.x - _rectTransform.rect.width / 2f:
				case < 0 when Camera.main!.orthographicSize * Camera.main!.aspect
				              > _rectTransform.localPosition.x + _rectTransform.rect.width / 2f:
					return;
				default:
					_rectTransform.anchoredPosition += touch.deltaPosition * ScrollSpeed * Vector2.right;
					break;
			}
		}

		public void Show()
		{
			Panel.SetActive(true);
		}

		public void Hide()
		{
			Panel.SetActive(false);
		}

		public void OnClick()
		{
			if (_isDragging) return;
			
			Touch touch = Input.GetTouch(0);

			RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, touch.position,
				Camera.main, out Vector2 localPoint);

			int px = Mathf.Clamp(0,
				(int) (((localPoint.x - _rectTransform.rect.x) * _texture.width) / _rectTransform.rect.width),
				_texture.width);
			int py = Mathf.Clamp(0,
				(int) (((localPoint.y - _rectTransform.rect.y) * _texture.height) / _rectTransform.rect.height),
				_texture.height);

			SelectedColor = _texture.GetPixel(px, py);
			if (_glossaryBindings.TryGetValue("#"+ColorUtility.ToHtmlStringRGB(SelectedColor), out string country))
			{
				_image.material.SetColor(_selectedColor, SelectedColor);
				Glossary.Show(country);
			}
		}
	}
}
