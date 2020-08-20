using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCam : MonoBehaviour
{
    public GameObject terrainCam;
    public GameObject stoneCam;
    AudioListener terrainAudioListener;
    AudioListener stoneAudioListener;


    // Start is called before the first frame update
    void Start()
    {
        //Get Camera Audio Listener
        terrainAudioListener = terrainCam.GetComponent<AudioListener>();
        stoneAudioListener = stoneCam.GetComponent<AudioListener>();

        cameraPositionChange(PlayerPrefs.GetInt("CameraPosition"));

    }


    //TODO when switching cameras I need to also switch, reference points for the foot tracking

    // Update is called once per frame
    void Update()
    {
        //
        SwitchCamera();

    }

    public void SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.Space)) cameraChangeCounter();
    }

    public void cameraChangeCounter()
    {
        int cameraPositionCounter = PlayerPrefs.GetInt("CameraPosition");
        cameraPositionCounter++;
        cameraPositionChange(cameraPositionCounter);

    }

    public void cameraPositionChange(int camPosition)
    {
        if(camPosition>1)
        {
            camPosition = 0;
        }

        //set camera position database
        PlayerPrefs.SetInt("CameraPosition", camPosition);

        // set camera position terrainCAM
        if(camPosition ==0)
        {
            terrainCam.SetActive(true);
            terrainAudioListener.enabled = true;

            stoneAudioListener.enabled = false;
            stoneCam.SetActive(false);
        }
        // set camera position stoneCAM
        if(camPosition ==1)
        {
            stoneCam.SetActive(true);
            stoneAudioListener.enabled = true;

            terrainAudioListener.enabled = false;
            terrainCam.SetActive(false);
        }
    }
}
