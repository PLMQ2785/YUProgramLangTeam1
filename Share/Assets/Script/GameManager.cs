using UnityEngine;

public class GameManager : MonoBehaviour
{
    private UIManager _uiManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void init()
    {
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public UIManager GetUIManager()
    {
        return _uiManager;
    }
}
