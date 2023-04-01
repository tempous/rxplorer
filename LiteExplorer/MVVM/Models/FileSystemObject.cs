using System.Windows.Media;

namespace LiteExplorer.MVVM.Models;

internal class FileSystemObject
{
    public ImageSource Image { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public long Size { get; set; }
    public long TotalSpace { get; set; }
    public long FreeSpace { get; set; }
    public string Format { get; set; }
    public string Type { get; set; }
}
