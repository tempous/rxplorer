using LiteExplorer.Commands.Base;
using LiteExplorer.Extensions;
using LiteExplorer.Models;
using LiteExplorer.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace LiteExplorer.ViewModels
{
    internal class TabItemViewModel : ViewModel
    {
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

            FileSystemObjects.Clear();

            if (CurrentPath == null) GetDrives(); else GetItems();
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
            RunCmd = new ActionCommand(OnRunCmdExecuted, CanRunCmdExecute);
            OpenCmd = new ActionCommand(OnOpenCmdExecuted, CanOpenCmdExecute);
            BackCmd = new ActionCommand(OnBackCmdExecuted, CanBackCmdExecute);
            ShowMessageCmd = new ActionCommand(OnShowMessageCmdExecuted, CanShowMessageCmdExecute);

            OpenCmd.Execute(CurrentPath);
        }
        #endregion

        #region Methods

        private void GetDrives()
        {
            foreach (var drive in DriveInfo.GetDrives())
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
            }
        }

        private void GetItems()
        {
            foreach (var item in Directory.EnumerateFileSystemEntries(CurrentPath))
            {
                var fileExists = File.Exists(item);

                FileSystemObjects.Add(new FileSystemObject()
                {
                    Image = fileExists
                        ? FileManager.GetImageSource(item)
                        : FolderManager.GetImageSource(item, Enums.ItemState.Undefined),
                    Name = Path.GetFileName(item),
                    Path = item,
                    Size = fileExists ? new FileInfo(item).Length : 0
                });
            }
        }

        #endregion
    }
}
