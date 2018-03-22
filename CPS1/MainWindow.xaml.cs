﻿namespace CPS1
{
    using System.Windows;
    using System.Windows.Controls;

    using CPS1.ViewModel;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
            this.InitializeComponent();
        }
    }
}