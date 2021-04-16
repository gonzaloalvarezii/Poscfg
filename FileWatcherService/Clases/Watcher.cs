using System;
using System.IO;
using System.Threading.Tasks;

namespace FileWatcherService.Clases
{
    public class Watcher
        {
    
            public string Directory { get; set; }
            public string  Filter { get; set; }  
            public static DateTime _LastWrite=new DateTime(1990, 01, 01, 00, 00, 00, 000);
                        
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
        
            public Watcher(string directory, string filter)
            {
                this.Directory = directory;
                this.Filter = Filter;
            }
        
            public void StartWatch()
            {
                
                fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                fileSystemWatcher.Filter = this.Filter;
                fileSystemWatcher.Path = this.Directory;
                fileSystemWatcher.EnableRaisingEvents = true;
        
                fileSystemWatcher.Changed += new FileSystemEventHandler(fileSystemWatcher_Changed);
            }  
            async void fileSystemWatcher_Changed(object source, FileSystemEventArgs e)
            {
                WatcherChangeTypes wct = e.ChangeType;                
                if (wct.ToString() == "Changed")
                {
                    
                    DateTime currentWrite=DateTime.Parse(File.GetLastWriteTime(e.FullPath).ToString("MM/dd/yyyy HH:mm:ss.fff"));
                    _LastWrite=currentWrite;
                    
                    await Task.Delay(5000);
                    Console.WriteLine("{0} {1:MM/dd/yyyy HH:mm:ss.fff} {2:MM/dd/yyyy HH:mm:ss.fff}",wct.ToString(),currentWrite, _LastWrite);

                    if (_LastWrite == currentWrite){
                        Console.WriteLine("Invocar API: {0:MM/dd/yyyy HH:mm:ss.fff}", DateTime.Now );
                    }

                }
            }
        }
}
