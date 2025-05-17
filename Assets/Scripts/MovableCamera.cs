using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableCamera : MonoBehaviour
{
    public bool IsSlidable;

    public float SlideSpeed = 0.2f;
    public float AutoSlideSpeed = 0.1f;
    
    /// <summary>
    /// Отступ от леовй границы сцены для спавна персонажа и камеры
    /// </summary>
    public float LeftBoundOffset;
    
    public bool MoveLimited;
    public float CameraLeftBound;
    public float CameraRightBound;
    public SpriteRenderer BoundsTarget;

    private Camera _thisCamera;
    private float _height, _width;

    private bool _autoSlide;
    private int _autoDirection;

    private Bounds _bounds;

    private const float LongTapTime = 0.2f;

    private float _tapTime;
    private bool _isTapSlide;

    public delegate void TapHandler(Vector2 position);
    public static event TapHandler Tap;
    public static event TapHandler ShortTap;
    public static event TapHandler LongTap;

    public delegate void SlideHandler(Vector2 deltaPos, float deltaTime);
    public static event SlideHandler Slide;
    
    private void Awake()
    {
        _thisCamera = GetComponent<Camera>();
        if (BoundsTarget != null)
            _bounds = BoundsTarget.bounds;
    }

    private void Start()
    {
        _height = _thisCamera.orthographicSize;
        _width = _height * _thisCamera.aspect;

        if (BoundsTarget != null)
        {
            CameraLeftBound = _bounds.min.x + _width;
            CameraRightBound = _bounds.extents.x - _width;
            float startPositionX = _bounds.min.x + LeftBoundOffset;
            transform.position = new Vector3(startPositionX, transform.position.y, transform.position.z);
        }

        Slide += OnSlide;
    }
    
    private void Update()
    {
        if (Input.touchCount == 1)
        {
            //if (IsClickedOnUi()) return;

            Touch touch = InputManager.Instance.GetPointerInput();

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _tapTime = 0;
                    break;

                case TouchPhase.Stationary:
                    _tapTime += Time.deltaTime;
                    break;

                case TouchPhase.Moved:
                    _isTapSlide = true;
                    Slide?.Invoke(touch.deltaPosition, touch.deltaTime);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                {
                    if (!_isTapSlide)
                    {
                        if (_tapTime < LongTapTime)
                            ShortTap?.Invoke(touch.position);
                        else
                            LongTap?.Invoke(touch.position);

                        Tap?.Invoke(touch.position);

                        _tapTime = 0;
                    }

                    _isTapSlide = false;
                    break;
                }
            }
        }
    }

    private void OnSlide(Vector2 deltaPos, float deltaTime)
    {
        transform.position -= new Vector3(deltaPos.normalized.x * SlideSpeed, 0);
    }

    private static bool IsClickedOnUi()
    {

        PointerEventData eventDataCurrentPosition = new(EventSystem.current)
        {
            position = InputManager.Instance.GetPointerInput().position
        };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        // return results.Count > 0;
        foreach (var item in results)
        {
            if (item.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                return true;
            }
        }
        return false;
    }
}
