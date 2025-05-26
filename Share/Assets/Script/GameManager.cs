using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Manager And Controller")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private WeatherController weatherController;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameTime gameTime;
    [SerializeField] private Weather weather;
    [SerializeField] Character character;

    public static GameManager Instance { get; private set; } // Singleton instance


    // �ٸ� �ڵ忡�� �Ŵ����� �����ϱ� ���� public getter
    public UIManager GetUIManager() => uiManager;
    public InputManager GetInputManager() => inputManager;
    public WeatherController GetWeatherController() => weatherController;
    public CharacterController GetCharacterController() => characterController;
    public GameTime GetGameTime() => gameTime;
    public Weather GetWeather() => weather;
    public Character GetCharacter() => character;


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

        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }



        weatherController?.Init();
        inputManager?.Init();
        characterController?.Init();
        uiManager?.Init();


        //Debug.Log("Application Initialization Complete.");

    }

    public void Shutdown()
    {
        Application.Quit();
    }
}
