using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Main : SingletonBase<Main>
{
    public SceneType currentScene = SceneType.None;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        UIManager.Instance.Initialize();
        UserDataModel.Instance.Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeScene(SceneType.Lobby);
    }

    public void ChangeScene(SceneType scenetype)
    {
        if (currentScene == scenetype)
        {
            return;
        }

        UIManager.Instance.HideAllPanel();
        UIManager.Instance.HideAllPopup();

        UnityEngine.SceneManagement.SceneManager.LoadScene(scenetype.ToString());

        LoadSceneUI(scenetype);

        currentScene = scenetype;
    }

    public void LoadSceneUI(SceneType scenetype)
    {
        switch(scenetype)
        {
            case SceneType.Lobby:
            {
                LobbyUI lobbyUI = UIManager.Instance.GetUI<LobbyUI>(UIList.LobbyUI);
                lobbyUI.Show();
                break;
            }            
            case SceneType.Game:
            {
                break;
            }            
            case SceneType.Shooting:
            {
                ShootingUI shootingUI = UIManager.Instance.GetUI<ShootingUI>(UIList.ShootingUI);
                shootingUI.Show();
                break;
            }
            default:
            {

            }
            break;
        }
    }
}
