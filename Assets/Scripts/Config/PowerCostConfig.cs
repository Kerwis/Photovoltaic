using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

[CreateAssetMenu(fileName = "DataPower", menuName = "CreatePowerConfig", order = 1)]
public class PowerCostConfig : RemoteConfig
{
	protected override string DownloadURL => @"https://www.gft.pl/kalkPV.htm";
	public List<int> powerCosts;
	protected override bool ParseString(StreamReader reader)
	{
		string s;
		int newValue;
		
		for (int i = 4; i < powerCosts.Count; i++)
		{
			s = reader.ReadLine();

			if (s == null)
				return false;
			
			if (int.TryParse(s, out newValue))
			{
				powerCosts[i] = newValue;
			}
			else
			{
				return false;
			}
		}

		return true;
	}
}