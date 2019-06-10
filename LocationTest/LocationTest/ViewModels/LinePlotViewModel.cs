using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;
using OxyPlot.Series;
using LocationTest.ViewModels.Base;
using Newtonsoft.Json;

namespace LocationTest.ViewModels
{
    public class LinePlotViewModel : ViewModelBase
    {
        private PlotModel _plotModel;
        private int markerType = 1;
        private readonly string JSONSTUFF = "[{x:494,y:293},{x:493,y:298.7},{x:497,y:299.4},{x:492,y:299.6}]";

        private readonly string JSONSTUFF2 = "[{x:494.5,y:293},{x:491,y:298.7},{x:499,y:299.4},{x:492,y:299.6}]";

        private readonly string JSONSTUFF3 = "[{x:494.9,y:293},{x:493,y:298.7},{x:497,y:299.4},{x:492,y:299.6}]";
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                RaisePropertyChanged();
            }
        }

        public LineSeries GenerateLineSeries(string JSON){
            List<DataPoint> json = JsonConvert.DeserializeObject<List<DataPoint>>(JSON);
            LineSeries series = new LineSeries
            {
                RenderInLegend = true,
                Title ="SOMESENSOR",
            Tag = "sensor",
            MarkerType = (MarkerType)markerType,
            LineStyle = LineStyle.Automatic,
                StrokeThickness = 2.0,
            };
            if (markerType==4)
                markerType = 1;
            else
                markerType++;

            json.ForEach(x => series.Points.Add(x));

            series.Points.Sort((x, y) => x.X.CompareTo(y.X));
            return series;
            }
        public override void OnAppearing(object navigationContext)
        {
            base.OnAppearing(navigationContext);

            PlotModel = new PlotModel
            {
                Title = "Data"
            };

            var lineserie1 = GenerateLineSeries(JSONSTUFF);
            var lineserie2 = GenerateLineSeries(JSONSTUFF2);
            var lineserie3 =GenerateLineSeries(JSONSTUFF3);
            

            PlotModel.Series.Add(lineserie1);

            PlotModel.Series.Add(lineserie2);

            PlotModel.Series.Add(lineserie3);
        }
    }
}
