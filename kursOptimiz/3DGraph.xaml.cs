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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Reflection;

namespace kursOptimiz
{
    public partial class _3DGraph : Window
    {
        const double normalizeFactor = 20;
        Vertex3D[,] vertices;
        //Func<double, double, double> funcMain;
        MethodInfo funcMain;
        int yPointsCount, xPointsCount;
        double xMin, xMax, yMin, yMax, zMin, zMax;
        DispatcherTimer dt;

        GeometryModel3D modelGeometry, xAxis, yAxis, zAxis;
        bool mDown;
        Point mLastPos;

        public _3DGraph()
        {
            InitializeComponent();
        }

        public _3DGraph(MethodInfo func, double xMin, double xMax, double yMin, double yMax)
            : this()
        {
            funcMain = func;
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
        }

        void SetGrid(int xPointsCount, int yPointsCount, double xMin, double xMax, double yMin, double yMax)
        {
            this.xPointsCount = xPointsCount;
            this.yPointsCount = yPointsCount;
            vertices = new Vertex3D[xPointsCount, xPointsCount];
            double dx = (xMax - xMin) / xPointsCount;
            double dy = (yMax - yMin) / yPointsCount;
            for (int yCounter = 0; yCounter < yPointsCount; yCounter++)
                for (int xCounter = 0; xCounter < xPointsCount; xCounter++)
                {
                    double xVal = xMin + dx * xCounter;
                    double yVal = yMin + dy * yCounter;
                    SetVertex(yCounter, xCounter, xVal, yVal, 0);
                }
        }

        void SetVertex(int yIndex, int xIndex, double xValue, double yValue, double zValue)
        {
            vertices[yIndex, xIndex] = new Vertex3D();
            vertices[yIndex, xIndex].x = xValue;
            vertices[yIndex, xIndex].y = yValue;
            vertices[yIndex, xIndex].z = zValue;
        }

        void SetValues()
        {
            for (int yCounter = 0; yCounter < yPointsCount; yCounter++)
                for (int xCounter = 0; xCounter < xPointsCount; xCounter++)
                {
                    Vertex3D vertex = vertices[yCounter, xCounter];
                    vertex.z = Convert.ToDouble(funcMain.Invoke(null, new object[] { vertex.x, vertex.y }));
                }
        }

        void NormailzeValues()
        {
            for (int yCounter = 0; yCounter < yPointsCount; yCounter++)
                for (int xCounter = 0; xCounter < xPointsCount; xCounter++)
                {
                    Vertex3D vertex = vertices[yCounter, xCounter];
                    vertex.x = (vertex.x - xMin) / (xMax - xMin) * normalizeFactor - normalizeFactor / 2;
                    vertex.y = (vertex.y - yMin) / (yMax - yMin) * normalizeFactor - normalizeFactor / 2;
                    vertex.z = (vertex.z - zMin) / (zMax - zMin) * normalizeFactor;
                }
            zMin = 0;
            zMax = normalizeFactor;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAll();
        }

