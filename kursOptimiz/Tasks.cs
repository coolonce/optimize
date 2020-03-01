using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace kursOptimiz
{
    class Tasks
    {
        private Dictionary<string, string> tasksList = new Dictionary<string, string>();
        private Dictionary<string, double[]> granPoint = new Dictionary<string, double[]>();
        private Dictionary<string, string> formalTaskList = new Dictionary<string, string>();
        private Dictionary<string, bool> searchForMin = new Dictionary<string, bool>();
        private Dictionary<string, string> resultOutputMessages = new Dictionary<string, string>();
        static double XYPointsCount;
        static double FuncAccuracy;
        public static double Param1Min;
        public static double Param1Max;
        public static double Param2Min;
        public static double Param2Max;
        public static int CalculationsCount { get; private set; }


        public Tasks()
        {
            InitTasks();
        }

        public static int GetCalculations()
        {
            return CalculationsCount;
        }

        private void InitTasks()
        {
            tasksList.Add("Задание Винокурова Никиты", "1");
            granPoint.Add("1", new double[] { 1, 10, 1, 10 });
            string task = "a *(A1^2 + b*A2 - u*V1)^N +\na1 *(A1*b1 + A2^2 - u1*V2)^N";
            string message = "Максимальный выход за смену достигается при следующих значениях A1=: и A2=: и равен : кг/смена";
            formalTaskList.Add("1", task);
            searchForMin.Add("1", false);
            resultOutputMessages.Add("1", message);

            tasksList.Add("Задание Татаринцева Вадима", "2");
            granPoint.Add("2", new double[] { -5, 0, -1, 5 });
            task = "a *G *(t1^2 + beta*t2 + u*deltaP1)^N +\ng* (t1*b1 + t2^2 +u 1*deltaP2)^N";
            message = "Минимальная себестоимость фильтрата за смену достигается при следующих значениях T1=: и T2=: и равна : у.е./смена";
            formalTaskList.Add("2", task);
            searchForMin.Add("2", true);
            resultOutputMessages.Add("2", message);

            tasksList.Add("Задание Филимоновой Марины", "3");
            granPoint.Add("3", new double[] { -2, 15, -2, 12 });
            task = "a*G*( (T1 - T2)^2 + b/H *\n(T1 + T2 - у*N)^2)";
            message = "Максимальная прибыль от реализации целевого компонента достигается при следующих значенияъ T1=: и T2=: и равна : у.е./час";
            formalTaskList.Add("3", task);
            searchForMin.Add("3", false);
            resultOutputMessages.Add("3", message);

            tasksList.Add("Задание Смирнова Кирилла", "4");
            granPoint.Add("4", new double[] { 1, 10, 1, 10});
            task = "a *(A1^2 + b*A2 - u*V1)^N +\na1 *(A1*b1 + A2^2 - u1*V2)^N";
            message = "Максимальная прибыль от реализации целевого компонента за смену достигается при следующих значенияъ A1=: и A2=: и равна : у.е.";
            formalTaskList.Add("4", task);
            searchForMin.Add("4", false);
            resultOutputMessages.Add("4", message);

            CalculationsCount = 0;
        }
        public Dictionary<string, string> GetTaskList()
        {
            return tasksList;
        }

        public Dictionary<string, double[]> GetGranPoint()
        {
            return granPoint;
        }

        public Dictionary<string, string> GetFormalTaskList()
        {
            return formalTaskList;
        }

        public Dictionary<string,string> GetOutMessage()
        {
            return resultOutputMessages;
        }

        public Dictionary<string, bool> GetSearchingForMin()
        {
            return searchForMin;
        }

        public static double MainFunction3(double t1, double t2)
        {
            double alpha = 1; double beta = 1; double nu = 1;
            //Раскод реакционной массы (2кг/ч) Данно в задаче 
            double G = 1.0;
            //Давление в реакторе (1Кпа) 
            double A = 9;
            //Скорорость вращения мешалки(2об/с) 
            double N = 10;
            return 100*alpha * G * (Math.Pow(t1 - t2, 2) + beta + (1 / A) + Math.Pow(t1 + t2 - nu * N, 2));
        }

        //TODO
        public static bool Contidions3(double t1, double t2)
        {
            bool firstMin = t1 >= Param1Min;
            bool secondMin = t1 <= Param1Max;
            bool thirdMin = t2 >= Param2Min;
            bool fourthMin = t2 <= Param2Max;
            bool fifthCond = ((t1 + t2) <= 12);
            return firstMin && secondMin && thirdMin && fourthMin && fifthCond;
        }


        public static double MainFunction2(double t1, double t2)
        {
            double alpha = 1; double beta = 1; double nu = 1; double gamma= 1; double beta1 = 1;double nu1 = 1;
            //Раскод реакционной массы (2кг/ч) Данно в задаче 
            double G = 1.0;
            //величины перепадов давления на перегородках
            double deltaP1 = 11.0; double deltaP2 = 7.0;
            //Давление в реакторе (1Кпа) 
            double A = 9;
            //P - цена за кубометр жидкости
            double P = 10;
            //Количество перегородок
            double N = 2;
            return 80*alpha * G * Math.Pow((Math.Pow(t1, 2) + beta*t2 - nu*deltaP1),N) + 80*gamma*Math.Pow(beta1*t1 + Math.Pow(t2,2) - nu1*deltaP2, N);
        }

        //TODO
        public static bool Contidions2(double t1, double t2)
        {
            bool firstMin = t1 >= Param1Min;
            bool secondMin = t1 <= Param1Max;
            bool thirdMin = t2 >= Param2Min;
            bool fourthMin = t2 <= Param2Max;
            bool fifthCond = ((0.5 * t1 + t2) <= 12);
            return firstMin && secondMin && thirdMin && fourthMin && fifthCond;
        }

        public static double MainFunction1(double A1, double A2)
        {
            //Нормирующие множители, равные 1  Даны по заданию
            double alpha = 1; double beta = 1; double nu = 1; double alpha1 = 1; double beta1 = 1; double nu1 = 1;
            //Рабочие объёмы реакторов Дано в задаче
            double V1 = 11; double V2 = 7;
            //Количество реакторов
            double N = 2;
            return 8*alpha * Math.Pow(Math.Pow(A1, 2) + beta * A2 - nu * V1, N) + 8*alpha1 * Math.Pow(Math.Pow(A2, 2) + beta1 * A1 - nu1 * V2, N);
        }

        //TODO
        public static bool Contidions1(double t1, double t2)
        {
            bool firstMin = t1 >= Param1Min;
            bool secondMin = t1 <= Param1Max;
            bool thirdMin = t2 >= Param2Min;
            bool fourthMin = t2 <= Param2Max;
            bool fifthCond = ((t1 + t2) <= 8);
            return firstMin && secondMin && thirdMin && fourthMin && fifthCond;
        }

        public static bool Contidions4(double t1, double t2)
        {
            bool firstMin = t1 >= Param1Min;
            bool secondMin = t1 <= Param1Max;
            bool thirdMin = t2 >= Param2Min;
            bool fourthMin = t2 <= Param2Max;
            bool fifthCond = ((4*t1 + 5*t2) <= 20);
            return firstMin && secondMin && thirdMin && fourthMin && fifthCond;
        }

        public static double MainFunction4(double A1, double A2)
        {
            //Нормирующие множители, равные 1  Даны по заданию
            double alpha = 1; double beta = 1; double nu = 1; double alpha1 = 1; double beta1 = 1; double nu1 = 1;
            //Рабочие объёмы реакторов Дано в задаче
            double V1 = 11; double V2 = 7;
            //Количество реакторов
            double N = 2;
            return 800 * alpha * Math.Pow(Math.Pow(A1, 2) + beta * A2 - nu * V1, N) + 800 * alpha1 * Math.Pow(Math.Pow(A2, 2) + beta1 * A1 - nu1 * V2, N);
        }


        public static void SetAccuracy(double accX, double accZ)
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


        public static List<PointF> Calculate(out List<PointF> pts, MethodInfo mainF, MethodInfo condF, bool sMin)
        {
            CalculationsCount = 0;
            double param1min = -1, param2min = -1, param1max = -1, param2max = -1;
            double funcMin = double.MaxValue;
            double funcMax = double.MinValue;
            //double accuracy = (Param1Max - Param1Min) / XYPointsCount;
            double accuracy = 0.1;
            pts = new List<PointF>();
            for (double Param2Coord = Param2Min; Param2Coord <= Param2Max; Param2Coord += accuracy)
                for (double Param1Coord = Param1Min; Param1Coord <= Param1Max; Param1Coord += accuracy)
                {
                    if (!Convert.ToBoolean(condF.Invoke(null, new object[] { Param1Coord, Param2Coord })))
                    {
                        continue;
                    }
                    //double val = MainFunction(Param1Coord, Param2Coord);
                    double val = Convert.ToDouble(mainF.Invoke(null, new object[] { Param1Coord, Param2Coord }));
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
            if (sMin)
            {
                pts.Add(new PointF((float)param1min, (float)param2min));
                pts.Add(new PointF((float)param1max, (float)param2max));
            }
            else
            {
                pts.Add(new PointF((float)param1max, (float)param2max));
                pts.Add(new PointF((float)param1min, (float)param2min));
            }
            return pts;
        }
    }
}
