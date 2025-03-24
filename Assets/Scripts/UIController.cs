using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject player;
    public Canvas hudCanvas; 
    private Canvas uiCanvas;
    private FirstPersonController playerController;
    private TextMeshProUGUI startButtonText;
    private TextMeshProUGUI titleText;
    private GameObject darkOverlay;
    private bool isPlaying = false;

    void Start()
    {
        uiCanvas = GetComponent<Canvas>();
        if (uiCanvas == null)
        {
            Debug.LogError("UIController requires a Canvas component!");
            enabled = false;
            return;
        }

        if (player != null)
        {
            playerController = player.GetComponent<FirstPersonController>();
            if (playerController == null)
            {
                Debug.LogError("Player requires FirstPersonController!");
                enabled = false;
                return;
            }
            playerController.enabled = false;
        }

        if (hudCanvas == null)
        {
            Debug.LogError("HUDCanvas not assigned in UIController!");
            return;
        }
        hudCanvas.enabled = false; 

        startButtonText = GameObject.Find("StartButton/Text (TMP)").GetComponent<TextMeshProUGUI>();
        if (startButtonText == null)
        {
            Debug.LogError("StartButton Text (TMP) not found!");
            return;
        }
        startButtonText.text = "Start";

        titleText = GameObject.Find("TitleText").GetComponent<TextMeshProUGUI>();
        if (titleText == null)
        {
            Debug.LogError("TitleText not found!");
            return;
        }
        titleText.text = "SCENE / UI";

        darkOverlay = GameObject.Find("DarkOverlay");
        if (darkOverlay == null)
        {
            Debug.LogError("DarkOverlay not found!");
            return;
        }
        darkOverlay.SetActive(false);

        uiCanvas.enabled = true;
    }

    public void ToggleGameplay()
    {
        isPlaying = !isPlaying;
        uiCanvas.enabled = !isPlaying;
        darkOverlay.SetActive(!isPlaying); 
        hudCanvas.enabled = isPlaying; 
        if (playerController != null)
            playerController.enabled = isPlaying;
        Cursor.lockState = isPlaying ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPlaying;

        startButtonText.text = isPlaying ? "Pause" : "Resume";
        titleText.text = isPlaying ? "SCENE / UI" : "Paused";
        Debug.Log(isPlaying ? "Gameplay started/resumed." : "Gameplay paused.");
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Quit triggered.");
    }
}