﻿using System;
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

            PlotModel.Series.Add(lineSerie);
        }
    }
}
