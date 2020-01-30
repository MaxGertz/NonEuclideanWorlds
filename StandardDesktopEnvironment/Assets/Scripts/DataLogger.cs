using System;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    public enum PlatformType
    {
        Desktop,
        Vr
    }

    private const string Delimiter = ";";

    private string _directory;
    private string _filePath;
    private bool _loggingStarted;
    private Player _player;
    private string _savedTextCompleteFilePath;

    private StreamWriter _sw;
    public GameObject[] objectsToLog = new GameObject[1];
    public string participantId = "PleaseEnter";
    public PlatformType platformType;

    private void Start()
    {
        _directory = "Study/";

        _directory = Path.Combine(_directory, participantId);

        if (!Directory.Exists(_directory))
        {
            Directory.CreateDirectory(_directory);
            Debug.Log("Logger - Created directory: " + _directory);
        }

        var timestamp = (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        _filePath = Path.Combine(_directory,
            timestamp + "_participant_" + participantId + "_platform_" + platformType + ".csv");
        Debug.Log("Logger - Logging directory: " + _filePath);
        _sw = new StreamWriter(_filePath);

        // Prepare header
        const string header = "timestamp" + Delimiter +
                              "gameobject_name" + Delimiter +
                              "pos_x" + Delimiter +
                              "pos_y" + Delimiter +
                              "pos_z" + Delimiter +
                              "euler_x" + Delimiter +
                              "euler_y" + Delimiter +
                              "euler_z";

        _sw.WriteLine(header);
        _sw.Flush();
    }

    private void Update()
    {
        if (!_player)
            _player = FindObjectOfType<Player>();
        else
            Log(_player.gameObject);

        // pressing "l" sets start/stop marker
        if (Input.GetKeyDown("l"))
        {
            if (!_loggingStarted)
            {
                Debug.Log("Logger - Logging start marker set");
                var startMarker = new GameObject("StartMarker");
                Log(startMarker);
                _loggingStarted = true;
                Destroy(startMarker);
            }
            else if (_loggingStarted)
            {
                Debug.Log("Logger - Logging stop marker set");
                var stopMarker = new GameObject("StopMarker");
                Log(stopMarker);
                _loggingStarted = false;
                Destroy(stopMarker);
            }
        }

        foreach (var obj in objectsToLog) Log(obj);
    }

    private void Log(GameObject obj)
    {
        if (!obj) return;

        var timestamp = (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        var objName = obj.name;
        var pos = obj.transform.position;
        var eulerAngles = obj.transform.rotation.eulerAngles;

        var row = timestamp + Delimiter +
                  objName + Delimiter +
                  pos.x + Delimiter +
                  pos.y + Delimiter +
                  pos.z + Delimiter +
                  eulerAngles.x + Delimiter +
                  eulerAngles.y + Delimiter +
                  eulerAngles.z + Delimiter;

        _sw.WriteLine(row);
        _sw.Flush();
    }

    private void OnDestroy()
    {
        _sw.Close();
    }

    [InitializeOnLoad]
    public class Startup
    {
        static Startup()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
        }
    }
}