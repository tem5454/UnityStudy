using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : UIBase
{
    public void OnClickGameStartBtn()
    {
        Main.Instance.ChangeScene(SceneType.Shooting);        
    }

    public void OnClickOptionBtn()
    {

    }

    public void OnClickExitBtn()
    {
        Application.Quit();
    }
}
