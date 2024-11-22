using System.Text.Json;

namespace AdminLauncher.BusinessLibrary
{
    public class PersistenceManager
    {
        private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AdminLauncherPrograms.json");
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public void SaveData(Manager Manager)
        {
            logger.Info($"Save manager into the file. Manager contains: programs{Manager.programManager.Programs.Count};routine{Manager.programManager.Routines.Count}");
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, JsonSerializer.Serialize(Manager));
        }
        public Manager LoadData()
        {
            if (!File.Exists(path))
                return new Manager();
            var jsonText = File.ReadAllText(path);
            var x = JsonSerializer.Deserialize<Manager>(jsonText);
            return x;
        }
        public string CreateBackupManager()
        {
            string directoryPathBackup = Path.Combine(Path.GetDirectoryName(path), "AdminLauncherBackup");
            if (!Directory.Exists(directoryPathBackup))
                Directory.CreateDirectory(directoryPathBackup);
            var now = DateTime.Now;
            string fileNameBackup = Path.Combine(directoryPathBackup, $"BACKUP-{now.Year}-{now.Month}-{now.Day}T{now.Hour}-{now.Minute}-{now.Second}-AdminLauncherPrograms.json");

            File.Copy(path, fileNameBackup);
            return fileNameBackup;
        }
    }
}
