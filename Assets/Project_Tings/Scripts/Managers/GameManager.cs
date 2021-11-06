using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [Header("Player and AIs")]
    [SerializeField] private PlayerInputs player;
    [SerializeField] private CarAI playerAI;
    [SerializeField] private CarAI[] enemies;
    [SerializeField] private List<GameObject> cakeCars;    

    [Header("Gameplay Loop")]
    [SerializeField] private float countDownTimer = 3.0f;
    [SerializeField] private float goMsgTimer = 1.5f;
    [SerializeField] private float finalLapMsgTimer = 1.5f;
    private const int LAPSNEEDED = 3;
    private bool isGameRunning;
    private bool isGamePaused;    

    [Header("GUI")]
    [SerializeField] private TMP_Text lapText;
    [SerializeField] private TMP_Text finalLapMsg;
    [SerializeField] private TMP_Text posText;
    [SerializeField] private TMP_Text posTextSuffix;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseFirstButton;
    [SerializeField] private GameObject pauseWindow;
    [SerializeField] private GameObject backgroundPanel; // Use this panel for the gameover window aswell
    private float mouseInactiveTimer = 1.0f;

    [Header("Game Start UI")]
    [SerializeField] private TMP_Text goMsg;
    [SerializeField] private TMP_Text countDownMsg;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverFirstButton;
    [SerializeField] private GameObject gameOverWindow;
    [SerializeField] private GameObject standingWindow;
    [SerializeField] private TMP_Text continueText;
    [SerializeField] private TMP_Text firstPlace;
    [SerializeField] private TMP_Text secondPlace;
    [SerializeField] private TMP_Text thirdPlace;    
    [SerializeField] private float standingWindowCloseTimer = 1.0f;    

    private void Awake()
    {
        isGameRunning = true;
        isGamePaused = false;
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        cakeCars = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player.GetPlayerInputActions().Player.PauseGame.performed += _ => GamePauseState();
        Invoke(nameof(GameStart), countDownTimer);        
    }

    // Update is called once per frame
    void Update()
    {
        GameStartCountDown();
        InGameUI();
        PlayerFinalLapUI();
        WinCondition();
        CursorManager();
        InputManagerPaused();
        InputManagerGameOver();        
    }

    private void GameStart()
    {
        // Default value of speed controller is 1
        player.SetSpeedController(1.0f);
        foreach(CarAI ai in enemies)
        {
            ai.SetSpeedController(1.0f);
        }        
    }

    private void GameStartCountDown()
    {
        if(countDownTimer > 0)
        {
            goMsg.gameObject.SetActive(false);
            countDownTimer -= Time.deltaTime;
            countDownMsg.text = Mathf.RoundToInt(countDownTimer).ToString();
        }
        else
        {
            countDownMsg.gameObject.SetActive(false);
            countDownTimer = 0;
        }

        switch(countDownTimer)
        {
            case 0:
                goMsg.gameObject.SetActive(true);
                Invoke(nameof(DisableGoMsg), goMsgTimer);
                break;
        }
    }

    private void DisableGoMsg()
    {
        goMsgTimer = 0;
        goMsg.gameObject.SetActive(false);
    }

    private void InGameUI()
    {
        lapText.text = "Lap " + player.GetLapIndex().ToString() + "/" + LAPSNEEDED.ToString();
                
        switch(player.playerPlacement)
        {
            case 1:
                posText.text = "Pos " + player.playerPlacement.ToString();
                posTextSuffix.text = "st";
                break;
            case 2:
                posText.text = "Pos " + player.playerPlacement.ToString();
                posTextSuffix.text = "nd";
                break;
            case 3:
                posText.text = "Pos " + player.playerPlacement.ToString();
                posTextSuffix.text = "rd";
                break;
            default:
                break;
        }        
    }

    private void PlayerFinalLapUI()
    {
        if(player.GetLapIndex() == LAPSNEEDED)
        {
            finalLapMsg.gameObject.SetActive(true);
            Invoke(nameof(DisablePlayerFinalLapMsg), finalLapMsgTimer);
        }
    }

    private void DisablePlayerFinalLapMsg()
    {
        finalLapMsgTimer = 0.0f;
        finalLapMsg.gameObject.SetActive(false);
    }

    private void WinCondition()
    {
        // Player conditions
        // Cars need to do an extra lap from the laps needed since they start with a lap index of 1
        if (player.GetLapIndex() == (LAPSNEEDED + 1))
        {
            if (!cakeCars.Contains(player.gameObject))
            {
                cakeCars.Add(player.gameObject);
                standingWindow.SetActive(true);
            }
        }

        foreach (CarAI ai in enemies)
        {
            if (ai.GetLapIndex() == (LAPSNEEDED + 1))
            {
                if (!cakeCars.Contains(ai.gameObject))
                    cakeCars.Add(ai.gameObject);
            }
        }

        if (cakeCars.Count != 0)
        {
            if (cakeCars.Contains(player.gameObject))
            {
                isGameRunning = false;

                // Player can't controller the car anymore
                player.SetSpeedController(0.0f);
                playerAI.enabled = true;
                playerAI.SetSpeedController(1.0f);

                GameOver();
                Invoke(nameof(CloseStandingWindow), standingWindowCloseTimer);                
            }
            foreach (GameObject car in cakeCars)
            {
                // Clamp the lap count
                car.GetComponent<ICakeCar>().SetLapIndex(LAPSNEEDED);
            }
        }

    }

    public void GameOver()
    {        
        switch (cakeCars.Count)
        {
            case 1:
                if (cakeCars[0] == player.gameObject)
                    firstPlace.text = "You";
                else
                    firstPlace.text = cakeCars[0].name.ToString();
                
                secondPlace.text = "Waiting...";
                thirdPlace.text = "Waiting...";
                break;
            case 2:
                if (cakeCars[1] == player.gameObject)
                    secondPlace.text = "You";
                else
                    secondPlace.text = cakeCars[1].name.ToString();                

                thirdPlace.text = "Waiting...";
                break;
            case 3:
                if (cakeCars[2] == player.gameObject)
                    thirdPlace.text = "You";
                else
                    thirdPlace.text = cakeCars[2].name.ToString();                
                break;
            default:
                break; // Do nothing
        }
    }

    public void InputManagerGameOver()
    {
        Vector2 gamePadNavigation = player.GetPlayerInputActions().UI.Navigate.ReadValue<Vector2>();
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (gamePadNavigation != Vector2.zero)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(gameOverFirstButton);

                Cursor.visible = false;
            }
        }
    }    

    private void CloseStandingWindow()
    {        
        if(standingWindow.activeSelf)
        {
            Vector2 gamePadNavigation = player.GetPlayerInputActions().UI.Navigate.ReadValue<Vector2>();
            if (gamePadNavigation != Vector2.zero)
                continueText.text = "Press X to Continue";

            Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            if (Input.anyKey || mousePos != Vector2.zero)
                continueText.text = "Press Space to Continue";

            if (Gamepad.current.buttonSouth.isPressed || Keyboard.current.spaceKey.isPressed)
            {
                standingWindowCloseTimer = 0.0f;                
                standingWindow.SetActive(false);
                GameOverWindow();
            }
        }
    }

    private void GameOverWindow()
    {
        standingWindow.SetActive(false);
        gameOverWindow.SetActive(true);
        backgroundPanel.SetActive(true);

        // Get first button for controller
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameOverFirstButton);
    }

    private void CursorManager()
    {
        if (isGameRunning && isGamePaused)
            ShowMouseCursor();
        if (isGameRunning && !isGamePaused)
            Cursor.visible = false;
        if (!isGameRunning)
            ShowMouseCursor();
    }

    private void ShowMouseCursor()
    {
        // If mouse is moving show the cursor
        Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (mousePos != Vector2.zero)
        {
            Cursor.visible = true;
            mouseInactiveTimer = 1.0f; // Reset timer to default value
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

    private void InputManagerPaused()
    {
        if(isGamePaused)
        {
            Vector2 gamePadNavigation = player.GetPlayerInputActions().UI.Navigate.ReadValue<Vector2>();            
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (gamePadNavigation != Vector2.zero)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(pauseFirstButton);

                    Cursor.visible = false;
                }
            }
        }
    }

    public void GamePauseState()
    {
        if (isGamePaused)                                
            Resume();        
        else
            Pause();        
    }

    public void Pause()
    {       
        pauseWindow.SetActive(true);
        backgroundPanel.SetActive(true);

        Time.timeScale = 0.0f;

        // Get first button for controller
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstButton);
        
        isGamePaused = true;
    }

    public void Resume()
    {        
        pauseWindow.SetActive(false);
        backgroundPanel.SetActive(false);   

        Time.timeScale = 1.0f;

        isGamePaused = false;
    }

    public void Restart()
    {        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Time.timeScale = 1.0f;
        // Go back to the main menu
        SceneManager.LoadScene(0);   
    }
}
