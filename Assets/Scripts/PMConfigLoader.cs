using System;
using System.Collections;
using SFB;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PMConfigLoader : MonoBehaviour, IPointerDownHandler
{
    string fileExtension = "pmcfg";
    public static Action<ProjectManagerConfig> onLoadComplete = delegate { };

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", $".{fileExtension}", false);
    }

    public void OnFileUpload(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    public void OnPointerDown(PointerEventData eventData) { }

    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    
    private void StartImport()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Project Manager Config", "", fileExtension, false);
        if (paths.Length > 0) {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }

    public void OnClick()
    {
        StartImport();
    }
#endif

    public IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;
        onLoadComplete(JsonUtility.FromJson<ProjectManagerConfig>(loader.text));
    }
}
