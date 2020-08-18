/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;


public class StatisticsRecorder : MonoBehaviour {

    // todo: fix missing type Tuple
    List<Tuple<float, float>> cumulativeAngles;
    List<Tuple<float, float>> cumulativeDistances;
    float elapsedTime = 0;
    string path;
    float distanceTravelled = 0;
    float angleTravelled = 0; // measured in degrees
    Vector3 lastPos;
    int studyId;
	// Use this for initialization
	void Start () {
        path = FileIdBuilder();
        lastPos = transform.position;
        System.Random userFileIdGenerator = new System.Random();
        studyId = serFileIdGenerator.Next(1, 100000);
    }

    string FileIdBuilder(string name)
    {
        StringBuilder idBuilder = new StringBuilder();
        idBuilder.Append("Assets/Resources/");
        idBuilder.Append(name);
        idBuilder.Append(studyId);
        idBuilder.Append(".txt");
        return idBuilder.ToString();
    }
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;
        distanceTravelled += Vector3.Distance(transform.position, lastPos);
        angleTravelled += Vector3.Angle(lastPos, transform.forward);
        lastPos = transform.position;
        Debug.Log(elapsedTime);
        cumulativeAngles.Add(Tuple.Create(elapsedTime, angleTravelled));
        cumulativeDistances.Add(Tuple.Create(elapsedTime, distanceTravelled));
    }



    void OnApplicationQuit()
    {
        StreamWriter logWriter = new StreamWriter(path);
        logWriter.Write("ElapsedTime: ");
        logWriter.Write(elapsedTime);
        logWriter.Write(", ");
        logWriter.Write("Distance: ");
        logWriter.Write(distanceTravelled);
        logWriter.Write(", ");
        logWriter.Write("Angle: ");
        logWriter.Write(angleTravelled);
        logWriter.Close();

        StreamWriter cumulativeAnglesWriter = new SteamWriter(FileIdBuilder("cumulativeAngles"));
        for (int idx = 0; idx <= cumulativeAngles.Capacity; idx++)
        {
            cumulativeAnglesWriter.Write(string(idx));
            cumulativeAnglesWriter.Write(", ");
            cumulativeAnglesWriter.Write(string(cumulativeAngles[idx].Item1));
            cumulativeAnglesWriter.Write(", ");
            cumulativeAnglesWriter.Write(string(cumulativeAngles[idx].Item2));
            cumulativeAnglesWriter.Write("\n");
        }
        cumulativeAnglesWriter.Close();

        StreamWriter cumulativeDistancesWriter = new SteamWriter(FileIdBuilder("cumulativeDistances"));
        for (int idx = 0; idx <= cumulativeDistances.Capacity; idx++)
        {
            cumulativeDistancesWriter.Write(string(idx));
            cumulativeDistancesWriter.Write(", ");
            cumulativeDistancesWriter.Write(string(cumulativeAngles[idx].Item1));
            cumulativeDistancesWriter.Write(", ");
            cumulativeDistancesWriter.Write(string(cumulativeAngles[idx].Item2));
            cumulativeDistancesWriter.Write("\n");
        }
        cumulativeDistancesWriter.Close();
    }
}

*/