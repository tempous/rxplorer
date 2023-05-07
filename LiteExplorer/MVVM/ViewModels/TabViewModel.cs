using LiteExplorer.Extensions;
using LiteExplorer.Extensions.Enums;
using LiteExplorer.Infrastructure.Commands;
using LiteExplorer.MVVM.Models;
using LiteExplorer.MVVM.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace LiteExplorer.MVVM.ViewModels;

internal class TabViewModel : ViewModel, IDisposable
{
    #region Private fields

    private readonly BackgroundWorker worker;

    #endregion

    #region Properties

    #region TabTitle

    private string tabTitle;

    public string TabTitle
    {
        get => tabTitle;
        set => SetValue(ref tabTitle, value);
    }

    #endregion

    #region TabPath

    private string tabPath;

    public string TabPath
    {
        get => tabPath;
        set => SetValue(ref tabPath, value);
    }

    #endregion

    #region TabCommand

    private string tabCommand;

    public string TabCommand
    {
        get => tabCommand;
        set => SetValue(ref tabCommand, value);
    }

    #endregion

    #region CurrentFileSystemObject

    private FileSystemObject currentFileSystemObject;

    public FileSystemObject CurrentFileSystemObject
    {
        get => currentFileSystemObject;
        set => SetValue(ref currentFileSystemObject, value);
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

    private void OnRunCmdExecuted(object p) => Process.Start(new ProcessStartInfo("cmd", $"/k {p}") { WorkingDirectory = TabPath ?? Environment.SystemDirectory });

    #endregion

    #region Open

    public ICommand OpenCmd { get; }

    private bool CanOpenCmdExecute(object p) => true;

    private void OnOpenCmdExecuted(object p)
    {
        string path = p?.ToString();

        if (path is null || Directory.Exists(path))
        {
            TabPath = path;

            if (worker.IsBusy)
                worker.CancelAsync();
            else
                OpenPath();
        }
        else if (File.Exists(path))
        {
            try
            {
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(ex.Message, $"Error: {ex.NativeErrorCode}", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            Process.Start(new ProcessStartInfo($"https://www.google.com/?q={Uri.EscapeDataString(path)}") { UseShellExecute = true });
        }
    }

    #endregion

    #region Back

    public ICommand BackCmd { get; }

    private bool CanBackCmdExecute(object p) => p != null;

    private void OnBackCmdExecuted(object p)
    {
        TabPath = Directory.GetParent(p.ToString())?.FullName;

        if (worker.IsBusy)
            worker.CancelAsync();
        else
            OpenPath();
    }

    #endregion

    #endregion

    #region Constructor

    public TabViewModel(string path = null)
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

        OpenCmd.Execute(path);
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
        TabTitle = TabPath != null && Path.GetPathRoot(TabPath) == TabPath
            ? GetDriveLabel(new DriveInfo(TabPath))
            : Path.GetFileName(TabPath);
        FileSystemObjects.Clear();
        worker.RunWorkerAsync();
    }

    private string GetDriveLabel(DriveInfo drive) => $"{(drive.VolumeLabel != "" ? drive.VolumeLabel : "Local drive")} ({drive.Name})";

    private void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        if (TabPath == null)
        {
            var driveCount = Directory.GetLogicalDrives().Length;

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    FileSystemObjects.Add(new FileSystemObject()
                    {
                        Image = FolderManager.GetImageSource(drive.RootDirectory.FullName, ItemState.Undefined),
                        Name = GetDriveLabel(drive),
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
            try
            {
                var entryCount = new DirectoryInfo(TabPath).EnumerateFileSystemInfos().Count();

                foreach (var item in Directory.EnumerateFileSystemEntries(TabPath))
                {
                    if (worker.CancellationPending)
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
                                : FolderManager.GetImageSource(item, ItemState.Undefined),
                            Name = Path.GetFileName(item),
                            Path = item,
                            Size = fileExists ? new FileInfo(item).Length : 0
                        });

                        worker.ReportProgress((int)((double)FileSystemObjects.Count / entryCount * 100));
                    }, DispatcherPriority.Background);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Access denied", MessageBoxButton.OK, MessageBoxImage.Error);
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
