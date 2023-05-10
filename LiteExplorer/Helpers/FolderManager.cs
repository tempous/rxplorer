using LiteExplorer.Helpers.Enums;
using LiteExplorer.Helpers.WinAPI;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LiteExplorer.Helpers;

public static class FolderManager
{
    public static ImageSource GetImageSource(string directory, ItemState folderType)
    {
        return GetImageSource(directory, new Size(20, 20), folderType);
    }

    public static ImageSource GetImageSource(string directory, Size size, ItemState folderType)
    {
        using (var icon = ShellManager.GetIcon(directory, ItemType.Folder, IconSize.Large, folderType))
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
        }
    }
}
