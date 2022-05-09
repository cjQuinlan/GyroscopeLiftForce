using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace GyroscopeLevitationDataAnalysis
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Levitation Experiment Simulator");

            // =============================
            // Experimental Simulation Using VideoDataWithMagSheild1
            double AV_Main_0 = 2010;
            double AP_0 = 0.75;
            double AP_exp_C = 0.015;
            Program.RunExperimentModel("1", 2990, AV_Main_0, AP_0, AP_exp_C,
                @"D:\<Your Target Directory>\Simulation1.csv",
                @"D:\<Your Target Directory>\Simulation1_Control.csv");

            // =============================
            // Experimental Simulation Using VideoDataWithMagSheild3
            AV_Main_0 = 2300;
            AP_0 = 0.7;
            AP_exp_C = 0.013;
            Program.RunExperimentModel("3", 3265, AV_Main_0, AP_0, AP_exp_C,
                @"D:\<Your Target Directory>\Simulation2.csv",
                @"D:\<Your Target Directory>\Simulation2_Control.csv");
        }

        private static void RunExperimentModel(string index, int EndTime, double AV_Main_0, double AP_0, double AP_exp_C, string filePathLevitation, string filePathControl)
        {
            // Experimentally Measured Constants picked to match real data
            double linearGasDrag = 0.0025;
            double squareGasDrag = 0.0000005;
            double linearBearingDrag = 0.00045;
            double squareBearingDrag = 0.000002;
            double squareLift = 0.0000008;
            
            // Experiment Model Setup
            ExperimentModel Experiment = new ExperimentModel(AP_0,
                                                            AP_exp_C,
                                                            AV_Main_0,
                                                            gasDrag_C2: linearGasDrag,
                                                            gasDrag_C3: squareGasDrag,
                                                            shellDrag_C4: linearBearingDrag,
                                                            shellDrag_C5: squareBearingDrag,
                                                            unknownDrag_C6: 0,
                                                            gasDrag_C7: (linearGasDrag * 1.5),
                                                            gasDrag_C8: (squareGasDrag * 1.5),
                                                            mainDrag_C9: linearBearingDrag,
                                                            mainDrag_C10: squareBearingDrag,
                                                            mainDrag_C11: linearGasDrag,
                                                            standDrag_C12: linearBearingDrag,
                                                            standDrag_C13: squareBearingDrag,
                                                            gasUpMain_C14: squareLift,
                                                            gasDownShell_C15: squareLift,
                                                            callibration_C16: 0.001);
            
            // Levitation Simulation
            List<DataPoint> Simulation = Experiment.Run(EndTime, true);
            Console.WriteLine("Running Simulation With Levitation...");
            List<string> output = new List<string>();
            string Headers = "Time "+ index + ",DM Levitation " + index + ",AV Main " + index + ",AV Shell " + index + ",Air Pressure " + index;
            output.Add(Headers);
            foreach (DataPoint dp in Simulation)
            {
                output.Add(dp.ToDelimString(','));
            }
            Console.WriteLine("Printing To: " + filePathLevitation);
            File.WriteAllLines(filePathLevitation, output);

            // Control Simulation
            List<DataPoint> Simulation_Control = Experiment.Run(EndTime, false);
            Console.WriteLine("Running Simulation Without Levitation...");
            List<string> output_Control = new List<string>();
            Headers = "Time " + index + ",DM Control " + index + ",AV Main " + index + ",AV Shell " + index + ",Air Pressure " + index;
            output_Control.Add(Headers);
            foreach (DataPoint dp in Simulation_Control)
            {
                output_Control.Add(dp.ToDelimString(','));
            }
            Console.WriteLine("Printing To: " + filePathControl);
            File.WriteAllLines(filePathControl, output_Control);
            Console.WriteLine("Done.");
        }
    }
}
