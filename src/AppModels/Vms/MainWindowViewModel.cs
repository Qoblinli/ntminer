﻿using NTMiner.Core;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        private string _serverJsonVersion;
        private StateBarViewModel _stateBarVm = new StateBarViewModel();
        private MinerStateViewModel _minerStateVm;
        public MinerStateViewModel MinerStateVm {
            get {
                if (_minerStateVm == null) {
                    _minerStateVm = new MinerStateViewModel(_stateBarVm);
                }
                return _minerStateVm;
            }
        }

        public ICommand CustomTheme { get; private set; }
        public ICommand UseThisPcName { get; private set; }
        public ICommand CloseMainWindow { get; private set; }

        public MainWindowViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.CloseMainWindow = new DelegateCommand(() => {
                VirtualRoot.Execute(new CloseMainWindowCommand());
            });
            this.CustomTheme = new DelegateCommand(() => {
                VirtualRoot.Execute(new ShowLogColorCommand());
            });
            this.UseThisPcName = new DelegateCommand(() => {
                string thisPcName = NTMinerRoot.GetThisPcName();
                this.ShowDialog(message: $"确定使用本机名{thisPcName}作为矿机名吗？", title: "确认", onYes: () => {
                    MinerProfile.MinerName = thisPcName;
                }, icon: IconConst.IconConfirm);
            });
            if (DevMode.IsDevMode) {
                _serverJsonVersion = GetServerJsonVersion();
            }
        }

        public string GetServerJsonVersion() {
            string serverJsonVersion = string.Empty;
            if (NTMinerRoot.Instance.LocalAppSettingSet.TryGetAppSetting("ServerJsonVersion", out IAppSetting setting) && setting.Value != null) {
                serverJsonVersion = setting.Value.ToString();
            }
            return serverJsonVersion;
        }

        public string BrandTitle {
            get {
                if (NTMinerRoot.KernelBrandId == Guid.Empty && NTMinerRoot.PoolBrandId == Guid.Empty) {
                    return string.Empty;
                }
                if (NTMinerRoot.Instance.SysDicItemSet.TryGetDicItem(NTMinerRoot.KernelBrandId, out ISysDicItem dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                else if (NTMinerRoot.Instance.SysDicItemSet.TryGetDicItem(NTMinerRoot.PoolBrandId, out dicItem)) {
                    if (!string.IsNullOrEmpty(dicItem.Value)) {
                        return dicItem.Value + "专版";
                    }
                    return dicItem.Code + "专版";
                }
                return string.Empty;
            }
        }

        public StateBarViewModel StateBarVm {
            get => _stateBarVm;
        }

        public bool IsUseDevConsole {
            get { return NTMinerRoot.IsUseDevConsole; }
            set {
                NTMinerRoot.IsUseDevConsole = value;
                OnPropertyChanged(nameof(IsUseDevConsole));
            }
        }

        public AppContext.MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
            }
        }

        public string ServerJsonVersion {
            get => _serverJsonVersion;
            set {
                if (_serverJsonVersion != value) {
                    _serverJsonVersion = value;
                    OnPropertyChanged(nameof(ServerJsonVersion));
                }
            }
        }

        public Visibility IsOverClockVisible {
            get {
                if (Design.IsInDesignMode) {
                    return Visibility.Visible;
                }
                if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.NVIDIA) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
