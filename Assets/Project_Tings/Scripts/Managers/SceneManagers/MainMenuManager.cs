using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    [Header("MainMenu")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject optionsButton;

    [Header("Options")]
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject optionsFirstButton;
    private PlayerInputActions inputActions;
    private float mouseInactiveTimer = 1.0f;
    private GameObject currentBtn;
    

    private void Awake()
    {
        inputActions = new PlayerInputActions();
                
        Time.timeScale = 1.0f;
        currentBtn = playButton;

        mainMenuUI.SetActive(true);
        optionsUI.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
    }

    private void InputManager()
    {
        Vector2 gamePadNavigation = inputActions.UI.Navigate.ReadValue<Vector2>();
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if(gamePadNavigation != Vector2.zero)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(currentBtn);

                Cursor.visible = false;
            }
        }

        Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (mousePos != Vector2.zero)
        {
            Cursor.visible = true;
            mouseInactiveTimer = 1.0f;
        }
        else
            HideMouseCursor();
    }

    private void HideMouseCursor()
    {
        if (mouseInactiveTimer > 0)
            mouseInactiveTimer -= Time.unscaledDeltaTime;
        else
            Cursor.visible = false;
    }

    public void Play()
    {
        // Set current button to play button
        // When player returns to main menu play is the first selected
        currentBtn = playButton;

        sceneLoader.LoadScene(Scene.Game);
    }

    public void OpenOptions()
    {
        mainMenuUI.SetActive(false);
        optionsUI.SetActive(true);

        // First button on main menu
        currentBtn = optionsFirstButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void CloseOptions()
    {
        mainMenuUI.SetActive(true);
        optionsUI.SetActive(false);

        // Set current button to options button
        // When options closes options is the first selected
        currentBtn = optionsButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void Quitgame()
    {
        Application.Quit();
    }
}
