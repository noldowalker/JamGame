using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserInterface;
using UserInterface.Enums;


public class UIService : MonoBehaviour
{
    public static UIService Current;

    [SerializeField] GameObject WinScrene;
    [SerializeField] bool showWinScr;

    [Header("Включить игровой интерфейс")]
    [SerializeField] 
    private bool showGameLayout; 
    
    [Header("Включить главное меню")]
    [SerializeField] 
    private bool showMainMenu;
    
    [Space]
    [Header("Необходимые для игрового интерфейса ссылки")]
    [SerializeField] 
    private Canvas gameCanvas;
    [Header("Префабы")]
    [SerializeField] 
    private HotkeyBar hotKeyPrefab;
    [SerializeField] 
    private HelpPanel helpPanelPrefab;
    [SerializeField]
    private HotkeyCell hotkeyCellPrefab;
    [Header("Спрайты")]
    [SerializeField]
    private Sprite helpIcon;
    [SerializeField]
    private Image youDiedPanel;
    
    [Space]
    [Header("Необходимые для Главного меню ссылки")]
    [SerializeField] 
    private MainMenu mainMenuPrefab;
    
    [Space]
    [Header("Необходимые для загрузки ссылки")]
    [SerializeField] 
    private LoadingScreen loadingScreenPrefab;
    
    private HotkeyBar _hotKeyPanel;
    private HelpPanel _helpPanel;
    private float _testHp = 100;
    private AudioSource audioSource;
    
    private void Awake()
    {
        if (showWinScr)
        {
            ShowWinScr();
        }
        if (Current != null)
            Debug.LogError("Создано больше 1 сервиса для интерфейса!");
        
        if (showGameLayout)
            TurnOnGameInterface();
        if (showMainMenu)
            TurnOnMainMenu();
        
        Current = this;
    }

    void Start()
    {
        youDiedPanel.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I)) 
            ObserverWithoutData.FireEvent(Events.HelpPanelCalled);
    }

    public Image GetPlayerHpBarImage() => _hotKeyPanel.PlayerHpBar;
    
    public HotkeyCell AddHotkey(Events eventForPress, string hotkeyChar, Sprite icon)
    {
        var containerTransform = gameCanvas.transform;
        var hotkey = Instantiate(hotkeyCellPrefab, containerTransform);
        hotkey.SetNewHotkey(hotkeyChar);
        hotkey.SetEventSubscription(eventForPress);
        hotkey.SetIcon(icon);
        
        return hotkey;
    }

    public void ShowWinScr()
    {
        WinScrene.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        LoadSceneWithScreen("MainMenuScene");
    }
    public void ShowDeathMessage()
    {
        if (_hotKeyPanel)
        {
            StartCoroutine(TranslationToMainMenuOnDeath());
        }
    }
    
    private void TurnOnGameInterface()
    {
        if (gameCanvas == null)
            Debug.LogError("Не установлен основной игровой канвас для сервиса пользовательского интерфейса");
        if (hotKeyPrefab == null)
            Debug.LogError("Не установлен префаб для панели горячих клавиш");
        if (helpPanelPrefab == null)
            Debug.LogError("Не установлен префаб для панели-подсказки");

        var helpHotkey = AddHotkey(Events.HelpPanelCalled, "I", helpIcon);
        helpHotkey.AlignToCoords(50, 50, Align.LeftBottom);
        
        _hotKeyPanel = Instantiate(hotKeyPrefab, gameCanvas.transform);
        
        _helpPanel = Instantiate(helpPanelPrefab, gameCanvas.transform);
        ObserverWithoutData.Sub(Events.HelpPanelCalled, _helpPanel.SwitchPanelVisibility);
    }
    
    private void TurnOnMainMenu()
    {
        if (gameCanvas == null)
            Debug.LogError("Не установлен основной игровой канвас для сервиса пользовательского интерфейса");
        if (mainMenuPrefab == null)
            Debug.LogError("Не установлен префаб для главного меню");

        Instantiate(mainMenuPrefab, gameCanvas.transform);
    }

    private void OnDestroy()
    {
        Current = null;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(6);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private IEnumerator TranslationToMainMenuOnDeath()
    {
        MusicHandleScript.Current.SwitchCurrentMusic(MusicEnum.DEATH_SCREEN);
        foreach(var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<EnemyAIControllerScript>().AIDisabled = true;
        }
        youDiedPanel.gameObject.SetActive(true);
        Time.timeScale = .2f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.1f);
        youDiedPanel.gameObject.SetActive(false);
        Current.LoadSceneWithScreen(SceneManager.GetActiveScene().name);
    }

    public void PlayPressButtonSound()
    {
        SoundHandleScript.Current.PlaySound(SoundEnum.BUTTON_CLICK, audioSource);
    }

    public void ChangeSpheresAmount(int spheresAmount)
    {
        _hotKeyPanel.ChangeSpheresAmount(spheresAmount);
    }

    public void LoadSceneWithScreen(string sceneName)
    {
        if (gameCanvas == null)
            Debug.LogError("Не установлен основной игровой канвас для сервиса пользовательского интерфейса");
        if (loadingScreenPrefab == null)
            Debug.LogError("Не установлен префаб для экрана загрузки");

        var loadingScreen =Instantiate(loadingScreenPrefab, gameCanvas.transform);
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.LoadScene(sceneName);
    }
}
