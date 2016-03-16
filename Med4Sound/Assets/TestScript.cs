using System;
using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System.Collections.Generic;
using System.Globalization;

public class TestScript : MonoBehaviour{

    Windows.Kinect.AudioSource aSource1;
    Windows.Kinect.AudioSource aSource2;

    public GameObject AudiGameObject;
    /// <summary>
    /// Number of samples captured from Kinect audio stream each millisecond.
    /// </summary>
    private const int SamplesPerMillisecond = 16;

    /// <summary>
    /// Number of bytes in each Kinect audio stream sample (32-bit IEEE float).
    /// </summary>
    private const int BytesPerSample = sizeof(float);

    /// <summary>
    /// Number of audio samples represented by each column of pixels in wave bitmap.
    /// </summary>
    private const int SamplesPerColumn = 40;

    /// <summary>
    /// Minimum energy of audio to display (a negative number in dB value, where 0 dB is full scale)
    /// </summary>
    private const int MinEnergy = -90;

    /// <summary>
    /// Width of bitmap that stores audio stream energy data ready for visualization.
    /// </summary>
    private const int EnergyBitmapWidth = 780;

    /// <summary>
    /// Height of bitmap that stores audio stream energy data ready for visualization.
    /// </summary>
    private const int EnergyBitmapHeight = 195;

     /// <summary>
    /// Array of background-color pixels corresponding to an area equal to the size of whole energy bitmap.
    /// </summary>
    private readonly byte[] backgroundPixels = new byte[EnergyBitmapWidth * EnergyBitmapHeight];

    /// <summary>
    /// Will be allocated a buffer to hold a single sub frame of audio data read from audio stream.
    /// </summary>
    private readonly byte[] audioBuffer = null;

    /// <summary>
    /// Buffer used to store audio stream energy data as we read audio.
    /// We store 25% more energy values than we strictly need for visualization to allow for a smoother
    /// stream animation effect, since rendering happens on a different schedule with respect to audio
    /// capture.
    /// </summary>
    private readonly float[] energy = new float[(uint)(EnergyBitmapWidth * 1.25)];

    /// <summary>
    /// Object for locking energy buffer to synchronize threads.
    /// </summary>
    private readonly object energyLock = new object();

    /// <summary>
    /// Active Kinect sensor
    /// </summary>
    private KinectSensor kinectSensor = null;

    /// <summary>
    /// Reader for audio frames
    /// </summary>
    private AudioBeamFrameReader reader = null;

    /// <summary>
    /// Last observed audio beam angle in radians, in the range [-pi/2, +pi/2]
    /// </summary>
    private float beamAngle = 0;

    /// <summary>
    /// Last observed audio beam angle confidence, in the range [0, 1]
    /// </summary>
    private float beamAngleConfidence = 0;

    /// <summary>
    /// Array of foreground-color pixels corresponding to a line as long as the energy bitmap is tall.
    /// This gets re-used while constructing the energy visualization.
    /// </summary>
    private byte[] foregroundPixels;

    /// <summary>
    /// Sum of squares of audio samples being accumulated to compute the next energy value.
    /// </summary>
    private float accumulatedSquareSum;

    /// <summary>
    /// Number of audio samples accumulated so far to compute the next energy value.
    /// </summary>
    private int accumulatedSampleCount;

    /// <summary>
    /// Index of next element available in audio energy buffer.
    /// </summary>
    private int energyIndex;

    /// <summary>
    /// Number of newly calculated audio stream energy values that have not yet been
    /// displayed.
    /// </summary>
    private int newEnergyAvailable;

    /// <summary>
    /// Error between time slice we wanted to display and time slice that we ended up
    /// displaying, given that we have to display in integer pixels.
    /// </summary>
    private float energyError;

    /// <summary>
    /// Last time energy visualization was rendered to screen.
    /// </summary>
    private DateTime? lastEnergyRefreshTime;

    /// <summary>
    /// Index of first energy element that has never (yet) been displayed to screen.
    /// </summary>
    private int energyRefreshIndex;


    AudioBeam aBeam;

    // Use this for initialization
    void Start () {
        kinectSensor = KinectSensor.GetDefault();
        aSource1 = kinectSensor.AudioSource;
        aBeam = aSource1.AudioBeams[0];
        //aSource2 = 
    }

