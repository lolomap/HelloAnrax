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
		private static readonly int _selectedColor = Shader.PropertyToID("_SelectedColor");

		private void Awake()
		{
			_image = GetComponent<Image>();
			_texture = _image.sprite.texture;
			_rectTransform = GetComponent<RectTransform>();
		}

		public void OnClick()
		{
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
			_image.material.SetColor(_selectedColor, SelectedColor);
		}
	}
}
