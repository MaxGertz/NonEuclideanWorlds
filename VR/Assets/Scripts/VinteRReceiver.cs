using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.XR;
using VinteR.Model.Gen;

public class VinteRReceiver : MonoBehaviour
{
    private const float roomscale = 0.001f;

    private UdpClient _optiTrackClient;
    //  private Quaternion _playerRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

    private IPEndPoint _optiTRackEndPoint = new IPEndPoint(IPAddress.Any, 3457);

    private Thread _optiTrackListener;
    //Optitrack Object Position

    private Vector3 _playerPosition = new Vector3(0.0f, 0.0f, 0.0f);

    private float _yOffset;
    public bool active = true;

    public bool disablePositionalTracking;

    [Tooltip("This is the Gameobject that represents the camera")]
    public Player player;

    private void Start()
    {
        Debug.Log("Starting OptiTrack Listener...");
        _optiTrackClient = new UdpClient(_optiTRackEndPoint);
        _optiTrackListener = new Thread(ReceiveOptiTrackData);
        _optiTrackListener.IsBackground = true;
        _optiTrackListener.Start();
        Debug.Log("Done!");
        InputTracking.disablePositionalTracking = disablePositionalTracking;
        _yOffset = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (active)
        {
            player.transform.position = _playerPosition;
            if (player.ActiveWorld) _yOffset = player.ActiveWorld.YOffset;
        }
    }

    private void ReceiveOptiTrackData()
    {
        Debug.Log("Listening...");
        while (true)
            try
            {
                //Debug.Log(OptiTrackClient.ToString());
                var data = _optiTrackClient.Receive(ref _optiTRackEndPoint);
                var mocapFrame = MocapFrame.Parser.ParseFrom(data);
                ParseMocapFrame(mocapFrame);
            }
            catch (Exception e)
            {
                return;
            }
    }

    private void ParseMocapFrame(MocapFrame mocapFrame)
    {
        if (mocapFrame != null)
            foreach (var body in mocapFrame.Bodies)
                if (mocapFrame.SourceId.Equals("optitrack"))
                {
                    var name = body.Name;

                    if (name.Equals("MaxGertz_GO_3"))
                    {
                        var x = body.Centroid.X;
                        var y = body.Centroid.Y;
                        var z = body.Centroid.Z;
                        var rot = new Quaternion(-body.Rotation.X, body.Rotation.Y, body.Rotation.Z,
                            -body.Rotation.W);
                        if (player.IsTeleported)
                        {
                            // y difference of the two worlds (in mm)
                            y += _yOffset;
                        }

                        var pos = new Vector3(z * roomscale, y * roomscale + 0.1f, x * roomscale);

                        _playerPosition = pos;
                    }
                }
    }

    private void OnDestroy()
    {
        _optiTrackListener.Abort();
        _optiTrackClient.Close();
    }

    private void OnQuitApplication()
    {
        _optiTrackListener.Abort();
        _optiTrackClient.Close();
    }
}