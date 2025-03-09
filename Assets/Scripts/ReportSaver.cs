using System;
using System.IO;
using System.Text;
using SFB;
using UnityEngine;

public class ReportSaver
{
#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    private void SaveFile()
    {
        byte[] bytes = Encoding.ASCII.GetBytes(JsonUtility.ToJson(report));
        DownloadFile(gameObject.name, "OnFileDownload", "report.tlrpt", bytes, bytes.Length);
    }
#else
    private void SaveFile()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "report", "tlrpt");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllText(path, JsonUtility.ToJson(report));
        }
    }
#endif
    private Report report;

    public ReportSaver(Report report)
    {
        this.report = report;
    }

    public void Save()
    {
        SaveFile();
    }
}