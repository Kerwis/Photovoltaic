using System.IO;
using UnityEngine;

namespace Config
{
    
    [CreateAssetMenu(fileName = "DataGround", menuName = "CreateGroundConfig", order = 1)]
    public class GroundPriceConfig : RemoteConfig
    {
        protected override string DownloadURL => @"https://www.gft.pl/GroundPrice.htm";
        public float groundExtraCost = 90;
        public float ceramicsExtraCost = 60;

        protected override bool ParseString(StreamReader reader)
        {
            string s;
            float newValue;

            s = reader.ReadLine();

            if (s == null)
            {
                return false;
            }

            if (float.TryParse(s, out newValue))
            {
                groundExtraCost = newValue;
            }
            else
            {
                return false;
            }

            s = reader.ReadLine();

            if (s == null)
            {
                return false;
            }

            if (float.TryParse(s, out newValue))
            {
                ceramicsExtraCost = newValue;
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}