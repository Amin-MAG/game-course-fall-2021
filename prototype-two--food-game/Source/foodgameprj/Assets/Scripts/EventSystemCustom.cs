﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EventSystemCustom : MonoBehaviour
{
    public UnityEvent onBoardScoresChanged;

    void Awake()
    {
        onBoardScoresChanged = new UnityEvent();
    }

}