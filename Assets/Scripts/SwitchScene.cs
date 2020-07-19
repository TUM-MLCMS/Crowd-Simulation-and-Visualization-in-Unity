using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SwitchScene : MonoBehaviour
{
    public void BottleneckScene() {
        SceneManager.LoadScene("Scenes/BottleneckScene");
    }

    public void ChickenTestScene() {
        SceneManager.LoadScene("Scenes/ChickenTestScene");
    }

    public void CorridorScene() {
        SceneManager.LoadScene("Scenes/CorridorScene");
    }

    public void CornerScene() {
        SceneManager.LoadScene("Scenes/CornerScene");
    }

    public void MainScene() {
        SceneManager.LoadScene("Scenes/MainScene");
    }
}
