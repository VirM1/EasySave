using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ProjectLibrary.Models;
using System.Text.Json.Serialization;
using ProjectLibrary.Models.Converters;

namespace EasySaveWPF.ViewModels.Proxys
{
    public class WorkProxy : Work, INotifyPropertyChanged
    {

        private double _progress;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public new long RemainingFileSize { 
            get => remainingFileSize;
            set
            {
                remainingFileSize = value;
                OnPropertyChanged(nameof(RemainingFileSize));
            }
        }

        public new int RemainingFileCount
        {
            get => remainingFileCount;
            set
            {
                remainingFileCount = value;
                this.Progress = getPercentage();
                OnPropertyChanged(nameof(RemainingFileCount));
            }
        }

        public new long InitialFileSize
        {
            get => initialFileSize;
            set
            {
                initialFileSize = value;
                OnPropertyChanged(nameof(InitialFileSize));
            }
        }

        public new int InitialFileCount
        {
            get => initialFileCount;
            set
            {
                initialFileCount = value;
                OnPropertyChanged(nameof(InitialFileCount));
            }
        }


        [JsonConverter(typeof(WorkStatusConverter))]
        public new WorkStatus Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        public new string CurrentSourcePath { 
            get => currentSourcePath;
            set
            {
                currentSourcePath = value;
                OnPropertyChanged(nameof(CurrentSourcePath));
            }
        }
        public new string CurrentDestinationPath { 
            get => currentDestinationPath;
            set
            {
                currentDestinationPath = value;
                OnPropertyChanged(nameof(CurrentDestinationPath));
            }
        }


        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public double getPercentage()
        {
            if (this.InitialFileCount != 0)
            {
                double perc = ((100 * this.RemainingFileCount) / this.InitialFileCount);
                return 100 - Math.Round(perc, 3);
            }
            return 100;
        }
    }
}
