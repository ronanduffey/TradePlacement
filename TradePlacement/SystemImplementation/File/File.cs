using System.IO;

namespace TradePlacement.SystemImplementation.File
{
    public class File : IFile
    {
        public void Copy(string path, string copyPath)
        {
            var copyDirectory = new FileInfo(copyPath).DirectoryName;
            var fileName = new FileInfo(copyPath).Name;
            Directory.CreateDirectory(copyDirectory);
            System.IO.File.Copy(path, Path.Combine(copyDirectory, fileName), true);
        }
    }
}