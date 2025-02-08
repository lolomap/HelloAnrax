using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

//TODO: I don't remember how to use it
public class FrameRateManager : MonoBehaviour
{
    public int NormalFrameRate;
    public int EconomyFrameRate;
    public bool EconomyEnabled;

    private int _frameInterval;
    private int _lastRequestedFrame;
    public bool AutoRequest;
    public bool LockEconomy;

    private const int BufferFrames = 3;

    private void Awake()
    {
        Application.targetFrameRate = NormalFrameRate;
        _frameInterval = NormalFrameRate / EconomyFrameRate;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!EconomyEnabled)
            return;
        if (AutoRequest && !LockEconomy)
        {
            _lastRequestedFrame = Time.frameCount;
        }

        OnDemandRendering.renderFrameInterval =
            (Time.frameCount - _lastRequestedFrame) < BufferFrames ? 1 : _frameInterval;
    }

    private IEnumerator RequestFullFrameRateCoroutine(float seconds)
    {
        float secs = 0f;
        while (secs < seconds)
        {
            _lastRequestedFrame = Time.frameCount;
            secs += Time.deltaTime;
            yield return null;
        }
    }

    public void RequestFullFrameRate(float seconds)
    {
        if (LockEconomy) return;
        StartCoroutine(RequestFullFrameRateCoroutine(seconds));
    }
}
