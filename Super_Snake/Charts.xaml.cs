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
using LiveCharts;
using LiveCharts.Wpf;

namespace Super_Snake
{
    /// <summary>
    /// Interaction logic for Charts.xaml
    /// </summary>
    public partial class Charts : Window
    {
        public Charts(double[] values)
        {
            InitializeComponent();
            var chartValues = new ChartValues<double>();
            chartValues.AddRange(values);

            FitnessChart.Series.Add(new ScatterSeries
            {
                Values = chartValues,
                StrokeThickness = 4,
                StrokeDashArray = new DoubleCollection(20),
                Stroke = new SolidColorBrush(Color.FromRgb(107, 185, 69)),
                Fill = Brushes.Transparent,
            });
        }


    }


}
