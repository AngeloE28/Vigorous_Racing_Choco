using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

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
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject pauseFirstButton;
    [SerializeField] private GameObject optionsButton;    
    [SerializeField] private GameObject backgroundPanel; // Use this panel for the gameover window aswell
    private float mouseInactiveTimer = 1.0f;

    [Header("Options UI")]
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject optionsFirstButton;
    private GameObject currentBtn;

    [Header("Tabs")]
    [SerializeField] private GameObject volumeUI;
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject controlsBtn;

    [Header("Controls Tab")]
    [SerializeField] private GameObject gameplayControlsBtn;
    [SerializeField] private GameObject gameplayControls;
    [SerializeField] private GameObject uiControlsBtn;
    [SerializeField] private GameObject uiControls;


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

    [Header("Sound")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource uiSound;
    [SerializeField] private AudioClip countdown;
    [SerializeField] private AudioClip goSound;    
    private float ringDelay = 0.8f;

    private void Awake()
    {
        isGameRunning = true;
        isGamePaused = false;
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        currentBtn = pauseFirstButton;
        cakeCars = new List<GameObject>();
        Invoke(nameof(FirstRing), 0.1f); // Small delay at the start
    }

    // Start is called before the first frame update
    void Start()
    {
        player.GetPlayerInputActions().Player.PauseGame.performed += _ => GamePauseState();
        float countDown = countDownTimer;
        Invoke(nameof(GameStart), countDown);                
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

    #region Game Events
    private void GameStart()
    {
        // Default value of speed controller is 1
        player.SetSpeedController(1.0f);
        foreach(CarAI ai in enemies)
        {
            ai.SetSpeedController(1.0f);
        }        
    }

    #region Rings
    private void FirstRing()
    {
        float secondRingDelay = 0.6f;
        uiSound.PlayOneShot(countdown);
        Invoke(nameof(SecondRing), secondRingDelay);
    }

    private void SecondRing()
    {
        uiSound.PlayOneShot(countdown);
        Invoke(nameof(ThirdRing), ringDelay);
    }

    private void ThirdRing()
    {
        uiSound.PlayOneShot(countdown);
        Invoke(nameof(FourthRing), ringDelay);
    }

    private void FourthRing()
    {
        uiSound.PlayOneShot(countdown);
        Invoke(nameof(GoRing), ringDelay);
    }    

    private void GoRing()
    {
        uiSound.pitch = 2.0f;
        uiSound.PlayOneShot(goSound);        
    }
    #endregion Rings

    private void GameStartCountDown()
    {
        if (countDownTimer > 0)
        {
            goMsg.gameObject.SetActive(false);
            countDownTimer -= Time.deltaTime;
            countDownMsg.text = Mathf.RoundToInt(countDownTimer).ToString();                            
        }        
        else
        {
            countDownMsg.gameObject.SetActive(false);
            goMsg.gameObject.SetActive(true);            
            Invoke(nameof(DisableGoMsg), goMsgTimer);
            countDownTimer = 0;
        }        
    }

    private void DisableGoMsg()
    {
        uiSound.pitch = 1.0f;
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

    private void GameOverWindow()
    {
        standingWindow.SetActive(false);
        gameOverWindow.SetActive(true);
        backgroundPanel.SetActive(true);

        // Get first button for controller
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(gameOverFirstButton);
    }

    private void CloseStandingWindow()
    {        
        if(standingWindow.activeSelf)
        {
            continueText.text = "Press Any Button to Continue";                       
            if (Input.anyKey)
            {
                standingWindowCloseTimer = 0.0f;                
                standingWindow.SetActive(false);
                GameOverWindow();
            }
        }
    }

    #endregion

    #region UI Events
    public void InputManagerGameOver()
    {
        currentBtn = gameOverFirstButton;
        Vector2 gamePadNavigation = player.GetPlayerInputActions().UI.Navigate.ReadValue<Vector2>();
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (gamePadNavigation != Vector2.zero)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(currentBtn);

                Cursor.visible = false;
            }
        }
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
            if (pauseUI.activeSelf)
                currentBtn = pauseFirstButton;
            if (optionsUI.activeSelf)
                currentBtn = optionsFirstButton;

            Vector2 gamePadNavigation = player.GetPlayerInputActions().UI.Navigate.ReadValue<Vector2>();            
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (gamePadNavigation != Vector2.zero)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(currentBtn);

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
        pauseUI.SetActive(true);
        backgroundPanel.SetActive(true);
        currentBtn = pauseFirstButton;
        Time.timeScale = 0.0f;

        // Stop all audioSources
        audioMixer.SetFloat(Sounds.sfxVol.ToString(), -80.0f);
        audioMixer.SetFloat(Sounds.musicVol.ToString(), -80.0f);

        // Get first button for controller
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
        
        isGamePaused = true;
    }

    public void Resume()
    {
        if (pauseUI.activeSelf)
            pauseUI.SetActive(false);
        if (backgroundPanel.activeSelf)
            backgroundPanel.SetActive(false);
        if (optionsUI.activeSelf)
            optionsUI.SetActive(false);
        if (volumeUI.activeSelf)
            volumeUI.SetActive(false);
        if (controlsUI.activeSelf)
            controlsUI.SetActive(false);

        currentBtn = pauseFirstButton;

        Time.timeScale = 1.0f;

        // Resume all audiosources        
        audioMixer.SetFloat(Sounds.sfxVol.ToString(), PlayerPrefs.GetFloat(Options._SFXVOL.ToString()));
        audioMixer.SetFloat(Sounds.musicVol.ToString(), PlayerPrefs.GetFloat(Options._MUSICVOL.ToString()));

        isGamePaused = false;
    }

    public void OpenOptions()
    {
        pauseUI.SetActive(false);
        optionsUI.SetActive(true);
        volumeUI.SetActive(true);
        controlsUI.SetActive(false);

        // First button on main menu
        currentBtn = optionsFirstButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void CloseOptions()
    {
        pauseUI.SetActive(true);
        optionsUI.SetActive(false);
        volumeUI.SetActive(false);
        controlsUI.SetActive(false);

        // Set current button to options button
        // When options closes options is the first selected
        currentBtn = optionsButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void OpenVolumeTab()
    {
        if (controlsUI.activeSelf)
            controlsUI.SetActive(false);
        volumeUI.SetActive(true);

        currentBtn = optionsFirstButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void OpenControlsTab()
    {
        if (volumeUI.activeSelf)
            volumeUI.SetActive(false);
        controlsUI.SetActive(true);

        gameplayControls.SetActive(true);
        uiControls.SetActive(false);

        currentBtn = controlsBtn;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void ShowGameplayControsl()
    {
        uiControls.SetActive(false);
        gameplayControls.SetActive(true);

        currentBtn = gameplayControlsBtn;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void ShowUIControls()
    {
        uiControls.SetActive(true);
        gameplayControls.SetActive(false);

        currentBtn = uiControlsBtn;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void Restart()
    {
        // Make sure volume is normal
        audioMixer.SetFloat(Sounds.sfxVol.ToString(), PlayerPrefs.GetFloat(Options._SFXVOL.ToString()));
        audioMixer.SetFloat(Sounds.musicVol.ToString(), PlayerPrefs.GetFloat(Options._MUSICVOL.ToString()));
        sceneLoader.LoadScene(Scene.Game);
    }

    public void Quit()
    {
        Time.timeScale = 1.0f;
        // Make sure volume is normal
        audioMixer.SetFloat(Sounds.sfxVol.ToString(), PlayerPrefs.GetFloat(Options._SFXVOL.ToString()));
        audioMixer.SetFloat(Sounds.musicVol.ToString(), PlayerPrefs.GetFloat(Options._MUSICVOL.ToString()));

        // Go back to the main menu
        sceneLoader.LoadScene(Scene.MainMenu);   
    }

    #endregion
}