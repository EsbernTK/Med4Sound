using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class audioProccessing : MonoBehaviour {

    public AudioMixer mixer;
    public GameObject playerPrefab;
    public float playerSpeed;
    private GameObject player;
    private Vector2 movement;


    void Start () {
        player = Instantiate(playerPrefab, playerPrefab.transform.position, Quaternion.identity) as GameObject;

	}
	
	void Update () {
        playerMovement();
        audioStuff();
	}


    void audioStuff() {

        mixer.SetFloat("Pitch", Mathf.Abs(movement.x)/5);

        mixer.SetFloat("PitchShifter", Mathf.Abs(movement.y) / 5);

        if (movement.x < 0.2f && movement.x > -0.2f) {
            mixer.SetFloat("Pitch", 1);
        }
    }


    void playerMovement() {
        movement += new Vector2(Input.GetAxis("Horizontal")/playerSpeed, Input.GetAxis("Vertical")/playerSpeed);
        print(movement);
        player.transform.position = movement;
    }

}
