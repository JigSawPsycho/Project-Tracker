using System;
using System.IO;
using System.Text;
using SFB;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class PMConfigSaver : MonoBehaviour, IPointerDownHandler
{
    public static ProjectManagerConfig pmConfig;
    public static Action onPreFileDownload = delegate { };
    public static Action onFileDownloaded = delegate { };
#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    public void OnPointerDown(PointerEventData eventData) {
        onPreFileDownload();
        if(pmConfig == null) Debug.LogError("pmConfig is null!");
        byte[] bytes = Encoding.ASCII.GetBytes(JsonUtility.ToJson(pmConfig));
        DownloadFile(gameObject.name, "OnFileDownload", "pmconfig.pmcfg", bytes, bytes.Length);
    }
#else

    public void OnPointerDown(PointerEventData eventData) { }

    private void SaveFile()
    {
        onPreFileDownload();
        if(pmConfig == null) Debug.LogError("pmConfig is null!");
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "pmconfig", "pmcfg");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllText(path, JsonUtility.ToJson(pmConfig));
        }
        OnFileDownload();
    }
    
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    
    public void OnClick()
    {
        SaveFile();
    }

#endif

    public void OnFileDownload() {
        onFileDownloaded();
    }
}