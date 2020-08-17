using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneState
{
    public static void LoadScene(int sceneId) {
        SceneManager.LoadScene(sceneId);
    }
}
