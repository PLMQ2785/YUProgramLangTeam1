using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Manager And Controller")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private WeatherController weatherController;
    [SerializeField] private CharacterController characterController;

    public static GameManager Instance { get; private set; } // Singleton instance


    // 다른 코드에서 매니저에 접근하기 위한 public getter
    public UIManager GetUIManager() => uiManager;
    public InputManager GetInputManager() => inputManager;
    public WeatherController GetWeatherController() => weatherController;
    public CharacterController GetCharacterController() => characterController;


    private void Awake()
    {
        Init();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Init()
    {
        //Debug.Log("Init");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (uiManager == null) Debug.LogError("UIManager not assigned to Application.");
        if (inputManager == null) Debug.LogError("InputManager not assigned to Application.");
        if (weatherController == null) Debug.LogError("WeatherController not assigned to Application.");
        if (characterController == null) Debug.LogError("CharacterController not assigned to Application.");

        inputManager?.Init(); // Assuming InputManager has an Initialize method
        uiManager?.Init();    // Assuming UIManager has an Initialize method
        weatherController?.Init(); // Assuming WeatherController has an Initialize method

        // CharacterController might need references to InputManager or other systems
        characterController?.Init(inputManager); // Custom Initialize for CharacterController

        Debug.Log("Application Initialization Complete.");

    }

    public void Shutdown()
    {
        Application.Quit();
    }
}