    public Vector3 SyncSoundSource;
    // Update is called once per frame
    void Update() {
        //Debug.Log(kinectSensor.AudioSource.AudioBeams[0].BeamAngle + " beamangle 1");
        //aSource1.AudioBeams[0].
        if (Input.GetKeyDown(KeyCode.A))
        {
            //SampleAudio();
        }
    }


    public void SimpleAudioTracking()
    {
        
    }


    public List<byte[]> soundSamples = new List<byte[]>();

    public void SampleAudio()
    {

       AudioBeamFrameReader frameReader = aBeam.AudioSource.OpenReader();
       IList <AudioBeamFrame> beamFrames = frameReader.AcquireLatestBeamFrames();

        Debug.Log(beamFrames.Count);
        Debug.Log(beamFrames[0].SubFrames.Count);
        foreach(AudioBeamFrame frame in beamFrames)
        {
            for (int i = 0; i < frame.SubFrames.Count; i++) {
                byte[] temparr = new byte[1024];
                frame.SubFrames[i].CopyFrameDataToArray(temparr);
                soundSamples.Add(temparr);
                }
        }

        Debug.Log(soundSamples.Count);
    }

    /// <summary>
    /// Handles the audio frame data arriving from the sensor
    /// </summary>
    /// <param name="sender">object sending the event</param>
    /// <param name="e">event arguments</param>
    private void Reader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
    {
        AudioBeamFrameReference frameReference = e.FrameReference;
       // AudioBeamFrameList frameList = frameReference.AcquireBeamFrames();
        IList<AudioBeamFrame> frameList = frameReference.AcquireBeamFrames();
        //AudioBeamFrameList frameList = new AudioBeamFrameList( );

        //IList<AudioBeamFrame> frameList = frameReference.AcquireBeamFrames();

        if (frameList != null)
        {
                // Only one audio beam is supported. Get the sub frame list for this beam
                IList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                // Loop over all sub frames, extract audio buffer and beam information
                foreach (AudioBeamSubFrame subFrame in subFrameList)
                {
                    // Check if beam angle and/or confidence have changed
                    bool updateBeam = false;

                    if (subFrame.BeamAngle != this.beamAngle)
                    {
                        this.beamAngle = subFrame.BeamAngle;
                        updateBeam = true;
                    }

                    if (subFrame.BeamAngleConfidence != this.beamAngleConfidence)
                    {
                        this.beamAngleConfidence = subFrame.BeamAngleConfidence;
                        updateBeam = true;
                    }

                    if (updateBeam)
                    {
                        // Refresh display of audio beam
                        //this.AudioBeamChanged();
                    }

                    // Process audio buffer
                    subFrame.CopyFrameDataToArray(this.audioBuffer);

                    for (int i = 0; i < this.audioBuffer.Length; i += BytesPerSample)
                    {
                        // Extract the 32-bit IEEE float sample from the byte array
                        float audioSample = BitConverter.ToSingle(this.audioBuffer, i);

                        this.accumulatedSquareSum += audioSample * audioSample;
                        ++this.accumulatedSampleCount;

                        if (this.accumulatedSampleCount < SamplesPerColumn)
                        {
                            continue;
                        }

                        float meanSquare = this.accumulatedSquareSum / SamplesPerColumn;

                        if (meanSquare > 1.0f)
                        {
                            // A loud audio source right next to the sensor may result in mean square values
                            // greater than 1.0. Cap it at 1.0f for display purposes.
                            meanSquare = 1.0f;
                        }

                        // Calculate energy in dB, in the range [MinEnergy, 0], where MinEnergy < 0
                        float energy = MinEnergy;

                        if (meanSquare > 0)
                        {
                            energy = (float)(10.0 * Math.Log10(meanSquare));
                        }

                        lock (this.energyLock)
                        {
                            // Normalize values to the range [0, 1] for display
                            this.energy[this.energyIndex] = (MinEnergy - energy) / MinEnergy;
                            this.energyIndex = (this.energyIndex + 1) % this.energy.Length;
                            ++this.newEnergyAvailable;
                        }

                        this.accumulatedSquareSum = 0;
                        this.accumulatedSampleCount = 0;
                    }
                }
            }
        }

}


