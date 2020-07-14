/*
     * https://valvesoftware.github.io/steamvr_unity_plugin/api/Valve.VR.SteamVR_PlayArea.html#Valve_VR_SteamVR_PlayArea_GetBounds_Valve_VR_SteamVR_PlayArea_Size_Valve_VR_HmdQuad_t__
     * SteamVR_PlayArea.GetBounds(SteamVR_PlayArea.Size.Calibrated, ref>rect) - this returns a boolean
     * 
     * https://www.reddit.com/r/Vive/comments/4m28pl/finding_the_play_area_dimensions/
     * Someone talks about rescaling a cube?
     * 
     * 
     * [StructLayout(LayoutKind.Sequential)]
        public struct HmdQuad_t
        {
	        public HmdVector3_t vCorners0; //HmdVector3_t[4]
	        public HmdVector3_t vCorners1;
	        public HmdVector3_t vCorners2;
	        public HmdVector3_t vCorners3;
        }

        [StructLayout(LayoutKind.Sequential)] 
        public struct HmdVector3_t
        {
	        public float v0; //float[3]
	        public float v1;
	        public float v2;
        }
    */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class FenceGenPlayArea : MonoBehaviour {

    GameObject fenceGroup;
    Vector3[] corners = new Vector3[4];
    GameObject[] cornerObjects = new GameObject[4];
    public Transform steamRig;

    // Use this for initialization
    void Start () {
        fenceGroup = new GameObject("Fence group");
        fenceGroup.transform.position = steamRig.position;

        StartCoroutine(PlayArea());

        // Vector3 newScale = new Vector3(Mathf.Abs(rect.vCorners0.v0 - >rect.vCorners2.v0), this.transform.localScale.y, Mathf.Abs(rect.vCorners0.v2 - rect.vCorners2.v2));
    }

    IEnumerator PlayArea()
    {
        HmdQuad_t playAreaQuad = new HmdQuad_t();

        while (!SteamVR_PlayArea.GetBounds(SteamVR_PlayArea.Size.Calibrated, ref playAreaQuad))
            yield return new WaitForSeconds(0.1f);

        // v0,v1,v2 -> x,y,z

        corners[0] = new Vector3(playAreaQuad.vCorners0.v0, playAreaQuad.vCorners0.v1, playAreaQuad.vCorners0.v2);
        corners[1] = new Vector3(playAreaQuad.vCorners1.v0, playAreaQuad.vCorners1.v1, playAreaQuad.vCorners1.v2);
        corners[2] = new Vector3(playAreaQuad.vCorners2.v0, playAreaQuad.vCorners2.v1, playAreaQuad.vCorners2.v2);
        corners[3] = new Vector3(playAreaQuad.vCorners3.v0, playAreaQuad.vCorners3.v1, playAreaQuad.vCorners3.v2);

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(corners[i].ToString());
            cornerObjects[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cornerObjects[i].transform.parent = fenceGroup.transform;
            cornerObjects[i].transform.localPosition = corners[i];
            cornerObjects[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}