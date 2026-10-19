using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIPauseScreenController : MonoBehaviour
{
    [SerializeField] private GameObject content;
    
    private bool _previousPassTime = false;
    private bool _isPaused = false;

    private GameInputs _input;
    
    
    public void OnEnable()
    {
        _input = new GameInputs();
        
        _input.Enable();

        _input.Menu.Pause.Enable();
        _input.Menu.Pause.performed += TogglePause;

    }

    private void TogglePause(InputAction.CallbackContext obj)
    {
        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void OnDisable()
    {
        _input.Disable();

        _input.Menu.Pause.Disable();
        _input.Menu.Pause.performed -= TogglePause;

    }


    public void PauseGame()
    {
        _isPaused = true;
        content.SetActive(true);

        if (GameManager.Instance == null)
        {
            return;
        }
        
        _previousPassTime = GameManager.Instance.IsDayRunning;

        if (_previousPassTime)
        {
            GameManager.Instance?.SetPassTime(false);
        }
        
    }

    public void ResumeGame()
    {
        _isPaused =  false;
        content.SetActive(false);
        if (_previousPassTime)
        {
            GameManager.Instance?.SetPassTime(true);
        }
    }
    
    public void MainMenu()
    {
        _isPaused =  false;
        content.SetActive(false);
        SceneChangeManager.Instance?.ChangeScene(Scenes.TitleScreen);
    }
        
    public void Quit()
    {
        SceneChangeManager.Instance?.ExitApplication();
    }
    
}
