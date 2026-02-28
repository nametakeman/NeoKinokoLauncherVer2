using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test : MonoBehaviour
{
    private string path = "C:/Users/souza/Downloads/corocorogame";
    private string inPath = "C:/Users/souza/Downloads/INcorocorogame";

    // Start is called before the first frame update
    void Start()
    {
        Test2();
        Debug.Log("End");
    }

    void Test1()
    {
        string zipFilePath = new FileSpliting().PackagingFile(path, inPath);

        new FileSpliting().DivideZipFile(100000, zipFilePath, inPath);
    }

    void Test2()
    {
        new FileCombine().MergeSplitedFile(inPath, "C:/Users/souza/Downloads");
    }
}
