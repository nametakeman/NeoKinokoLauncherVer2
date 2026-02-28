using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class FileCombine
{
    //不足したファイルがあった際に呼ばれるデリゲート
    public Action<List<long>> IsLackFile;

    /// <summary>
    /// 分割されたZIPファイルを結合するメソッド
    /// </summary>
    /// <param name="splitedFilesPath">結合されるファイル群が置かれているディレクトリのパス</param>
    public void MergeSplitedFile(string splitedFilesPath, string margedFileInDirPath)
    {
        //対象ディレクトリ内のファイルのパスを取得してくる
        string[] splitedFiles = Directory.GetFiles(splitedFilesPath);
        //取得したファイルパス群から、語尾の数値でソートを行う(数値以外の部分は共通しているため、単純にソートを行うことが可能)
        //ToArray()はキャッシュ化してLINQの遅延実行を無視するため
        string[] sortedFiles = splitedFiles.OrderBy(f => f).ToArray();


        //.00のデータからファイル名・データのサイズを取り出す
        if (Path.GetExtension(sortedFiles[0]) != ".00")
        {
            //.00のファイルが無かった際のエラー処理(例外を指定する)
            throw new Exception();
        }

        byte[] dlDatabytes = File.ReadAllBytes(sortedFiles[0]);
        DLData targetDLData = new DLData(dlDatabytes);
        string dlFileName = targetDLData.FileName;
        long splitedFileNum = targetDLData.SplitedFileNum;

        //全ての分割されたファイルが揃っているかを結合前に確認する
        long checkCounter = 0;
        List<long> lackFileNumber = new List<long>();
        List<long> passFileIndex = new List<long>();
        for(int i = 0; i < sortedFiles.Length; i++)
        {
            //ファイル名の取得
            string fileName = Path.GetFileName(sortedFiles[i]);
            string[] splitedFileName = fileName.Split('.');
            string gameName = splitedFileName[0];
            long fileNumber = long.Parse(splitedFileName[1]);


            if(gameName != dlFileName)
            {
                passFileIndex.Add(i);
                continue;
            }

            //同じファイル番号を持つファイルが連続していた場合
            if(fileNumber < checkCounter)
            {
                passFileIndex.Add(i);
                checkCounter--;
            }
            //ファイルが一つ飛んでいる場合
            else if(fileNumber > checkCounter)
            {
                lackFileNumber.Add(fileNumber);
                i--;
            }
            checkCounter++;
        }

        if(lackFileNumber.Count() != 0)
        {
            //ファイル欠損時に呼ぶメソッドを呼んで処理を終了
            IsLackFile.Invoke(lackFileNumber);
            Debug.Log("ファイルの欠損を確認しました");
            return;
        }

        string margedFilePath = Path.Combine(margedFileInDirPath, dlFileName + ".zip");
        //データを結合する処理
        using (FileStream outFs = new FileStream(margedFilePath, FileMode.Create,FileAccess.Write))
        {
            for (int i = 1; i < sortedFiles.Length; i++)
            {
                if (passFileIndex.Contains(i)) continue;

                byte[] bytedatas = File.ReadAllBytes(sortedFiles[i]);
                outFs.Write(bytedatas, 0, bytedatas.Length);
            }
        }

        //結合して出来たZIPファイルを解凍する
        ZipFile.ExtractToDirectory(margedFilePath, Path.Combine(margedFileInDirPath, dlFileName), Encoding.GetEncoding("shift_jis"));

        //ZIPファイルを削除する
        System.IO.File.Delete(margedFilePath);
    }
}
