﻿using NTMiner.Vms;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class KernelsWindow : BlankWindow {
        private static readonly object _locker = new object();
        private static KernelsWindow _instance = null;
        public static void ShowWindow() {
            UIThread.Execute(() => {
                if (_instance == null) {
                    lock (_locker) {
                        if (_instance == null) {
                            _instance = new KernelsWindow();
                            _instance.Show();
                        }
                    }
                }
                else {
                    _instance.ShowWindow(false);
                }
            });
        }

        public KernelsWindowViewModel Vm {
            get {
                return (KernelsWindowViewModel)this.DataContext;
            }
        }

        public KernelsWindow() {
            InitializeComponent();
            if (DevMode.IsDevMode) {
                this.Width += 600;
            }
            AppContext.Instance.KernelVms.PropertyChanged += Current_PropertyChanged;
        }

        protected override void OnClosed(EventArgs e) {
            AppContext.Instance.KernelVms.PropertyChanged -= Current_PropertyChanged;
            base.OnClosed(e);
            _instance = null;
        }

        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(AppContext.KernelViewModels.AllKernels)) {
                Vm.OnPropertyChanged(nameof(Vm.QueryResults));
            }
        }

        private void ButtonLeftCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset - CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void ButtonRightCoin_Click(object sender, RoutedEventArgs e) {
            double offset = CoinsScrollView.ContentHorizontalOffset + CoinsScrollView.ViewportWidth;
            CoinsScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < CoinsScrollView.ScrollableWidth;
        }

        private void CoinsScrollView_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window window = GetWindow(this);
                window.DragMove();
            }
        }

        private void TbKeyword_LostFocus(object sender, RoutedEventArgs e) {
            Vm.Search.Execute(null);
        }

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
    }
}
