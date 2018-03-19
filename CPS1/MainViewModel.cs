﻿namespace CPS1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Windows.Input;

    using CPS1.Functions;

    public class MainViewModel
    {
        private readonly IFileDialog fileDialog;

        private Signal firstSignalType = Signal.Sine;

        private ICommand generateSignalCommand;

        private ICommand openCommand;

        private ICommand saveCommand;

        private Signal secondSignalType = Signal.Sine;

        private IFileSerializer serializer;

        public MainViewModel()
        {
            this.SignalFirst = new FunctionData();
            this.SignalSecond = new FunctionData();

            var signals = new List<Tuple<Signal, string, Required>>();
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.FullyRectifiedSine,
                    "Fully rectified sine signal",
                    FullyRectifiedSineWave.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.NormalDistribution,
                    "Gaussian distribution signal",
                    NormalDistributionWave.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.HalfRectifiedSine,
                    "Half rectified sine signal",
                    HalfRectifiedSineWave.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.ImpulseNoise,
                    "Impulse noise signal",
                    ImpulseNoise.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.KroneckerDelta,
                    "Kronecker delta signal",
                    KroneckerDelta.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.RandomNoise,
                    "Random noise signal",
                    RandomNoiseWave.RequiredAttributes));
            signals.Add(new Tuple<Signal, string, Required>(Signal.Sine, "Sine signal", SineWave.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(Signal.Square, "Square signal", SquareWave.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.SymmetricalSquare,
                    "Symmetrical square signal",
                    SymmetricalSquareWave.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.Triangle,
                    "Triangle signal",
                    TriangleWave.RequiredAttributes));
            signals.Add(
                new Tuple<Signal, string, Required>(
                    Signal.UnitStep,
                    "Unit step signal",
                    UnitStepWave.RequiredAttributes));

            this.AvailableSignals = ImmutableList.CreateRange(signals);

            this.SetRequiredParameters(this.SignalFirst);
            this.SetRequiredParameters(this.SignalSecond);

            this.fileDialog = new FileDialogWpf();
            this.serializer = new FileBinarySerializer();
        }

        public ImmutableList<Tuple<Signal, string, Required>> AvailableSignals { get; }

        public string FirstSignalType
        {
            get => this.AvailableSignals.Where(s => s.Item1.Equals(this.firstSignalType)).Select(s => s.Item2)
                .FirstOrDefault();
            set
            {
                this.firstSignalType = this.AvailableSignals.Where(s => s.Item2.Equals(value)).Select(s => s.Item1)
                    .FirstOrDefault();
                this.SetRequiredParameters(this.SignalFirst);
            }
        }

        public ICommand GenerateSignalCommand => this.generateSignalCommand
                                                 ?? (this.generateSignalCommand = new CommandHandler(
                                                         this.GenerateSignal,
                                                         () => true));

        public ICommand OpenCommand =>
            this.openCommand ?? (this.openCommand = new CommandHandler(this.OpenSignal, () => true));

        public int Param { get; } = 2;

        public ICommand SaveCommand =>
            this.saveCommand ?? (this.saveCommand = new CommandHandler(this.SaveSignal, () => true));

        public string SecondSignalType
        {
            get => this.AvailableSignals.Where(s => s.Item1.Equals(this.secondSignalType)).Select(s => s.Item2)
                .FirstOrDefault();
            set
            {
                this.secondSignalType = this.AvailableSignals.Where(s => s.Item2.Equals(value)).Select(s => s.Item1)
                    .FirstOrDefault();
                this.SetRequiredParameters(this.SignalSecond);
            }
        }

        public FunctionData SignalFirst { get; }

        public FunctionData SignalSecond { get; }

        public IEnumerable<string> SignalsLabels
        {
            get
            {
                return this.AvailableSignals.Select(p => p.Item2);
            }
        }

        private void GenerateSignal(object parameter)
        {
            if (parameter is short chart)
            {
                if (chart == 1)
                {
                    Generator.GenerateSignal(this.SignalFirst, this.firstSignalType);
                    Histogram.GetHistogram(this.SignalFirst);
                }
                else if (chart == 2)
                {
                    Generator.GenerateSignal(this.SignalSecond, this.secondSignalType);
                    Histogram.GetHistogram(this.SignalSecond);
                }
            }
        }

        private void OpenSignal(object parameter)
        {
            if (parameter is short chart)
            {
                var filename = this.fileDialog.GetOpenFilePath();
                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
                var points = (List<Point>)serializer.Deserialize(filename);

                if (chart == 1)
                {
                    this.SignalFirst.Points.Clear();
                    this.SignalFirst.Points.AddRange(points);
                    this.SignalFirst.PointsUpdate();
                    Histogram.GetHistogram(SignalFirst);
                    this.SignalFirst.HistogramPointsUpdate();
                }
                else if (chart == 2)
                {
                    this.SignalSecond.Points.Clear();
                    this.SignalSecond.Points.AddRange(points);
                    this.SignalSecond.PointsUpdate();
                    Histogram.GetHistogram(SignalSecond);
                    this.SignalSecond.HistogramPointsUpdate();
                }
            }
        }

        private void SaveSignal(object parameter)
        {
            if (parameter is short chart)
            {
                var filename = this.fileDialog.GetSaveFilePath();
                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
                if (chart == 1)
                {
                    this.serializer.Serialize(SignalFirst.Points, filename);
                }
                else if (chart == 2)
                {
                    this.serializer.Serialize(SignalSecond.Points, filename);
                }
            }
        }

        private void SetRequiredParameters(FunctionData signal)
        {
            var choice = this.firstSignalType;
            if (signal == this.SignalSecond)
            {
                choice = this.secondSignalType;
            }

            signal.RequiredAttributes = this.AvailableSignals.Where(s => s.Item1 == choice).Select(s => s.Item3)
                .FirstOrDefault();
        }
    }
}