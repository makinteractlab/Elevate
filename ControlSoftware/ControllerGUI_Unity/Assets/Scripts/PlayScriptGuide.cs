using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//use newtonsoft
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Ports;

public class PlayScriptGuide : MonoBehaviour
{
    Play myPlay = new Play();

    void Start()
    {

    }

    //example of sendToQueue
    void useSendToQueue()
    {
        JObject exampleJobj = new JObject(); // put your JObject in here
        myPlay.sendToQueue(exampleJobj);
    }

    //example of isReady
    void useIsReady()
    {
        if (myPlay.isReady()) Debug.Log("It is ready");
        else Debug.Log("It is Playing now");
    }

    //example of getTimeleft
    void useGetImeLeft()
    {
        int time = myPlay.getTimeleft();
        Debug.Log(time + "sec left");
    }
}