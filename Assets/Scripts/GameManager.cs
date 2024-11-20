using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static bool _isInited;

    private void Awake()
    {
        if (_isInited) return;
        
        EventStorage.Load();
            
        _isInited = true;
    }

}
