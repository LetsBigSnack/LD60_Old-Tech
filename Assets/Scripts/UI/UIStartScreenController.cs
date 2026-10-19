using UnityEngine;

public class UIStartScreenController : MonoBehaviour
{
    public void StartGame()
    {
        SceneChangeManager.Instance.ChangeScene(Scenes.Testing_Scene);
    }
}
