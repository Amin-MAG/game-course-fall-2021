﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventSystemCustom : MonoBehaviour
{
    public UnityEvent OnCloneStickyPlatformEnter;
    public UnityEvent OnKeyPickUp;
    public UnityEvent WinningGame;
    public UnityEvent LoosingGame;

    void Awake()
    {
        OnCloneStickyPlatformEnter = new UnityEvent();
        OnKeyPickUp = new UnityEvent();
        WinningGame = new UnityEvent();
        LoosingGame = new UnityEvent();
    }
}