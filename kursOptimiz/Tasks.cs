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
            granPoint.Add("4", new double[] { 1, 10, 1, 10 });
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

        public Dictionary<string, string> GetOutMessage()
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
            return 100 * alpha * G * (Math.Pow(t1 - t2, 2) + beta + (1 / A) + Math.Pow(t1 + t2 - nu * N, 2));
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
            double alpha = 1; double beta = 1; double nu = 1; double gamma = 1; double beta1 = 1; double nu1 = 1;
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
            return 80 * alpha * G * Math.Pow((Math.Pow(t1, 2) + beta * t2 - nu * deltaP1), N) + 80 * gamma * Math.Pow(beta1 * t1 + Math.Pow(t2, 2) - nu1 * deltaP2, N);
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
            return 8 * alpha * Math.Pow(Math.Pow(A1, 2) + beta * A2 - nu * V1, N) + 8 * alpha1 * Math.Pow(Math.Pow(A2, 2) + beta1 * A1 - nu1 * V2, N);
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
            bool fifthCond = ((4 * t1 + 5 * t2) <= 20);
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
        private static int n_recurse = 0;
        public static List<PointF> MethodBox(out List<PointF> pts, MethodInfo mainF, MethodInfo condF, bool sMin)
        {
            //n - кол-во независеммых переменных
            int n = 2;
            //Длинна комлекса 2 * n при n <= 5  
            int N = 2 * n;
            double[,] x = CalcComplex(n, N);
            int P = CheckComplex(x, n, N);
            x = CalcComplexStar(x, n, N, P);


            //Отсюда закинуть все в отдельный метод
            double[] F = new double[N];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    F[j] = Convert.ToDouble(mainF.Invoke(null, new object[] { x[0, j], x[1, j] }));
                }
            }
            double[,] new_coord = MainOptimMethodBox(x, mainF, n, N, F, sMin);




            pts = new List<PointF>();
            if (sMin)
            {
                pts.Add(new PointF((float)new_coord[0,0], (float)new_coord[0, 0]));
                //pts.Add(new PointF((float)param1max, (float)param2max));
            }
            else
            {
                pts.Add(new PointF((float)new_coord[0, 0], (float)new_coord[0, 0]));
                //pts.Add(new PointF((float)param1min, (float)param2min));
            }

            return pts;
        }
        //Общий метод
        protected static double[,] MainOptimMethodBox(double[,] x, MethodInfo mainF, int n, int N, double[] F, bool sMin)
        {
            bool end_search = false;
            double[,] x_0 = new double[n, N];
            while (!end_search)
            {
                int[] goodBadInx = getIndexGoodBad(F, sMin);
                int Ginxgood = goodBadInx[0];
                int Dinxbad = goodBadInx[1];
                //На 4) Определение координат Сi центра Комплекса 
                double[] C = new double[n];
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < N; j++)
                    {
                        sum += x[i, j] - x[i, Dinxbad];
                    }
                    C[i] = (1 / N - 1) * sum;
                }
                //Проверка условия окончания поиска
                double B = 0;
                for (int i = 0; i < n; i++)
                {
                    B += Math.Abs(C[i] - x[i, Dinxbad]) + Math.Abs(C[i] - x[i, Ginxgood]);
                }
                B = B / (2.0 * Convert.ToDouble(n));

                if (B < FuncAccuracy)
                {
                    end_search = true;
                }
                Console.WriteLine(B);
                //Вычисление координаты новой точки Комплекса взамен наихудшей
                double[] g = new double[] { Param1Min, Param2Min };
                double[] h = new double[] { Param1Max, Param2Max };
                for (int i = 0; i < n; i++)
                {
                    x[i, 0] = 2.3 * C[i] - 1.3 * x[i, Dinxbad];
                    if (g[i] >= x[i, 0])
                    {
                        x[i, 0] = g[i] + FuncAccuracy;
                    }
                    if (x[i, 0] >= h[i])
                    {
                        x[i, 0] = h[i] - FuncAccuracy;
                    }
                }
                double[,] x_star = new double[n, N];
                for (int i = 0; i < n; i++)
                {
                    x[i, 0] = (x[i, 0] + C[i]) / 2.0;
                }


                //Вычисление значения целевой функции F0 в новой точке
                //Нахождение новой вершины смещением xi0   на половину расстояния к лучшей из вершин комплекса  с номером G                
                //x_0 = new double[n, N];
                //x = vertexOffset(x, mainF, n, N, Ginxgood, Dinxbad, F);
                double F0 = calcF0(mainF, x, n, N);
                // Чекни тут
                while (F0 > F[Dinxbad])
                {                    
                    for (int i = 0; i < n; i++)
                    {
                        x[i, 0] = (x[i, 0] + x[i, Ginxgood])/2.0;
                    }
                    F0 = calcF0(mainF, x, n, N);
                }

                //double F0 = calcF0(mainF, x_star, n, N);
                    
                F[Dinxbad] = F0;
            }

            return x_0;
        }
        //Нахождение новой вершины смещением xi0   на половину расстояния к лучшей из вершин комплекса  с номером G
        //protected static double[,] vertexOffset(double[,] x, MethodInfo mainF, int n, int N, int Ginxgood, int Dinxbad, double[] F)
        //{
        //    double[,] x_star = new double[n, N];
            
        //    if (F0 > F[Dinxbad])
        //    {
        //        vertexOffset(x_star, mainF, n, N, Ginxgood, Dinxbad, F);
        //    }
        //    return x_star;
        //}
       
        private static double calcF0(MethodInfo mainF,double[,] x, int n, int N)
        {
            double F = new double();
            if (n_recurse < 1000)
            {
                for (int i = 0; i < n; i++)
                {
                    F = Convert.ToDouble(mainF.Invoke(null, new object[] { x[i, 0], x[i, 0] }));
                }
            }
            n_recurse++;
            return F;
        }

        private static int[] getIndexGoodBad(double[] F, bool sMin)
        {
            int[] goodBad = new int[2];
            int minValueInx = 0;
            int maxValueInx = 0;
            double min = double.MaxValue;
            double max = double.MinValue;

            for (int i = 0; i < F.Length; i++)
            {
                if(F[i] < min)
                {
                    minValueInx = i;
                    min = F[i];
                }
                if(F[i]> max)
                {
                    maxValueInx = i;
                    max = F[i];
                }
            }

            if (sMin)
            {
                goodBad[0] = minValueInx;
                goodBad[1] = maxValueInx;
            }
            else
            {
                goodBad[0] = maxValueInx;
                goodBad[1] = minValueInx;
            }

            return goodBad;
        }

        private static double[,] CalcComplexStar(double[,] x, int n, int N, int P)
        {
            //double[,] x_star = new double[n, N];
            P = Math.Abs(N - P);
            double newP = Math.Abs(P);
            for (int i = 0; i < n; i++)
            {
                for (int j = P + 1; j < N; j++)
                {
                    double sum_x = 0;
                    for (int k = 0; k < P; k++)
                    {
                        sum_x += x[i, k];
                    }
                    x[i, j] = 0.5 * (x[i,j]) + sum_x/ newP;
                }
            }

            return x;
        }

        private static int CheckComplex(double[,] x, int n,  int N)
        {
            int P = 0;
            double[] g = new double[] { Param1Min, Param2Min };
            double[] h = new double[] { Param1Max, Param2Max };
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (x[i, j] > g[i] && x[i, j] < h[i])
                    {
                        P++;
                    }
                }
            }

            return P;
        }

        //Формирование исходного Комплекса
        private static double[,] CalcComplex(int n, int N)
        {
         
            //Комплекс
            double[,] x = new double[n, N];

            //нижнее и вверхнее допуустимое значение переменной
            double[] g = new double[] { Param1Min, Param2Min };
            double[] h = new double[] { Param1Max, Param2Max };
            //псевдослучайных чисел 
            double[,] r = new double[n, N];
            Random rnd = new Random();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    r[i, j] = rnd.NextDouble();
                    x[i, j] = g[i] + r[i, j] * (h[i] - g[i]);
                }
            }

            if (CheckComplex(x, n, N) >= 1)
            {
                return x;
            }
            else
            {
                CalcComplex(n, N);
            }

            return x;
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
            {
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
