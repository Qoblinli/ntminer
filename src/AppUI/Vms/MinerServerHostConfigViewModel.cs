﻿using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerServerHostConfigViewModel : ViewModelBase {
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MinerServerHostConfigViewModel() {
            this.Save = new DelegateCommand(() => {
                try {
                    if (string.IsNullOrEmpty(this.MinerServerHost)) {
                        this.MinerServerHost = NTMinerRegistry.MinerServerHost;
                    }
                    Server.MinerServerHost = this.MinerServerHost;
                    CloseWindow?.Invoke();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            });
            _minerServerHost = Server.MinerServerHost;
        }

        private string _minerServerHost;
        public string MinerServerHost {
            get {
                return _minerServerHost;
            }
            set {
                if (_minerServerHost != value) {
                    _minerServerHost = value;
                    OnPropertyChanged(nameof(MinerServerHost));
                }
            }
        }

        public int MinerServerPort {
            get {
                return WebApiConst.MinerServerPort;
            }
        }
    }
}
