using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "DataFalownik", menuName = "CreateFalownikConfig", order = 4)]
    public class FalownikPDFConfig : RemoteConfig
    {
        protected override string DownloadURL => @"https://www.gft.pl/falownikiPV.htm";
        public List<Producer> producers = new List<Producer>();
        protected override bool ParseString(StreamReader reader)
        {
            string s;
            string[] values;
            int newValueMin;
            int newValueMax;
            int extra;
            string lastName = "";
            Producer lastProducer = new Producer();
            producers = new List<Producer>();

            for (int i = 0; i < 1000; i++)
            {
                if (reader.EndOfStream)
                    break;

                s = reader.ReadLine();

                if (s == null)
                    return false;
                
                Debug.Log(s);

                values = s.Split(',');

                if (lastName != values[0])
                {
                    lastName = values[0];
                    lastProducer = new Producer();
                    lastProducer.name = lastName;
                    producers.Add(lastProducer);
                }

                if (int.TryParse(values[1], out newValueMin) 
                    && int.TryParse(values[2], out newValueMax)
                    && int.TryParse(values[4], out extra))
                {
                    var catalog = new Catalog();
                    catalog.powerMin = newValueMin;
                    catalog.powerMax = newValueMax;
                    catalog.extraCost = extra;
                    catalog.url = values[3];
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
            public int powerMin;
            public int powerMax;
            public string url;
            public int extraCost;
        }
    }
}