using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    public Simulation Simulation;
    public bool ShowTrajectories = false;
    public bool ShowGrid = false;
    public bool ShowDijkstraField = false;
    public GameObject PlaybackController;

    // Components
    public Text SimulationControlButtonTest; 
    public Text FrameNumber; 

    private void Awake() {
        Instance = this;
        PlaybackController.SetActive(false);
    }

    public void ToggleTrajectories(bool value) {
        ShowTrajectories = value;
    }

    public void ToggleGrid(bool value) {
        ShowGrid = value;
    }

    public void ToggleDijkstraField(bool value) {
        ShowDijkstraField = value;
        Simulation.Grid.ResetMeshColors(value);
    }

    public void OnSimulationControlClicked() {
        if(Simulation.IsSimulating) {
            Simulation.StopSimulation();
            PlaybackController.SetActive(true);
            FrameNumber.text = Simulation.RecordedFrames.ToString();
        }
        else {
            Simulation.StartSimulation();
            PlaybackController.SetActive(false);
        }
        SimulationControlButtonTest.text = Simulation.IsSimulating ? "End Simulating" : "Start Simulating";
    }

    public void OnPlaybackSliderChanged(float value) {
        var targetFrame = (int) (value * Simulation.RecordedFrames);
        FrameNumber.text = targetFrame.ToString();
        Simulation.Seek(targetFrame);
    }
}