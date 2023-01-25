using LiteExplorer.Commands.Base;
using LiteExplorer.Extensions;
using LiteExplorer.Models;
using LiteExplorer.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace LiteExplorer.ViewModels
{
    internal class TabItemViewModel : ViewModel, IDisposable
    {
        #region Private fields

        private readonly BackgroundWorker worker;

        #endregion

        #region Properties

        #region CurrentName
        private string currentName;

        public string CurrentName
        {
            get => currentName;
            set => SetValue(ref currentName, value);
        }
        #endregion

        #region CurrentPath
        private string currentPath;

        public string CurrentPath
        {
            get => currentPath;
            set => SetValue(ref currentPath, value);
        }
        #endregion

        #region CurrentItem
        private FileSystemObject currentItem;

        public FileSystemObject CurrentItem
        {
            get => currentItem;
            set => SetValue(ref currentItem, value);
        }
        #endregion

        #region ProgressPercent
        private double progressPercent;

        public double ProgressPercent
        {
            get => progressPercent;
            set => SetValue(ref progressPercent, value);
        }
        #endregion

        public ObservableCollection<FileSystemObject> FileSystemObjects { get; set; } = new();

        #endregion

        #region Commands

        #region Run
        public ICommand RunCmd { get; }
        private bool CanRunCmdExecute(object p) => true;
        private void OnRunCmdExecuted(object p) => Process.Start(new ProcessStartInfo(p.ToString()) { WorkingDirectory = CurrentPath ?? Environment.SystemDirectory });
        #endregion

        #region Open
        public ICommand OpenCmd { get; }
        private bool CanOpenCmdExecute(object p) => true;
        private void OnOpenCmdExecuted(object p)
        {
            if (p is FileSystemObject fso)
            {
                if (File.Exists(fso.Path))
                {
                    Process.Start(new ProcessStartInfo(fso.Path) { UseShellExecute = true });
                    return;
                }

                CurrentName = fso.Name;
                CurrentPath = fso.Path;
            }

            if (p is string path && path.Length > 0)
            {
                CurrentName = Directory.GetParent(path)?.Name;
                CurrentPath = Directory.GetParent(path)?.FullName;
            }

            if (worker.IsBusy)
                worker.CancelAsync();
            else
                OpenPath();
        }

        #endregion

        #region Back
        public ICommand BackCmd { get; }
        private bool CanBackCmdExecute(object p) => p != null;
        private void OnBackCmdExecuted(object p) => OpenCmd.Execute(p);
        #endregion

        #region ShowMessage
        public ICommand ShowMessageCmd { get; }
        private bool CanShowMessageCmdExecute(object p) => true;
        private void OnShowMessageCmdExecuted(object p)
        {
            if (p is FileSystemObject fso)
                MessageBox.Show(fso.Path, fso.Name);
        }
        #endregion

        #endregion

        #region Constructor
        public TabItemViewModel()
        {
            worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };

            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            RunCmd = new ActionCommand(OnRunCmdExecuted, CanRunCmdExecute);
            OpenCmd = new ActionCommand(OnOpenCmdExecuted, CanOpenCmdExecute);
            BackCmd = new ActionCommand(OnBackCmdExecuted, CanBackCmdExecute);
            ShowMessageCmd = new ActionCommand(OnShowMessageCmdExecuted, CanShowMessageCmdExecute);

            OpenCmd.Execute(CurrentPath);
        }

        public void Dispose()
        {
            worker.DoWork -= worker_DoWork;
            worker.ProgressChanged -= worker_ProgressChanged;
            worker.RunWorkerCompleted -= worker_RunWorkerCompleted;
            worker.Dispose();
        }
        #endregion

        #region Private methods

        private void OpenPath()
        {
            FileSystemObjects.Clear();
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (CurrentPath == null)
            {
                var driveCount = Directory.GetLogicalDrives().Count();

                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FileSystemObjects.Add(new FileSystemObject()
                        {
                            Image = FolderManager.GetImageSource(drive.RootDirectory.FullName, Enums.ItemState.Undefined),
                            Name = drive.VolumeLabel,
                            Path = drive.Name,
                            TotalSpace = drive.TotalSize,
                            FreeSpace = drive.TotalFreeSpace,
                            Size = drive.TotalSize - drive.TotalFreeSpace,
                            Format = drive.DriveFormat,
                            Type = drive.DriveType.ToString()
                        });

                        worker.ReportProgress((int)((double)FileSystemObjects.Count / driveCount * 100));
                    }, DispatcherPriority.Background);
                }
            }
            else
            {
                var entryCount = new DirectoryInfo(CurrentPath).EnumerateFileSystemInfos().Count();

                foreach (var item in Directory.EnumerateFileSystemEntries(CurrentPath))
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var fileExists = File.Exists(item);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        FileSystemObjects.Add(new FileSystemObject()
                        {
                            Image = fileExists
                                ? FileManager.GetImageSource(item)
                                : FolderManager.GetImageSource(item, Enums.ItemState.Undefined),
                            Name = Path.GetFileName(item),
                            Path = item,
                            Size = fileExists ? new FileInfo(item).Length : 0
                        });

                        worker.ReportProgress((int)((double)FileSystemObjects.Count / entryCount * 100));
                    }, DispatcherPriority.Background);
                }
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressPercent = e.ProgressPercentage;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) OpenPath();
        }

        #endregion
    }
}
