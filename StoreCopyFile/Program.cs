// See https://aka.ms/new-console-template for more information

using System.Runtime.InteropServices;

[DllImport("User32.dll", CharSet = CharSet.Unicode)]
static extern int MessageBox(IntPtr h, string m, string c, int type);

var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
var combinePath = Path.Combine(folderPath, "Store", "Path.txt");
var storeCombine = Path.Combine(folderPath, "Store", "SavePath.txt");
string path = string.Empty;
string storePath = String.Empty;
try
{
    path = File.ReadAllText(combinePath);
    storePath = File.ReadAllText(storeCombine);
}
catch (Exception e)
{
    var dirPath = Path.Combine(folderPath, "Store");
    if (!Directory.Exists(dirPath))
    {
        Directory.CreateDirectory(dirPath);
    }

    File.WriteAllText(combinePath, "");
    File.WriteAllText(storeCombine, "");
    MessageBox((IntPtr)0, "Пути файлов не были заданы, задайте и перезапустите приложение", "Ошибка", 0);
    throw;
}

using var watcher = new FileSystemWatcher(path);

watcher.NotifyFilter = NotifyFilters.Attributes
                       | NotifyFilters.CreationTime
                       | NotifyFilters.DirectoryName
                       | NotifyFilters.FileName
                       | NotifyFilters.LastAccess
                       | NotifyFilters.LastWrite
                       | NotifyFilters.Security
                       | NotifyFilters.Size;

watcher.Changed += OnChanged;
watcher.Created += OnCreated;


watcher.Filter = "*.xbak";
watcher.IncludeSubdirectories = true;
watcher.EnableRaisingEvents = true;

Thread.Sleep(-1);

void OnChanged(object sender, FileSystemEventArgs e)
{
    if (e.ChangeType != WatcherChangeTypes.Changed)
    {
        return;
    }

    try
    {
        var fileName = Path.GetFileName(e.Name);
        var store = Path.Combine(storePath, fileName);
        File.Copy(e.FullPath, store, true);
    }
    catch (Exception exception)
    {
        File.WriteAllText("error.txt", exception.Message + Environment.NewLine + exception.StackTrace);
        MessageBox((IntPtr)0, "Что-то пошло не так", "Ошибка", 0);
        throw;
    }
}

void OnCreated(object sender, FileSystemEventArgs e)
{
    try
    {
        string value = $"Created: {e.FullPath}";
        var fileName = Path.GetFileName(e.Name);
        var store = Path.Combine(storePath, fileName);
        File.Copy(e.FullPath, store, true);
    }
    catch (Exception exception)
    {
        File.WriteAllText("error.txt", exception.Message + Environment.NewLine + exception.StackTrace);
        MessageBox((IntPtr)0, "Что-то пошло не так", "Ошибка", 0);
        throw;
    }
}