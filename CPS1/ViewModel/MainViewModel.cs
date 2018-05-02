﻿using System.Collections.Concurrent;
using CPS1.Model.CommandHandlers;
using CPS1.Model.SignalData;

namespace CPS1.ViewModel
{
    public class MainViewModel
    {
        private CommandHandler compositeSineCommand;

        public CommandHandler CompositeSineCommand =>
            compositeSineCommand ?? (compositeSineCommand = new CommandHandler(CompositeSine, () => true));

        private void CompositeSine(object obj)
        {
            FirstSignalViewModel.GenerateSignalCommand.Execute(null);
            SecondSignalViewModel.SignalData.Frequency.Value = 5.0d;
            SecondSignalViewModel.GenerateSignalCommand.Execute(null);
            CompositionViewModel.AddCommand.Execute((short)1);
        }

        public static FunctionAttribute<bool> SecondSignalContinuous;

        public MainViewModel()
        {
            FirstSignalViewModel = new SignalViewModel();
            SecondSignalViewModel = new SignalViewModel();

            SecondSignalContinuous = SecondSignalViewModel.SignalData.Continuous;

            CompositionViewModel = new CompositionViewModel(FirstSignalViewModel, SecondSignalViewModel);
            ConversionViewModel = new ConversionViewModel(FirstSignalViewModel);
            ConvolutionFiltrationCorrelationViewModel = new ConvolutionFiltrationCorrelationViewModel(FirstSignalViewModel);
        }

        public CompositionViewModel CompositionViewModel { get; }

        public ConversionViewModel ConversionViewModel { get; }

        public ConvolutionFiltrationCorrelationViewModel ConvolutionFiltrationCorrelationViewModel { get; }

        public SignalViewModel FirstSignalViewModel { get; }

        public SignalViewModel SecondSignalViewModel { get; }
    }
}