        #region GraphControl

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (camera.Position.Z < 30)
            {
                camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 30);
                return;
            }
            if (camera.Position.Z > 150)
            {
                camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 150);
                return;
            }
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - e.Delta / 250D);
        }


        public PngBitmapEncoder GetPng()
        {
            RenderTargetBitmap renderer = new RenderTargetBitmap((int)Viewport.ActualWidth, (int)Viewport.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            Rectangle vRect = new Rectangle();
            vRect.Width = renderer.Width;
            vRect.Height = renderer.Height;
            vRect.Fill = Brushes.White;
            vRect.Arrange(new Rect(0, 0, vRect.Width, vRect.Height));
            renderer.Render(Viewport);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(renderer));
            return png;
        }

        private void buttonExportImage_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap renderer = new RenderTargetBitmap((int)Viewport.ActualWidth, (int)Viewport.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            Rectangle vRect = new Rectangle();
            vRect.Width = renderer.Width;
            vRect.Height = renderer.Height;
            vRect.Fill = Brushes.White;
            vRect.Arrange(new Rect(0, 0, vRect.Width, vRect.Height));
            renderer.Render(Viewport);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(renderer));

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Solution";
            dlg.DefaultExt = ".PNG";
            dlg.Filter = "Images (.PNG)|*.PNG";

            bool? result = dlg.ShowDialog();
            if (result.Value)
            {
                using (Stream stm = File.Create(dlg.FileName))
                    png.Save(stm);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dt.Stop();
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            mDown = true;
            Point pos = Mouse.GetPosition(Viewport);
            mLastPos = new Point(pos.X - Viewport.ActualWidth / 2, Viewport.ActualHeight / 2 - pos.Y);
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mDown = false;
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mDown) return;
            Point pos = Mouse.GetPosition(Viewport);
            Point actualPos = new Point(
                    pos.X - Viewport.ActualWidth / 2,
                    Viewport.ActualHeight / 2 - pos.Y);
            double dx = actualPos.X - mLastPos.X;
            double dy = actualPos.Y - mLastPos.Y;
            double mouseAngle = 0;

            if (dx != 0 && dy != 0)
            {
                mouseAngle = Math.Asin(Math.Abs(dy) /
                    Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                if (dx < 0 && dy > 0) mouseAngle += Math.PI / 2;
                else if (dx < 0 && dy < 0) mouseAngle += Math.PI;
                else if (dx > 0 && dy < 0) mouseAngle += Math.PI * 1.5;
            }
            else if (dx == 0 && dy != 0)
                mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
            else if (dx != 0 && dy == 0)
                mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

            double axisAngle = mouseAngle + Math.PI / 2;

            Vector3D axis = new Vector3D(
                    Math.Cos(axisAngle) * 4,
                    Math.Sin(axisAngle) * 4, 0);

            double rotation = 0.002 *
                    Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

            Transform3DGroup group = modelGeometry.Transform as Transform3DGroup;
            QuaternionRotation3D r =
                 new QuaternionRotation3D(
                 new Quaternion(axis, rotation * 180 / Math.PI));
            group.Children.Add(new RotateTransform3D(r));

            group = xAxis.Transform as Transform3DGroup;
            group.Children.Add(new RotateTransform3D(r));

            group = yAxis.Transform as Transform3DGroup;
            group.Children.Add(new RotateTransform3D(r));

            group = zAxis.Transform as Transform3DGroup;
            group.Children.Add(new RotateTransform3D(r));

            mLastPos = actualPos;

        }
        #endregion

        void InitAnimation()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(10);
            double rotation = 0.01;
            Vector3D axis = new Vector3D(0.05, 0.9, 0);
            dt.Tick += (s, e) =>
            {
                Transform3DGroup group = modelGeometry.Transform as Transform3DGroup;
                QuaternionRotation3D r =
                     new QuaternionRotation3D(
                     new Quaternion(axis, rotation * 180 / Math.PI));
                group.Children.Add(new RotateTransform3D(r));

                group = xAxis.Transform as Transform3DGroup;
                group.Children.Add(new RotateTransform3D(r));

                group = yAxis.Transform as Transform3DGroup;
                group.Children.Add(new RotateTransform3D(r));

                group = zAxis.Transform as Transform3DGroup;
                group.Children.Add(new RotateTransform3D(r));

            };
            dt.Start();
        }

        void FindAllMinMax()
        {
            zMin = double.MaxValue;
            zMax = double.MinValue;
            for (int yCounter = 0; yCounter < yPointsCount; yCounter++)
                for (int xCounter = 0; xCounter < xPointsCount; xCounter++)
                {
                    Vertex3D vertex = vertices[yCounter, xCounter];
                    if (vertex.z < zMin)
                        zMin = vertex.z;
                    if (vertex.z > zMax)
                        zMax = vertex.z;
                }
        }

        void ShowAll()
        {
            SetGrid(100, 100, xMin, xMax, yMin, yMax);
            SetValues();
            FindAllMinMax();
            DrawAllValuesOnLabels();
            NormailzeValues();
            CreateMeshGeometry();
            Transform3DGroup group = modelGeometry.Transform as Transform3DGroup;
            QuaternionRotation3D r =
                 new QuaternionRotation3D(
                 new Quaternion(new Vector3D(1, 0, 0), -200 / Math.PI));
            group.Children.Add(new RotateTransform3D(r));

            CreateAxes();

            group = xAxis.Transform as Transform3DGroup;
            group.Children.Add(new RotateTransform3D(r));
            group = yAxis.Transform as Transform3DGroup;
            group.Children.Add(new RotateTransform3D(r));
            group = zAxis.Transform as Transform3DGroup;
            group.Children.Add(new RotateTransform3D(r));

            InitAnimation();

        }

        void DrawAllValuesOnLabels()
        {
            labelXMin.Content = xMin;
            labelXMax.Content = xMax;
            labelYMin.Content = yMin;
            labelYMax.Content = yMax;
            labelZMin.Content = zMin;
            labelZMax.Content = zMax;
        }

        void CreateAxes()
        {
            //X-Axis
            Point3D start = new Point3D(-10, -10, 0);
            Point3D end = new Point3D(12, -10, 0);

            double width = 0.1f;
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(start.X, start.Y - width, start.Z - width)); //TopLeft
            mesh.Positions.Add(new Point3D(start.X, start.Y - width, start.Z + width)); //TopRight
            mesh.Positions.Add(new Point3D(start.X, start.Y + width, start.Z + width)); //BottomLeft
            mesh.Positions.Add(new Point3D(start.X, start.Y + width, start.Z - width)); //BottomRight

            mesh.Positions.Add(new Point3D(end.X, end.Y - width, end.Z - width));
            mesh.Positions.Add(new Point3D(end.X, end.Y - width, end.Z + width));
            mesh.Positions.Add(new Point3D(end.X, end.Y + width, end.Z + width));
            mesh.Positions.Add(new Point3D(end.X, end.Y + width, end.Z - width));

            //mesh.TriangleIndices.AddRange();
            int[] a1 = new int[] { 0, 1, 2, 2, 3, 0,
                0, 4, 7,
                7, 3, 0,
                0, 4, 5,
                5, 1, 0,
                1, 2, 6,
                6, 5, 1,
                3, 7, 6,
                6, 2, 3,
                4, 5, 6, 6, 7, 4
            };
            for (int i = 0; i < a1.Length; i++)
            {
                mesh.TriangleIndices.Add(a1[i]);
            }

            xAxis = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Black));
            xAxis.BackMaterial = new DiffuseMaterial(Brushes.Black);
            xAxis.Transform = new Transform3DGroup();
            group.Children.Add(xAxis);

            //Y-Axis
            start = new Point3D(-10, -10, 0);
            end = new Point3D(-10, 12, 0);

            mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(start.X - width, start.Y, start.Z - width)); //TopLeft
            mesh.Positions.Add(new Point3D(start.X - width, start.Y, start.Z + width)); //TopRight
            mesh.Positions.Add(new Point3D(start.X + width, start.Y, start.Z + width)); //BottomLeft
            mesh.Positions.Add(new Point3D(start.X + width, start.Y, start.Z - width)); //BottomRight

            mesh.Positions.Add(new Point3D(end.X - width, end.Y, end.Z - width));
            mesh.Positions.Add(new Point3D(end.X - width, end.Y, end.Z + width));
            mesh.Positions.Add(new Point3D(end.X + width, end.Y, end.Z + width));
            mesh.Positions.Add(new Point3D(end.X + width, end.Y, end.Z - width));

            a1 = new int[] { 0, 1, 2, 2, 3, 0,
                0, 4, 7,
                7, 3, 0,
                0, 4, 5,
                5, 1, 0,
                1, 2, 6,
                6, 5, 1,
                3, 7, 6,
                6, 2, 3,
                4, 5, 6, 6, 7, 4
            };
            for (int i = 0; i < a1.Length; i++)
            {
                mesh.TriangleIndices.Add(a1[i]);
            }

            yAxis = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Black));
            yAxis.BackMaterial = new DiffuseMaterial(Brushes.Black);
            yAxis.Transform = new Transform3DGroup();
            group.Children.Add(yAxis);

            //Z-Axis
            start = new Point3D(-10, -10, 0);
            end = new Point3D(-10, -10, 25);

            mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(start.X - width, start.Y - width, start.Z)); //TopLeft
            mesh.Positions.Add(new Point3D(start.X - width, start.Y + width, start.Z)); //TopRight
            mesh.Positions.Add(new Point3D(start.X + width, start.Y + width, start.Z)); //BottomLeft
            mesh.Positions.Add(new Point3D(start.X + width, start.Y - width, start.Z)); //BottomRight

            mesh.Positions.Add(new Point3D(end.X - width, end.Y - width, end.Z));
            mesh.Positions.Add(new Point3D(end.X - width, end.Y + width, end.Z));
            mesh.Positions.Add(new Point3D(end.X + width, end.Y + width, end.Z));
            mesh.Positions.Add(new Point3D(end.X + width, end.Y - width, end.Z));


            a1 = new int[] { 0, 1, 2, 2, 3, 0,
                0, 4, 7,
                7, 3, 0,
                0, 4, 5,
                5, 1, 0,
                1, 2, 6,
                6, 5, 1,
                3, 7, 6,
                6, 2, 3,
                4, 5, 6, 6, 7, 4
            };
            for (int i = 0; i < a1.Length; i++)
            {
                mesh.TriangleIndices.Add(a1[i]);
            }

            zAxis = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.Black));
            zAxis.BackMaterial = new DiffuseMaterial(Brushes.Black);
            zAxis.Transform = new Transform3DGroup();
            group.Children.Add(zAxis);
        }

        void CreateMeshGeometry()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            for (int i = 0; i < yPointsCount; i++)
                for (int j = 0; j < xPointsCount; j++)
                {
                    Vertex3D vertex = vertices[i, j];
                    mesh.Positions.Add(new Point3D(vertex.x, vertex.y, vertex.z));
                    mesh.TextureCoordinates.Add(new Point(0.5, (vertex.z - zMin) / zMax));
                }

            for (int i = 0; i < yPointsCount - 1; i++)
                for (int j = 0; j < xPointsCount - 1; j++)
                {
                    int n00 = i * xPointsCount + j;             //Top Left
                    int n10 = i * xPointsCount + j + 1;         //Top Right
                    int n01 = (i + 1) * xPointsCount + j;       // Bottom Left      
                    int n11 = (i + 1) * xPointsCount + j + 1;   //Bottom Right

                    mesh.TriangleIndices.Add(n01);
                    mesh.TriangleIndices.Add(n00);
                    mesh.TriangleIndices.Add(n10);

                    mesh.TriangleIndices.Add(n10);
                    mesh.TriangleIndices.Add(n11);
                    mesh.TriangleIndices.Add(n01);
                }

            //Geometry creation
            LinearGradientBrush bru = new LinearGradientBrush();
            bru.StartPoint = new Point(0.5, 0);
            bru.EndPoint = new Point(0.5, 1);
            bru.GradientStops.Add(new GradientStop(Colors.Blue, 0));
            bru.GradientStops.Add(new GradientStop(Colors.Aquamarine, 0.2));
            bru.GradientStops.Add(new GradientStop(Colors.Lime, 0.5));
            bru.GradientStops.Add(new GradientStop(Colors.Yellow, 0.8));
            bru.GradientStops.Add(new GradientStop(Colors.Red, 1));
            DiffuseMaterial mat = new DiffuseMaterial(bru);
            modelGeometry = new GeometryModel3D(mesh, mat);

            modelGeometry.Transform = new Transform3DGroup();
            modelGeometry.BackMaterial = new DiffuseMaterial(Brushes.Gray);
            group.Children.Add(modelGeometry);
        }
    }
}
