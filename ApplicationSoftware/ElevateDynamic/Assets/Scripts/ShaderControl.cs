using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderControl : MonoBehaviour
{

    public bool[] ok2Go;
    public Material greenLight;
    public Material defaultMat;
    private int[] targetHeight;
    public float speed;
    public float baseLevel = 100;

    public bool ready;
    // Start is called before the first frame update
    void Start()
    {
        ok2Go = new bool[transform.childCount];
        targetHeight = new int[transform.childCount];


        // save the target height
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform stone = transform.GetChild(i);
            stone.GetComponent<Renderer>().material = defaultMat;
            targetHeight[i] = (int)stone.position.y;
        }




    }

    // Update is called once per frame
    void Update()
    {
        if(ready)
        {
            for (int i = 0; i < transform.childCount; i++) updateShader(i);
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).position = new Vector3(transform.GetChild(i).position.x, targetHeight[i], transform.GetChild(i).position.z);

            }
        }


        if(Input.GetKeyDown(KeyCode.Q))
        {
            ok2Go[0] = true;
        }
    }


    void updateShader(int index) 
    {
        float height = transform.GetChild(index).position.y;
        Transform stone = transform.GetChild(index);
        if (ok2Go[index])
        {
            if (height < targetHeight[index])
            {
                stone.position = new Vector3(stone.position.x, stone.position.y + (Time.deltaTime * speed), stone.position.z);
            }
            else
            {
                transform.GetChild(index).GetComponent<Renderer>().material = greenLight;
            }
        }

        else
        {
            if (height > targetHeight[index]- baseLevel)
            {
                stone.position = new Vector3(stone.position.x, stone.position.y - (Time.deltaTime * speed), stone.position.z);
            }
            else
            {
                transform.GetChild(index).GetComponent<Renderer>().material = defaultMat;
            }
        }
    }

    //TODO sort the object order by distance
    public void sortbyDistance()
    {

    }


}
