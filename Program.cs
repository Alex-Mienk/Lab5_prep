
using System.ComponentModel.Design;
using System.IO.Compression;

namespace FileSystemEvents;

class Program
{


    private static void HandleImage(string path, string targetBaseDir)
    {
        string name = Path.GetFileName(path);
        if (Path.GetFileNameWithoutExtension(path).Length != 14)
        {
            Console.WriteLine("Skipping the file due to not correct name");
            return;
            // throw new ArgumentException("the name of the file is not of the standard"); //if I want to break when the file is not named properly
        }
            
        string year = name[..4];
        string month = name.Substring(4, 2);
        string final = name[6..]; // the name of the file with extension

        string subdir = Path.Combine(targetBaseDir, year, month); // I needed to get the directory preceding the filename in path
        Directory.CreateDirectory(subdir);

        string newPath = Path.Combine(subdir, final);

        File.Move(path, newPath); // after this I should have the file in the directory year/month with the name of the day and timestamp
    }




    private static void ZipHandle(string fullPath, string targetBaseDir)
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), "Lab5_prep");
        Console.WriteLine($"Here is where the files will be extracted: {tempRoot}");
        Directory.CreateDirectory(tempRoot);


        string extractPath = Path.Combine(tempRoot, Path.GetFileNameWithoutExtension(fullPath));
        
        if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);

        ZipFile.ExtractToDirectory(fullPath, extractPath); // this will extract the file into the temp folder

        foreach (var entry in Directory.EnumerateFiles(extractPath, "*.*", SearchOption.AllDirectories))
        {
            string ext = Path.GetExtension(entry).ToLowerInvariant();
            if (ext == ".jpg" || ext == ".jpeg")
                HandleImage(Path.GetFullPath(entry), targetBaseDir);
        }
    }


    static void Main(string[] args)
    {
        Watch(".", "*", false);
    }
    
    static void Watch(string path, string filter, bool includeSubDirs)
    {
        using var watcher = new FileSystemWatcher(path, filter);
        watcher.Created += OnCreated; // yes
        // watcher.Changed += OnChanged; // not sure
        // watcher.Deleted += OnDeleted; // not sure
        // watcher.Renamed += OnRenamed; // not sure
        watcher.Error += OnError;
        watcher.IncludeSubdirectories = includeSubDirs;
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Listening for events - press <enter> to finish");
        Console.ReadLine();
    }
    
    // private static void OnChanged(object sender, FileSystemEventArgs e)
    // {
    //     if (e.ChangeType != WatcherChangeTypes.Changed)
    //     {
    //         return;
    //     }
    //     Console.WriteLine($"Changed: {e.FullPath}");
    // }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {

        string ext = Path.GetExtension(e.FullPath).ToLower();

        switch(ext)
        {
            case ".jpg":
                Console.WriteLine("Found an image");
                HandleImage(e.FullPath, Directory.GetCurrentDirectory());
                break;
            case ".jpeg":
                Console.WriteLine("Found an image");
                HandleImage(e.FullPath, Directory.GetCurrentDirectory());
                break;

            case ".zip":
                Console.WriteLine("Found a zip");
                ZipHandle(e.FullPath, Directory.GetCurrentDirectory());
                break;

            default:
                break; //skipping other files

        }
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
    }



    // private static void OnDeleted(object sender, FileSystemEventArgs e)
    // {
    //     Console.WriteLine($"Deleted: {e.FullPath}");
    // }

    // private static void OnRenamed(object sender, RenamedEventArgs e)
    // {
    //     Console.WriteLine($"Renamed:");
    //     Console.WriteLine($"    Old: {e.OldFullPath}");
    //     Console.WriteLine($"    New: {e.FullPath}");
    // }

    private static void OnError(object sender, ErrorEventArgs e)
    {
        Console.WriteLine(e.GetException());
    }
}