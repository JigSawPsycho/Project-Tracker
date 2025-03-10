using System;
using System.IO;
using System.Text;
using SFB;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScreenshotSaver : MonoBehaviour, IPointerDownHandler
{
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
        Texture2D screenshot = CaptureScreenshotTexture();
        byte[] bytes = screenshot.EncodeToJPG(100);
        DownloadFile(gameObject.name, "OnFileDownload", "report.jpg", bytes, bytes.Length);
    }
#else

    public void OnPointerDown(PointerEventData eventData) { }

    private void SaveFile()
    {
        onPreFileDownload();
        Texture2D screenshot = CaptureScreenshotTexture();
        var path = StandaloneFileBrowser.SaveFilePanel("Save Report Image", "", "report", "jpg");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllBytes(path, screenshot.EncodeToJPG(100));
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

    public Texture2D CaptureScreenshotTexture()
    {
        gameObject.SetActive(false);
        Texture2D tex = ScreenCapture.CaptureScreenshotAsTexture(8);
        gameObject.SetActive(true);
        return tex;
    }

    public void OnFileDownload() {
        onFileDownloaded();
    }
}