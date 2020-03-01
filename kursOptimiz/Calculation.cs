using System;
using System.Collections.Generic;
using System.Drawing;

namespace kursOptimiz
{
    class Calculation
    {
        double XYPointsCount;
        double FuncAccuracy;
        public static double Param1Min;
        public static double Param1Max;
        public static double Param2Min;
        public static double Param2Max;
        public int CalculationsCount { get; private set; }
        //TODO
        public double MainFunction(double A1, double A2 = 1)
        {
            //Нормирующие множители, равные 1  Даны по заданию
            double alpha = 1; double beta = 1; double nu = 1; double alpha1 = 1; double beta1 = 1; double nu1 = 1;
            //Рабочие объёмы реакторов Дано в задаче
            double V1 = 11; double V2 = 7;
            //Количество реакторов
            double N = 2;
            double func = alpha * Math.Pow((Math.Pow(A1, 2) + beta * A2 - nu * V1), N) - alpha1 * Math.Pow(beta1 * A1 + Math.Pow(A2, 2) - nu1 * V2, N);
            return func;
        }

        //TODO
        public static bool Contidions(double A1, double A2)
        {
            bool tmp = A1 >= 0 && A1 <= 10 && A2 >= 0 && A2 <= 10;
            return tmp && A1+A2<=8;
        }

        public void Calculate(out List<PointF> pts)
        {
            CalculationsCount = 0;
            double param1min = -1, param2min = -1, param1max = -1, param2max = -1;
            double funcMin = double.MaxValue;
            double funcMax = double.MinValue;
            //double accuracy = (Param1Max - Param1Min) / XYPointsCount;
            double accuracy = 1;
            pts = new List<PointF>();
            for (double Param2Coord = Param2Min; Param2Coord <= Param2Max; Param2Coord += accuracy)
                for (double Param1Coord = Param1Min; Param1Coord <= Param1Max; Param1Coord += accuracy)
                {
                    if (!Contidions(Param1Coord, Param2Coord))
                        continue;
                    double val = MainFunction(Param1Coord, Param2Coord);
                    CalculationsCount++;
                    if (funcMin - val > FuncAccuracy)
                    {
                        param1min = Param1Coord;
                        param2min = Param2Coord;
                        funcMin = val;
                    }
                    if (val - funcMax > FuncAccuracy)
                    {
                        param1max = Param1Coord;
                        param2max = Param2Coord;
                        funcMax = val;
                    }
                }
            pts = new List<PointF>();
            //if (searchForMax)
            //   pts.Add(new PointF((float)param1max, (float)param2max));
            //else
            pts.Add(new PointF((float)param1min, (float)param2min));

        }

        public void SetAccuracy(double accX, double accZ)
        {
            XYPointsCount = accX;
            FuncAccuracy = accZ;
        }
        public static void SetMinMax(double param1Min, double param1Max, double param2Min, double param2Max)
        {
            Param1Min = param1Min;
            Param1Max = param1Max;
            Param2Min = param2Min;
            Param2Max = param2Max;
        }
    }
}
