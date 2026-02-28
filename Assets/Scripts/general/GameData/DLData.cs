using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// データ通信時に分割したファイルの整合性をチェックするための情報を格納するクラス
/// string型がbyte[]として、.00に保存される
/// </summary>
public class DLData
{
    //zipファイルの容量
    public long GameSize { get; private set; }
    //zipファイルの名前
    public string FileName { get; private set; }
    //分割したファイルの個数
    public long SplitedFileNum { get; private set; }

    //クラス生成時にデータを格納するためのコンストラクタ。
    public DLData(long gameSize, string fileName, long splitedFileNum)
    {
        SetData(gameSize, fileName, splitedFileNum);
    }
    public DLData(byte[] byteData)
    {
        SetDataByByte(byteData);
    }

    /// <summary>
    /// データをセットするメソッド（""の場合はセットしない）
    /// </summary>
    /// <param name="gameSize"></param>
    /// <param name="fileName"></param>
    public void SetData(long gameSize, string fileName,long splitedFileNum)
    {
        if(gameSize != null) GameSize = gameSize;
        if(fileName != "") FileName = fileName;
        if(splitedFileNum != null) SplitedFileNum = splitedFileNum;
    }

    public void SetDataByByte(byte[] byteData)
    {
        string dlData = System.Text.Encoding.UTF8.GetString(byteData);

        string[] splitedDatas = dlData.Split(",");
        GameSize = long.Parse(splitedDatas[0]);
        FileName = splitedDatas[1];
        SplitedFileNum = long.Parse(splitedDatas[2]);
    }

    /// <summary>
    /// サイズとファイル名をバイト配列にして渡す(,で区切り)
    /// </summary>
    public byte[] ReturnByteData()
    {
        string toByteData = GameSize.ToString() + "," + FileName + "," + SplitedFileNum.ToString();
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(toByteData);
        return byteData;
    }
}
