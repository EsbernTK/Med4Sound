using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class TestScript : MonoBehaviour{
    KinectSensor kinectSensor;
    Windows.Kinect.AudioSource aSource;
    // Use this for initialization
    void Start () {
        kinectSensor = KinectSensor.GetDefault();
	}

    // Update is called once per frame
    void Update() {
        Debug.Log(kinectSensor.AudioSource.AudioBeams[0].BeamAngle + " beamangle 1");
        aSource = kinectSensor.AudioSource;

        Debug.Log(aSource.AudioBeams.Count);
         //Debug.Log(kinectSensor.AudioSource.AudioBeams[1].BeamAngle + " beamangle 2");
    }
}
