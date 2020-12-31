using System;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "DataPanelSize", menuName = "CreatePanelsConfig", order = 2)]
public class PanelsSizeConfig : ScriptableObject
{
	public List<Tuple<int, int>> PanelsPowerSize;
}