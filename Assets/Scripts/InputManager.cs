#define DEBUG_MOUSE
#undef DEBUG_MOUSE

using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public static InputManager Instance;
	
	private readonly bool _isMouse;

	private Vector2 _lastPosition = Vector2.zero;
	private Vector2 _currentPosition = Vector2.zero;
	private Vector2 _deltaPosition = Vector2.zero;
	
	private float _lastPinchDistance;

	public float PinchDelta { get; private set; }

	public InputManager()
	{
#if UNITY_ANDROID
		_isMouse = false;
#else
		_isMouse = true;
#endif

#if DEBUG && DEBUG_MOUSE
		_isMouse = true;
#endif
	}

	public void Awake()
	{
		Instance = this;
	}

	public void Update()
	{
		if (_isMouse)
		{
			_currentPosition = Input.mousePosition;
			_deltaPosition = _currentPosition - _lastPosition;
			_lastPosition = _currentPosition;
		}
		else
		{
			if (Input.touchCount > 1)
			{
				float distance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				if (_lastPinchDistance > 0f)
					PinchDelta = distance - _lastPinchDistance;
				_lastPinchDistance = distance;
			}
			else
			{
				_lastPinchDistance = 0f;
				PinchDelta = 0f;
			}
		}
	}
    
	public Touch GetPointerInput()
	{
		if (!_isMouse) return Input.GetTouch(0);

		Touch result = new()
		{
			position = _currentPosition,
			deltaPosition = _deltaPosition
		};

		return result;
	}

	public void ShowAchievements()
	{
		AchievementsManager.ShowGPAchievements();
	}
}