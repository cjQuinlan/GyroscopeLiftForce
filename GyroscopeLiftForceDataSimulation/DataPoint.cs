using System;
using System.Collections.Generic;
using System.Text;

namespace GyroscopeLevitationDataAnalysis
{
    class DataPoint
    {
        public DataPoint(double seconds, double deltaMass, double aV_Main, double aV_Shell, double airPressure)
        {
            Seconds = seconds;
            DeltaMass = deltaMass;
            AV_Main = aV_Main;
            AV_Shell = aV_Shell;
            AirPressure = airPressure;
        }

        // Seconds
        public double Seconds { get; set; } 
        
        // Grams
        public double DeltaMass { get; set; }
        
        // Radians/Seconds
        public double AV_Main { get; set; }
        public double AV_Shell { get; set; }

        // Range from [0 = Perfect Vacuum, 1 = Normal STP]
        public double AirPressure { get; set; }
        


        public string ToDelimString(char delim)
        {
            int T = (int)Seconds;
            return T.ToString() + delim + DeltaMass.ToString() + delim + AV_Main.ToString() + delim + AV_Shell.ToString() + delim + AirPressure.ToString();
        }
    }
}
