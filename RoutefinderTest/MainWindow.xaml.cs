using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using MeesterTurner.Routefinder;

namespace PathfinderTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RouteFinder rf = new RouteFinder();

        public MainWindow()
        {
            InitializeComponent();
            AddRoads(1);
        }

        private void AddRoads(int mapID)
        {
            rf.allRoads.Clear();

            switch(mapID)
            {
                case 1:
                    rf.AddRoad(0, 0, 0, 5);
                    rf.AddRoad(0, 0, 3, 0);
                    rf.AddRoad(3, 0, 4, 3);
                    rf.AddRoad(4, 3, 2, 1);
                    rf.AddRoad(2, 1, 0, 5);
                    rf.AddRoad(2, 1, 3, 0);
                    rf.AddRoad(3, 0, 5, 1);
                    rf.AddRoad(0, 5, 3, 5);
                    rf.AddRoad(0, 0, 1, 1);
                    rf.AddRoad(1, 1, 2, 1);
                    rf.AddRoad(1, 1, 1, 2);
                    break;

                case 2:
                    Random r = new Random();

                    for (int x = 0; x <= 9; x++)
                    {
                        int ry = r.Next(5, 10);
                        for (int y = 0; y <= ry; y++)
                        {
                            rf.AddRoad(x, y, x + 1, y);
                            rf.AddRoad(x + 1, y, x + 1, y + 1);
                        }
                    }

                    for (int i = 1; i <= 45; i++)
                    {
                        int rn = r.Next(20, rf.allRoads.Count - 20);
                        rf.allRoads.RemoveAt(rn);
                    }
                    break;
            }

            DrawRoads();
            DrawPoints();
        }

        private void DrawPoints()
        {
            PointsCanvas.Children.Clear();

            foreach (Road r in rf.allRoads)
            {
                for(int i = 1; i <= 2; i++)
                {
                    int px;
                    int py;

                    if(i == 1)
                    {
                        px = r.From.x;
                        py = r.From.y;
                    }
                    else
                    {
                        px = r.To.x;
                        py = r.To.y;
                    }

                    Ellipse e = new Ellipse();
                    e.Width = 10;
                    e.Height = 10;
                    e.Fill = Brushes.AliceBlue;
                    e.Stroke = Brushes.Navy;
                    e.Cursor = Cursors.Hand;

                    e.MouseLeftButtonUp += Point_LeftMouseUp;
                    e.MouseRightButtonUp += Point_RightMouseUp;

                    e.Tag = string.Format("{0},{1}", px, py);
                    e.ToolTip = e.Tag;

                    Canvas.SetLeft(e, ConvertPoint(px) - 5);
                    Canvas.SetTop(e, ConvertPoint(py) - 5);

                    PointsCanvas.Children.Add(e);
                }
            }
        }

        private void Point_RightMouseUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse ell = (sender as Ellipse);

            txtToX.Text = ell.Tag.ToString().Split(Convert.ToChar(","))[0];
            txtToY.Text = ell.Tag.ToString().Split(Convert.ToChar(","))[1];
        }

        private void Point_LeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            Ellipse ell = (sender as Ellipse);

            txtFromX.Text = ell.Tag.ToString().Split(Convert.ToChar(","))[0];
            txtFromY.Text = ell.Tag.ToString().Split(Convert.ToChar(","))[1];
        }

        private void DrawRoads()
        {
            MainCanvas.Children.Clear();
            foreach (Road r in rf.allRoads)
                DrawPath(r, Brushes.Black, 1);
        }

        private void DrawPath(Road r, Brush colour, int lineThickness)
        {
            Line rl = new Line();
            rl.X1 = ConvertPoint(r.From.x);
            rl.Y1 = ConvertPoint(r.From.y);
            rl.X2 = ConvertPoint(r.To.x);
            rl.Y2 = ConvertPoint(r.To.y);
            rl.StrokeThickness = lineThickness;
            rl.Stroke = colour;

            MainCanvas.Children.Add(rl);
        }

        private int ConvertPoint(int gridpos)
        {
            return gridpos * 40 + 20;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DrawRoads();

            rf.StartX = Convert.ToInt32(txtFromX.Text);
            rf.StartY = Convert.ToInt32(txtFromY.Text);
            rf.DestinationX = Convert.ToInt32(txtToX.Text);
            rf.DestinationY = Convert.ToInt32(txtToY.Text);

            rf.Method = FinderMethod.AStar;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                rf.Go();
            }
            catch(Exception ex)
            {
                sw.Stop();
                MessageBox.Show("Exception: " + ex.Message);
                return;

            }
            sw.Stop();


            TimeSpan ts = sw.Elapsed;
            TimeLabel.Text = string.Format("Runtime: {0}ms", ts.TotalMilliseconds.ToString("0.000"));

            if(rf.journeys != null)
            {
                if (rf.journeys.Count == 0)
                {
                    MessageBox.Show("Unable to complete route");
                }
                else
                {
                    Journey j = rf.Shortest();
                    foreach (Road r in j.Roads)
                        DrawPath(r, Brushes.Red, 2);
                }
            }
        }

        private void MapButton1_Checked(object sender, RoutedEventArgs e)
        {
            AddRoads(1);
        }

        private void MapButton2_Checked(object sender, RoutedEventArgs e)
        {
            AddRoads(2);
        }
    }
}
