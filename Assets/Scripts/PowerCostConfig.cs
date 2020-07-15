using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CreatePowerConfig", order = 1)]
public class PowerCostConfig : ScriptableObject
{
	public List<int> powerCosts;

 	public void DownloadFromRemote(Action<bool> callback)
	{
		WebClient client = new WebClient();
		
		Stream data = client.OpenRead(@"https://www.gft.pl/kalkPV.htm");
		
		if (data == null)
		{
			callback(false);
			return;
		}
		
		StreamReader reader = new StreamReader(data);
		string s;
		int newValue;
		
		for (int i = 4; i < powerCosts.Count; i++)
		{
			s = reader.ReadLine();
			
			if(s == null)
				break;
			
			if (int.TryParse(s, out newValue))
			{
				powerCosts[i] = newValue;
			}
			else
			{
				callback(false);
				return;
			}
		}

		data.Close();
		reader.Close();

		callback(true);
	}
}