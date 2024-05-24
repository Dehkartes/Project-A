using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    private void Start()
    {
        var sheet = new ES3Spreadsheet();
        string path = Application.persistentDataPath + @"/Assets/Scripts/Resources/CoinData.csv";
        //Debug.Log(path);
        sheet.Load("CoinData");
        //sheet.Load("CoinData.csv");

        for (int col = 0; col < sheet.ColumnCount; col++)
            Debug.Log(sheet.GetCell<int>(col, 0));

        //var easy = ES3.Load("Coin", "CoinData.csv");
        //Debug.Log(sheet.GetCell<int>(0, 0));
        //Debug.Log(sheet);

    }

}
