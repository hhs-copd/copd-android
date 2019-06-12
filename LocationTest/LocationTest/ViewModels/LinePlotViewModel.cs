using LocationTest.Services;
using LocationTest.ViewModels.Base;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace LocationTest.ViewModels
{
    public class LinePlotViewModel : ViewModelBase
    {
        public string Token { get; set; }
        private int MarkerType = 1;

        private PlotModel _plotModel;

        public IGraphZoomModel GraphZoomModel { get; set; }

        public PlotModel PlotModel
        {
            get => this._plotModel;
            set
            {
                this._plotModel = value;
                this.RaisePropertyChanged();
            }
        }

        public LineSeries GenerateLineSeries(string name, GraphResponse graph)
        {
            IEnumerable<DataPoint> dataPoints = JsonConvert
                .DeserializeObject<List<Coordinate>>(graph.Body)
                .Select(item => DateTimeAxis.CreateDataPoint((DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(item.X)).DateTime), double.Parse(item.Y)))
                .OrderBy(a => a.X);

            LineSeries series = new LineSeries
            {
                RenderInLegend = true,
                Title = name,
                Tag = name,
                MarkerFill = OxyColor.FromRgb(50, 60, 255),
                MarkerSize = 4,
                MarkerType = OxyPlot.MarkerType.None,
                LineStyle = LineStyle.Automatic,
                MarkerStroke = OxyColor.FromRgb(20, 30, 255),
                StrokeThickness = 2.0,
            };

            this.MarkerType++;
            if (this.MarkerType == 5)
            {
                this.MarkerType = 1;
            }

            dataPoints.ToList().ForEach(series.Points.Add);

            return series;
        }

        public override async void OnAppearing(object navigationContext)
        {
            try
            {
                base.OnAppearing(navigationContext);

                this.PlotModel = new PlotModel
                {
                    Title = "Data"
                };

                DateTime startDate = DateTime.Now.AddMinutes(-30);
                DateTime endDate = DateTime.Now.AddMinutes(0);

                double minValue = DateTimeAxis.ToDouble(startDate);
                double maxValue = DateTimeAxis.ToDouble(endDate);

                if (this.PlotModel.Axes.Count == 0)
                {
                    this.PlotModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, Minimum = GraphZoomModel.Min, Maximum = GraphZoomModel.Max, });
                    this.PlotModel.Axes.Add(new DateTimeAxis() { Minimum = minValue, Maximum = maxValue, Position = AxisPosition.Bottom, IntervalType = DateTimeIntervalType.Seconds, StringFormat = "hh:mm:ss" });
                }

                ILambdaFunctionDataService dataService = DependencyService.Get<ILambdaFunctionDataService>();

                foreach (string sensor in GraphZoomModel.GraphItems)
                {
                    GraphResponse graph = await dataService.GetGraph(this.Token, sensor);
                    LineSeries series = this.GenerateLineSeries(sensor, graph);
                    this.PlotModel.Series.Add(series);
                }

                this.RaisePropertyChanged();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
