using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;

namespace kursOptimiz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            w = new Window1(IntiDataForTask);
            w2 = new Window2();
        }

        Calculation calc;
        Bitmap bmp;
        Graphics graph;
        List<System.Drawing.Color> colors;
        public static Random rand = new Random();
        private string task = "";
        private string[] outputMsg;
        Window1 w;
        Window2 w2;


        public MethodInfo MainFunc;
        public MethodInfo Condit;
        public MethodInfo Calculate;
        public MethodInfo SetMinMax;
        public MethodInfo SetAccuracy;
        public MethodInfo GetCalculations;
        public bool searchingForMin;
        public bool IntiDataForTask()
        {
            Type TestType = Type.GetType("kursOptimiz.Tasks", false, true);
            Tasks tasks = new Tasks();
            string inxTask = tasks.GetTaskList()[w.KeyTask];
            double[] granP = tasks.GetGranPoint()[inxTask];
            string taskFormula = tasks.GetFormalTaskList()[inxTask];
            searchingForMin = tasks.GetSearchingForMin()[inxTask];
            textBoxParam1Min.Text = granP[0].ToString();
            textBoxParam1Max.Text = granP[1].ToString();
            textBoxParam2Min.Text = granP[2].ToString();
            textBoxParam2Max.Text = granP[3].ToString();
            selectTasklabel.Content = $"Выбрано: {w.KeyTask}";
            task = taskFormula;
            outputMsg = tasks.GetOutMessage()[inxTask].Split(':');
            Calculation.SetMinMax(float.Parse(textBoxParam1Min.Text), float.Parse(textBoxParam1Max.Text), float.Parse(textBoxParam2Min.Text), float.Parse(textBoxParam2Max.Text));

            MainFunc = TestType.GetMethod("MainFunction" + inxTask);
            Condit = TestType.GetMethod("Contidions" + inxTask);
            Calculate = TestType.GetMethod("Calculate");
            SetMinMax = TestType.GetMethod("SetMinMax");
            SetAccuracy = TestType.GetMethod("SetAccuracy");
            GetCalculations = TestType.GetMethod("GetCalculations");
            return true;
        }

        private void resolve_button_Click(object sender, RoutedEventArgs e)
        {
            Calculation calc = new Calculation();
            try
            {
                Calculation.SetMinMax(float.Parse(textBoxParam1Min.Text), float.Parse(textBoxParam1Max.Text), float.Parse(textBoxParam2Min.Text), float.Parse(textBoxParam2Max.Text));
            }
            catch
            {
                MessageBox.Show("Ограничения должны быть вещественными числами. Иные символы запрещены. Повторите ввод", "Неверный числовой формат", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (float.Parse(textBoxParam1Min.Text) >= float.Parse(textBoxParam1Max.Text))
            {
                MessageBox.Show("Минимальное значение первого параметра должно быть меньше максимального", "Неверный интервал", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (float.Parse(textBoxParam2Min.Text)>= float.Parse(textBoxParam2Max.Text))
            {
                MessageBox.Show("Минимальное значение второго параметра  должно быть меньше максимального", "Неверный интервал", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Calculation.SetMinMax(float.Parse(textBoxParam1Min.Text), float.Parse(textBoxParam1Max.Text), float.Parse(textBoxParam2Min.Text), float.Parse(textBoxParam2Max.Text));
            double DeltaF = double.Parse(textBoxDeltaF.Text.ToString());
            List<PointF> pts = new List<PointF>();
            SetMinMax.Invoke(null, new object[] { float.Parse(textBoxParam1Min.Text), float.Parse(textBoxParam1Max.Text), float.Parse(textBoxParam2Min.Text), float.Parse(textBoxParam2Max.Text) });
            SetAccuracy.Invoke(null, new object[] { double.Parse(textBoxDeltaX.Text.ToString()), double.Parse(textBoxDeltaF.Text.ToString()) });
            Stopwatch stop = new Stopwatch();
            stop.Start();
            pts = Calculate.Invoke(null, new object[] { pts, MainFunc, Condit, searchingForMin }) as List<PointF>;
            stop.Stop();
            string calcTime = stop.ElapsedMilliseconds.ToString();

            object tmp = MainFunc.Invoke(null, new object[] { (double)pts[0].X, (double)pts[0].Y });
            object tmp1 = MainFunc.Invoke(null, new object[] { (double)pts[1].X, (double)pts[1].Y });
            float[] values;
            List<float> vals = new List<float>();
            float step = -1;

            stop.Reset();
            stop.Start();
            float min = -1;
            float max = -1;
            if (searchingForMin)
            {
                min = (float)(double)tmp;
                max = (float)(double)tmp1;
                if ((max - min) / 1000 > 1)
                {
                    step = (max - min) / 5000;
                }
                else if (((max - min) / 100) > 1)
                {
                    step = (max - min) / 500;
                }
                else
                {
                    step = (max - min) / 50;
                }
                float st = step;
                for (float i = min; i < max; i += step)
                {
                    vals.Add(i);
                    step += st;
                    
                }
            }
            else
            {
                max = (float)(double)tmp;
                min = (float)(double)tmp1;
                if ((max - min) / 1000 > 1)
                {
                    step = (max - min) / 2000;
                }
                else if ((max - min) / 100 > 1)
                {
                    step = (max - min) / 200;
                }
                else
                {
                    step = (max - min) / 20;
                }
                float st = step;
                for (float i = min; i < max; i += step)
                {
                    vals.Add(i);
                    step += st*2;
                }
            }
            if (vals.Count == 0)
            {
                MessageBox.Show("Все значения функции при данных ограничениях на пеерменные, находятся вне области определения функции", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                values = vals.ToArray();
                DrawValues(values);


                DrawResultPoints(pts);
                resT1.Content = $"А1 = {pts[0].X}";
                resT2.Content = $"А2 = {pts[0].Y}";
                //res.Content = $"Результат = {Calculation.MainFunction((double)pts[0].X, (double)pts[0].Y) - (Calculation.MainFunction((double)pts[0].X, (double)pts[0].Y) % float.Parse(textBoxDeltaF.Text)) + float.Parse(textBoxDeltaF.Text)}";

                show3d_button1.IsEnabled = true;
                res.Content = $"Результат = {Convert.ToDouble(tmp) - (Convert.ToDouble(tmp) % Convert.ToDouble(textBoxDeltaF.Text)) + Convert.ToDouble(textBoxDeltaF.Text)}";
                labelCalculationsCount.Content = $"Количество вычислений функции = {GetCalculations.Invoke(null, null).ToString()}";
                stop.Stop();
                string visTime = stop.ElapsedMilliseconds.ToString();
                this.VisualisationTime.Content = $"Время визуализации, мс: {visTime}";
                this.CalculationsTime.Content = $"Время расчёта, мс: {calcTime}";

                string output = $"{this.outputMsg[0]} {pts[0].X}";
                output += $"{this.outputMsg[1]} {pts[0].Y}";

                double ResultingOptimalOutput = Convert.ToDouble(tmp) - (Convert.ToDouble(tmp) % float.Parse(textBoxDeltaF.Text)) + float.Parse(textBoxDeltaF.Text);
                output += $"{this.outputMsg[2]} {ResultingOptimalOutput} {outputMsg[3]}";

                MessageBox.Show(output);
            }
        }


        void FindFuncMinMax(double delta, out double fMin, out double fMax)
        {
            fMin = double.MaxValue;
            fMax = double.MinValue;
            for (double Param2Coord = Calculation.Param2Min; Param2Coord <= Calculation.Param2Max; Param2Coord += delta)
                for (double Param1Coord = Calculation.Param1Min; Param1Coord <= Calculation.Param1Max; Param1Coord += delta)
                {
                    var res = MainFunc.Invoke(null, new object[] { Param1Coord, Param2Coord });
                    double var = Convert.ToDouble(res);
                    if (var > fMax)
                        fMax = var;
                    if (var < fMin)
                        fMin = var;
                }
        }

        void DrawValues(float[] valuesToSearch)
        {
            int counter = 0;
            double XPointsCount = double.Parse(textBoxDeltaX.Text);
            //float ZPointsCount = float.Parse(textBoxDeltaF.Text);
            SetMinMax.Invoke(null, new object[] { float.Parse(textBoxParam1Min.Text), float.Parse(textBoxParam1Max.Text), float.Parse(textBoxParam2Min.Text), float.Parse(textBoxParam2Max.Text) });
            double fMin, fMax;
            double deltaX = (Calculation.Param1Max - Calculation.Param1Min) / XPointsCount;
            FindFuncMinMax(deltaX, out fMin, out fMax);
            graph.Clear(System.Drawing.Color.White);
            double delta = (fMax - fMin) / XPointsCount;
            float max = valuesToSearch.Max();
            float min = valuesToSearch.Min();
            System.Drawing.Color toDraw = colors[rand.Next(colors.Count)];
            foreach (var value in valuesToSearch)
            {
                double color = (max - min) / 7;
                if(value < color / 4)
                {
                    toDraw = colors[0];
                }
                else if (value > (color/4) && value <= (color/2))
                {
                    toDraw = colors[1];
                }
                else if (value>(color/2) && value <= (color * 3 / 4))
                {
                    toDraw = colors[2];
                }
                else if (value > color*3/4 && value <= color)
                {
                    toDraw = colors[3];
                }
                else if (value > color && value <= color*2)
                {
                    toDraw = colors[4];
                }
                else if (value > color*2 && value <= color*3)
                {
                    toDraw = colors[5];
                }
                else if (value > color * 3 && value <= color * 4)
                {
                    toDraw = colors[6];
                }
                else if (value > color * 4 && value <= color * 5)
                {
                    toDraw = colors[7];
                }
                else if (value > color * 5 && value <= color * 6)
                {
                    toDraw = colors[8];
                }
                else if (value > color * 6 && value <= color * 7)
                {
                    toDraw = colors[9];
                }
                    //System.Drawing.Color n = colors[rand.Next(colors.Count)];
               // while (n.Equals(toDraw))
                   // n = colors[rand.Next(colors.Count)];
                //toDraw = n;
                for (double Param2Coord = Calculation.Param2Min; Param2Coord <= Calculation.Param2Max; Param2Coord += deltaX)
                    for (double Param1Coord = Calculation.Param1Min; Param1Coord <= Calculation.Param1Max; Param1Coord += deltaX)
                    {
                        //if (!Calculation.Contidions(Param1Coord, Param2Coord))
                        //    continue;
                        if (!Convert.ToBoolean(Condit.Invoke(null, new object[] { Param1Coord, Param2Coord })))
                            continue;
                        var res = MainFunc.Invoke(null, new object[] { Param1Coord, Param2Coord });
                        double val = Convert.ToDouble(res);
                        if (val >= value - delta && val <= value + delta)
                        {
                            int yCoord = GetPixelFromCoord(Param2Coord, true, false);
                            int xCoord = GetPixelFromCoord(Param1Coord, false, false);
                            bmp.SetPixel(xCoord, yCoord, toDraw);
                            counter++;
                        }
                    }
            }
            if (true)
            {
                graph.DrawLine(Pens.Black, 350, 0, 350, 700);
                graph.DrawLine(Pens.Black, 0, 350, 700, 350);
                Font fn = new Font("TimesNewRoman", 12);
                graph.DrawString("A1", fn, System.Drawing.Brushes.Black, new System.Drawing.Point(675, 350));
                graph.DrawString("A2", fn, System.Drawing.Brushes.Black, new System.Drawing.Point(350, 0));
                graph.DrawString($"({Calculation.Param1Min + (Calculation.Param1Max - Calculation.Param1Min) / 2}; {Calculation.Param2Min + (Calculation.Param2Max - Calculation.Param2Min) / 2})", fn, System.Drawing.Brushes.Black, new System.Drawing.Point(350, 350));
                graph.DrawString($"{Calculation.Param1Max}", fn, System.Drawing.Brushes.Black, new System.Drawing.Point(675, 330));
                graph.DrawString($"{Calculation.Param1Min}", fn, System.Drawing.Brushes.Black, new System.Drawing.Point(0, 330));
                graph.DrawString($"{Calculation.Param2Max}", fn, System.Drawing.Brushes.Black, new System.Drawing.Point(320, 0));
                graph.DrawString($"{Calculation.Param2Min}", fn, System.Drawing.Brushes.Black, new System.Drawing.Point(320, 675));
            }
            (pictureBoxMain.Child as System.Windows.Forms.PictureBox).Image = bmp;
        }

        void DrawResultPoints(List<PointF> pts)
        {

            PointF point = pts[0];
            int a1Coord = GetPixelFromCoord(point.X, false, false);
            int a2Coord = GetPixelFromCoord(point.Y, true, false);
            graph.DrawLine(Pens.Black, a1Coord, a2Coord - 6, a1Coord, a2Coord + 6);
            graph.DrawLine(Pens.Black, a1Coord - 6, a2Coord, a1Coord + 6, a2Coord);

            graph.DrawEllipse(Pens.Black, a1Coord - 6, a2Coord - 6, 12, 12);
            (pictureBoxMain.Child as System.Windows.Forms.PictureBox).Image = bmp;
        }
        void InitGraphics()
        {
            bmp = new Bitmap((pictureBoxMain.Child as System.Windows.Forms.PictureBox).Width, (pictureBoxMain.Child as System.Windows.Forms.PictureBox).Height);
            graph = Graphics.FromImage(bmp);
            graph.Clear(System.Drawing.Color.White);
        }

        void InitColors()
        {
            colors = new List<System.Drawing.Color>();        

            colors.Add(System.Drawing.Color.Purple);
            colors.Add(System.Drawing.Color.MediumPurple);
            colors.Add(System.Drawing.Color.MidnightBlue);
            colors.Add(System.Drawing.Color.Blue);

            colors.Add(System.Drawing.Color.Aqua);
            colors.Add(System.Drawing.Color.Green);
            colors.Add(System.Drawing.Color.Yellow);
            colors.Add(System.Drawing.Color.Orange);

            colors.Add(System.Drawing.Color.DarkOrange);
            colors.Add(System.Drawing.Color.Red);
        }

        int GetPixelFromCoord(double coord, bool isY, bool ignoreRestrictions)
        {
            int pixel = -1;
            if (isY)
                pixel = (int)(coord / (Calculation.Param2Max - Calculation.Param2Min) * (int)pictureBoxMain.ActualHeight + (int)pictureBoxMain.ActualHeight * (-(Calculation.Param2Min) / (Calculation.Param2Max - Calculation.Param2Min)));
            else
                pixel = (int)(coord / (Calculation.Param1Max - Calculation.Param1Min) * (int)pictureBoxMain.ActualWidth + (int)pictureBoxMain.ActualWidth * (-(Calculation.Param1Min) / (Calculation.Param1Max - Calculation.Param1Min)));
            if (isY)
            {
                pixel = (int)pictureBoxMain.ActualHeight - pixel - 1;
                if (!ignoreRestrictions)
                {
                    if (pixel < 0)
                        pixel = 0;
                }
            }
            else
            {
                if (!ignoreRestrictions)
                {
                    if (pixel == (int)pictureBoxMain.ActualWidth)
                        pixel--;
                }
            }
            return pixel;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            InitGraphics();
            InitColors();
            IntiDataForTask();

            show3d_button1.IsEnabled = false;
        }

        private void show3d_button1_Click(object sender, RoutedEventArgs e)
        {
            //(new _3DGraph(Calculation.MainFunction, Calculation.Param1Min, Calculation.Param1Max, Calculation.Param2Min, Calculation.Param2Max)).ShowDialog();
            (new _3DGraph(MainFunc, Calculation.Param1Min, Calculation.Param1Max, Calculation.Param2Min, Calculation.Param2Max)).ShowDialog();
        }

        private void buttonExportImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Solution2D";
            dlg.DefaultExt = ".PNG";
            dlg.Filter = "Images (.PNG)|*.PNG";

            bool? result = dlg.ShowDialog();
            if (result.Value)
                bmp.Save(dlg.FileName);
        }

        private void createReport_button1_Click(object sender, RoutedEventArgs e)
        {
            new Report().Create(bmp);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"Данная программа была создана в рамках курсового проекта
            по дисциплине ''Методы оптимизации'';
            Выполнил - студент группы №465, Винокуров Никита Александрович;
            Руководитель - Смирнов Игорь Александрович;", "Внимание!", MessageBoxButton.OK);
        }

        private void TaskList_Click(object sender, RoutedEventArgs e)
        {
            w = new Window1(IntiDataForTask);
            w.Show();
        }

        private void MethodList_Click(object sender, RoutedEventArgs e)
        {
            w2 = new Window2();
            w2.Show();
        }

        private void ShowTask_Click(object sender, RoutedEventArgs e)
        {
            ShowFormalTask sft = new ShowFormalTask();
            sft.URI = null;
            sft.formalTasklabel.Content = $"Функция: ɑ *( G * µ* ((T2 - T1)^N + (β * A - T1)^N)\r\n" +
                $"      ɑ, β, µ = 1 \r\n" +
                $"      G = 2, A = 1, N  = 2\r\n" +
                $"      -3 < T1 < 3\r\n" +
                $"      2 < T2 < 6\r\n" +
                $"      T1+T2 <= 12";
            sft.Show();
        }

        private void closedW(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ShowTaskButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(task, "Уравнение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
