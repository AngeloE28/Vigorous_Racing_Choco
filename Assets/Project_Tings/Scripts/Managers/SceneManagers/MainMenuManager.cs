using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    [Header("UI")]    
    [SerializeField] private GameObject playButton;
    private PlayerInputActions inputActions;
    private float mouseInactiveTimer = 1.0f;

    

    private void Awake()
    {
        inputActions = new PlayerInputActions();
                
        Time.timeScale = 1.0f;        
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
                EventSystem.current.SetSelectedGameObject(playButton);

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
        sceneLoader.LoadScene(Scene.Game);
    }

    public void Quitgame()
    {
        Application.Quit();
    }
}
