﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public string LevelToLoadName;

    public void LoadLevelNow()
    {
        SceneManager.LoadScene(LevelToLoadName);
    }
}
