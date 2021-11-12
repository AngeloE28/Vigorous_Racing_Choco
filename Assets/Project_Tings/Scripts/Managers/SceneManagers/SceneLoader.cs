using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public enum Scene
{
    MainMenu,    
    Game,
}

public class SceneLoader : MonoBehaviour
{    
    [Header("Loading UI")]
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;

    public void LoadScene(Scene scene)
    {
        StartCoroutine(LoadSceneAsynchronously(scene));        
    }

    private IEnumerator LoadSceneAsynchronously(Scene scene)
    {
        // Scene to load
        var sceneToLoad = SceneManager.LoadSceneAsync(scene.ToString());
        // Loading screen        
        loadingCanvas.SetActive(true);              

        while (!sceneToLoad.isDone)
        {
            // Progress of scene loaded
            float progress = Mathf.Clamp01(sceneToLoad.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100.0f) + "%";
            yield return null;
        }
    }    
}
