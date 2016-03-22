using UnityEngine;
using System.Collections;
using Windows.Kinect;
using UnityEngine.Networking;

public class AudioCalculator : NetworkBehaviour
{
    //[SyncVar] private float beamAngle;
    //[SyncVar] private float beamAngle2;
    [SyncVar] public Vector3 TrackedVector3;

    public GameObject AudioTrackedGameObject;
    private Vector3 kinectOffset;
    private OffsetCalculator offsetCalculator;

    /// <summary>
    /// Active Kinect sensor
    /// </summary>
    private KinectSensor kinectSensor = null;


    // Use this for initialization
    void Start () {
        if (Network.isServer)
        {
            offsetCalculator = OffsetCalculator.offsetCalculator;
            kinectSensor = KinectSensor.GetDefault();
        }
    }

    private GameObject[] players;
    // Update is called once per frame
    void Update ()
	{
        //if (Network.isServer)
        //{
        offsetCalculator = OffsetCalculator.offsetCalculator;
	        if (offsetCalculator.players.Length > 0)
	        {
                float angle1 = Mathf.Rad2Deg * offsetCalculator.players[0].GetComponent<UserSyncPosition>().beamAngle;
                float angle2 = Mathf.Rad2Deg * offsetCalculator.players[1].GetComponent<UserSyncPosition>().beamAngle;

                Vector3 interSectionPoint = offsetCalculator.vectorIntersectionPoint(angle1, angle2);
                TrackedVector3 = interSectionPoint;
	            AudioTrackedGameObject.transform.position = TrackedVector3;
	        }
	   // }
	}

}
