using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonBase<UIManager>
{
    /// <summary> 2D Panel UI Container </summary>
    private Dictionary<UIList, UIBase> panels = new Dictionary<UIList, UIBase>();

    /// <summary> 2D Popup UI Container </summary>
    private Dictionary<UIList, UIBase> popups = new Dictionary<UIList, UIBase>();

    [SerializeField] private Transform panelRoot;
    [SerializeField] private Transform popupRoot;

    private const string UI_PATH = "UI/";

    public void Initialize()
    {
        if (panelRoot == null)
        {
            GameObject goPanelRoot = new GameObject("Panel Root");
            panelRoot = goPanelRoot.transform;
            panelRoot.parent = this.transform;
            panelRoot.localPosition = Vector3.zero;
            panelRoot.localRotation = Quaternion.identity;
            panelRoot.localScale = Vector3.one;
        }

        if (popupRoot == null)
        {
            GameObject goPopupRoot = new GameObject("Popup Root");
            popupRoot = goPopupRoot.transform;
            popupRoot.parent = this.transform;
            popupRoot.localPosition = Vector3.zero;
            popupRoot.localRotation = Quaternion.identity;
            popupRoot.localScale = Vector3.one;
        }

        for (int index = (int)UIList.SCENE_PANEL + 1; index < (int)UIList.MAX_SCENE_PANEL; index++)
        {
            panels.Add((UIList)index, null);
        }

        for (int index = (int)UIList.SCENE_POPUP + 1; index < (int)UIList.MAX_SCENE_POPUP; index++)
        {
            popups.Add((UIList)index, null);
        }
    }

    public T GetUI<T>(UIList uiName, bool reload = false) where T : UIBase
    {
        // Get Panel
        if (UIList.SCENE_PANEL < uiName && uiName < UIList.MAX_SCENE_PANEL)
        {
            if (panels.ContainsKey(uiName))
            {
                if (reload && panels[uiName] != null)
                {
                    Destroy(panels[uiName].gameObject);
                    panels[uiName] = null;
                }

                if (panels[uiName] == null)
                {
                    string path = UI_PATH + uiName.ToString();
                    GameObject loadedUI = Resources.Load<GameObject>(path) as GameObject;
                    if (loadedUI == null) return null;

                    T result = loadedUI.GetComponent<T>();
                    if (result == null) return null;

                    panels[uiName] = Instantiate(loadedUI, panelRoot).GetComponent<T>() as T;
                    if (panels[uiName]) panels[uiName].gameObject.SetActive(false);
                    return panels[uiName].GetComponent<T>();
                }
                else
                {
                    return panels[uiName].GetComponent<T>();
                }
            }
        }

        // Get Popup
        if (UIList.SCENE_POPUP < uiName && uiName < UIList.MAX_SCENE_POPUP)
        {
            if (popups.ContainsKey(uiName))
            {
                if (reload && popups[uiName] != null)
                {
                    Destroy(popups[uiName].gameObject);
                    popups[uiName] = null;
                }

                if (popups[uiName] == null)
                {
                    string path = UI_PATH + uiName.ToString();
                    GameObject loadedUI = Resources.Load<GameObject>(path) as GameObject;
                    if (loadedUI == null) return null;

                    T result = loadedUI.GetComponent<T>();
                    if (result == null) return null;

                    popups[uiName] = Instantiate(loadedUI, popupRoot).GetComponent<T>() as T;
                    if (popups[uiName]) popups[uiName].gameObject.SetActive(false);
                    return popups[uiName].GetComponent<T>();
                }
                else
                {
                    return popups[uiName].GetComponent<T>();
                }
            }
        }

        return null;
    }

    /// <summary> Hide All 2D Popup UI </summary>
    public void HideAllPopup()
    {
        foreach (var popup in popups.Values)
        {
            if (popup) popup.Hide();
        }
    }

    /// <summary> Hide All 2D Panel </summary>
    public void HideAllPanel()
    {
        foreach (var panel in panels.Values)
        {
            if (panel) panel.Hide();
        }
    }
}
