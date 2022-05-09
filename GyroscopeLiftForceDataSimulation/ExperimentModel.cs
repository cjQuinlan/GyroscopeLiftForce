using System;
using System.Collections.Generic;
using System.Text;

namespace GyroscopeLevitationDataAnalysis
{
    class ExperimentModel
    {
        // Makes an experiment model to recreate data according to theoretical expectations
        public ExperimentModel(double airPressure_P0, double airPressure_C1, double angVel_M0, double gasDrag_C2, double gasDrag_C3, double shellDrag_C4, double shellDrag_C5, double unknownDrag_C6, double gasDrag_C7, double gasDrag_C8, double mainDrag_C9, double mainDrag_C10, double mainDrag_C11, double standDrag_C12, double standDrag_C13, double gasUpMain_C14, double gasDownShell_C15, double callibration_C16)
        {
            // Public fields
            AirPressure_P0 = airPressure_P0;
            AirPressure_C1 = airPressure_C1;
            AngVel_M0 = angVel_M0;
            GasDrag_C2 = gasDrag_C2;
            GasDrag_C3 = gasDrag_C3;
            ShellDrag_C4 = shellDrag_C4;
            ShellDrag_C5 = shellDrag_C5;
            UnknownDrag_C6 = unknownDrag_C6;
            GasDrag_C7 = gasDrag_C7;
            GasDrag_C8 = gasDrag_C8;
            MainDrag_C9 = mainDrag_C9;
            MainDrag_C10 = mainDrag_C10;
            MainDrag_C11 = mainDrag_C11;
            StandDrag_C12 = standDrag_C12;
            StandDrag_C13 = standDrag_C13;
            GasUpMain_C14 = gasUpMain_C14;
            GasDownShell_C15 = gasDownShell_C15;
            Callibration_C16 = callibration_C16;
            // Private fields
            Initial_AirPressure = airPressure_P0;
            Initial_AV_Main = angVel_M0;
            Initial_AV_Shell = 0;
        }

        private double Time { get; set; }
        private double DT { get; set; }
        private double Initial_AirPressure { get; set; }
        private double AirPressure { get; set; }
        private readonly double minAirPressure = 0.001; // Vacuum Says min value is 0.3 Pa = 0.000003 Atm
        private double Initial_AV_Main { get; set; }
        private double AV_Main { get; set; }
        private double D_AV_Main { get; set; }
        private double Initial_AV_Shell { get; set; }
        private double AV_Shell { get; set; }
        private double D_AV_Shell { get; set; }
        private double AV_Diff { get; set; }
        private double DeltaMass { get; set; }

        // Model of the Air Pressure
        // P(t) = P(0)exp(-C1*t)
        public double AirPressure_P0 { get; set; }
        public double AirPressure_C1 { get; set; }

        // Model of the angular velocity of the main gyroscope part
        // d(AV_Main)/dt = -P(t)*(C2*AV_Main + C3*AV_Main^2) - (C4*AV_Diff + C5*AV_Diff^2) - (C6*AV_Main)
        public double AngVel_M0 { get; set; }
        public double GasDrag_C2 { get; set; }
        public double GasDrag_C3 { get; set; }
        public double ShellDrag_C4 { get; set; }
        public double ShellDrag_C5 { get; set; }
        public double UnknownDrag_C6 { get; set; }

        // Model of the angular velocity of the shell part
        // d(AV_Shell)/dt = -P(t)*(C7*AV_Shell + C8*AV_Shell^2) + (C9*AV_Diff + C10*AV_Diff^2) + P(t)*(C11*AV_Diff) - (C12*AV_Diff + C13*AV_Diff^2)
        public double GasDrag_C7 { get; set; }
        public double GasDrag_C8 { get; set; }
        public double MainDrag_C9 { get; set; }
        public double MainDrag_C10 { get; set; }
        public double MainDrag_C11 { get; set; }
        public double StandDrag_C12 { get; set; }
        public double StandDrag_C13 { get; set; }

