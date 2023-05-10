using LiteExplorer.Helpers.Enums;
using LiteExplorer.Helpers.WinAPI;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LiteExplorer.Helpers;

public static class FileManager
{
    public static ImageSource GetImageSource(string filename)
    {
        return GetImageSource(filename, new Size(20, 20));
    }

    public static ImageSource GetImageSource(string filename, Size size)
    {
        using (var icon = ShellManager.GetIcon(Path.GetExtension(filename), ItemType.File, IconSize.Small, ItemState.Undefined))
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size.Width, size.Height));
        }
    }
}
