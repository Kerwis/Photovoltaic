using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "DataPanels", menuName = "CreatePanelsConfig", order = 3)]
    public class PanelsPDFConfig : RemoteConfig
    {
        protected override string DownloadURL => @"https://www.gft.pl/panelePV.htm";
        public List<Producer> producers = new List<Producer>();
        protected override bool ParseString(StreamReader reader)
        {
            string s;
            string[] values;
            int powerRead;
            int costRead;
            string lastName = "";
            Producer lastProducer = new Producer();
            producers = new List<Producer>();
		
            for (int i = 0; i < 1000; i++)
            {
                if(reader.EndOfStream)
                    break;
                
                s = reader.ReadLine();

                if (s == null)
                    return false;

                values = s.Split(',');

                if (lastName != values[0])
                {
                    lastName = values[0];
                    lastProducer = new Producer();
                    lastProducer.name = lastName;
                    producers.Add(lastProducer);
                }

                if (int.TryParse(values[1], out powerRead) && int.TryParse(values[3], out costRead))
                {
                    var catalog = new Catalog();
                    catalog.power = powerRead;
                    catalog.extraCost = costRead;
                    catalog.url = values[2];
                    lastProducer.catalogs.Add(catalog);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
        [Serializable]
        public class Producer
        {
            public string name;
            public List<Catalog> catalogs = new List<Catalog>();
        }

        [Serializable]
        public class Catalog
        {
            public int power;
            public string url;
            public int extraCost;
        }
    }
}