        // Model of contributions to the measured change in weight
        // Fluid Dynamics: DeltaMass(t) = P(t)*(C14*AV_Main^2 - C15*AV_Shell^2)
        public double GasUpMain_C14 { get; set; }
        public double GasDownShell_C15 { get; set; }
        // Calibration constant
        public double Callibration_C16 { get; set; }
        // Levitation Effect: DeltaMass(t) = (2.5*AV_Main^2 + 1.5*AV_Shell^2)*10^-9 grams

        private void stepModel(bool addLevitation)
        {
            // Air Pressure
            // P(t) = max(P(0)exp(-C1*t),minAirPressure)
            AirPressure = Math.Max(AirPressure_P0*Math.Exp(-AirPressure_C1*Time), minAirPressure);

            // Difference in angular velocities
            AV_Diff = AV_Main - AV_Shell;

            // Angular Velocity of Main Part
            // d(AV_Main)/dt = -P(t)*(C2*AV_Main + C3*AV_Main^2) - (C4*AV_Diff + C5*AV_Diff^2) - (C6*AV_Main)
            D_AV_Main = -1 * DT * (AirPressure * ((GasDrag_C2 * AV_Main) + (GasDrag_C3 * Math.Pow(AV_Main,2)) ) + ((ShellDrag_C4 * AV_Diff) + (ShellDrag_C5 * Math.Pow(AV_Diff, 2))) + (UnknownDrag_C6 * AV_Main));

            // Angular Velocity of Shell Part
            // d(AV_Shell)/dt = -P(t)*(C7*AV_Shell + C8*AV_Shell^2) + (C9*AV_Diff + C10*AV_Diff^2) + P(t)*(C11*AV_Diff) - (C12*AV_Diff + C13*AV_Diff^2)
            D_AV_Shell =  DT * (-1 * AirPressure * ((GasDrag_C7 * AV_Shell) + (GasDrag_C8 * Math.Pow(AV_Shell, 2))) + ((MainDrag_C9 * AV_Diff) + (MainDrag_C10 * Math.Pow(AV_Diff, 2))) + (AirPressure * MainDrag_C11 * AV_Diff) - ((StandDrag_C12 * AV_Shell) + (StandDrag_C13 * Math.Pow(AV_Shell, 2))));

            // Incriment Values by Rate
            Time += DT;
            AV_Main += D_AV_Main;
            AV_Shell += D_AV_Shell;

            // DeltaMass
            // Fluid Dynamics: DeltaMass(t) = P(t)*(C14*AV_Main^2 - C15*AV_Shell^2)
            DeltaMass = AirPressure * ((GasUpMain_C14 * Math.Pow(AV_Main, 2)) - (GasDownShell_C15 * Math.Pow(AV_Shell, 2)));
            // Calibration constant
            DeltaMass += Callibration_C16;
            // Levitation Effect!
            if (addLevitation)
            {
                double CorrectionFactor = 1.0; //6.238; // 2Pi
                DeltaMass += CorrectionFactor*((2.5 * Math.Pow(AV_Main, 2)) + (1.5 * Math.Pow(AV_Shell, 2))) * Math.Pow(10, -9);
            }
        }

        public DataPoint Measure()
        {
            return new DataPoint(Time, DeltaMass, AV_Main, AV_Shell, AirPressure);
        }

        public List<DataPoint> Run(double EndTime, bool addLevitation=true)
        {
            List<DataPoint> Simulation = new List<DataPoint>();
            int stepCount = 0;

            // Initial Conditions
            Time = 0;
            DT = 0.1;
            AirPressure = Initial_AirPressure;
            AV_Main = Initial_AV_Main;
            AV_Shell = Initial_AV_Shell;
            AV_Diff = AV_Main - AV_Shell;
            DeltaMass = 0;
            int stepsBeforeMeasure = (int)(5 / DT); // Should correspond to 5 seconds

            // Run Model using crude algorithim
            while (Time < EndTime)
            {
                stepModel(addLevitation);
                if(stepCount%stepsBeforeMeasure == 0)
                {
                    // Return a measurement for each 5 second interval to match the original data
                    Simulation.Add(Measure());
                }
                stepCount++;
            }

            return Simulation;
        }
    }
}
