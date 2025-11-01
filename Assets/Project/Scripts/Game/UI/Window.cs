using UnityEngine;

public interface Window
{
    public void OpenWindow(GameObject obj)
    {
        obj.SetActive(true);
        OnOpen();
    }

    public void CloseWindow(GameObject obj)
    {
        obj.SetActive(false);
        OnClose();
    }

    public void SwitchWindow(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
        if (obj.activeSelf)
        {
            OpenWindow(obj);
        }
        else
        {
            CloseWindow(obj);
        }
    }

    public abstract void OnOpen();
    public abstract void OnClose();
}