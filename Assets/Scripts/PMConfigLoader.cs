using System;
using System.Collections;
using SFB;
using UnityEngine;

public class PMConfigLoader
{
    string fileExtension;
    Action<ProjectManagerConfig> onLoadComplete;
    MonoBehaviour monoBehaviour;

    public PMConfigLoader(MonoBehaviour monoBehaviour, string fileExtension, Action<ProjectManagerConfig> onLoadComplete)
    {
        this.fileExtension = fileExtension;
        this.onLoadComplete = onLoadComplete;
        this.monoBehaviour = monoBehaviour;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);
    
    private string StartImport()
    {
        UploadFile(gameObject.name, "OnFileUpload", $".{fileExtension}", false);
    }

    public void OnFileUpload(string url) {
        monoBehaviour.StartCoroutine(OutputRoutine(url));
    }
#else
    private void StartImport()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select Project Manager Config", "", fileExtension, false);
        if (paths.Length > 0) {
            monoBehaviour.StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutine(string url) {
        var loader = new WWW(url);
        yield return loader;
        onLoadComplete(JsonUtility.FromJson<ProjectManagerConfig>(loader.text));
    }

    public void StartLoad()
    {
        StartImport();
    }

}
