using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;
using OxyPlot.Series;
using LocationTest.ViewModels.Base;

namespace LocationTest.ViewModels
{
    public class LinePlotViewModel : ViewModelBase
    {
        private PlotModel _plotModel;

        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set
            {
                _plotModel = value;
                RaisePropertyChanged();
            }
        }

        public override void OnAppearing(object navigationContext)
        {
            base.OnAppearing(navigationContext);

            PlotModel = new PlotModel
            {
                Title = "Line"
            };

            var lineSerie = new LineSeries
            {
                StrokeThickness = 2.0
            };

            lineSerie.Points.Add(new DataPoint(0, 0));
            lineSerie.Points.Add(new DataPoint(10, 20));
            lineSerie.Points.Add(new DataPoint(30, 1));
            lineSerie.Points.Add(new DataPoint(40, 12));

            lineSerie.Points.Add(new DataPoint(30, 27));
            var lineSerie2 = new LineSeries
            {
                StrokeThickness = 2.0
            };

            lineSerie2.Points.Add(new DataPoint(0, 0));
            lineSerie2.Points.Add(new DataPoint(10, 25));
            lineSerie2.Points.Add(new DataPoint(30, 45));
            lineSerie2.Points.Add(new DataPoint(40, 35));

            lineSerie2.Points.Add(new DataPoint(30, 27));
            lineSerie.Points.Sort((x,y) => x.X.CompareTo(y.X));
            PlotModel.Series.Add(lineSerie);
            PlotModel.Series.Add(lineSerie2);
        }
    }
}
