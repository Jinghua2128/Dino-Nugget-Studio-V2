using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button startGameButton;
    public Button quitButton;
    
    [Header("Audio")]
    public AudioClip menuMusic;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Play menu music
        if (menuMusic != null && audioSource != null)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Setup button listeners
        if (startGameButton != null)
            startGameButton.onClick.AddListener(StartGame);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}