using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CsvTest : MonoBehaviour {
	void Start () 
	{
		using (var writer = new CsvFileWriter("Assets/Scripts/Resources/CoinData.csv"))
		{
			List<string> columns = new List<string>(){"Coin"};// making Index Row
			writer.WriteRow(columns);
			columns.Clear();

			columns.Add("99");
			writer.WriteRow(columns);
			columns.Clear();
		}
	}
}
