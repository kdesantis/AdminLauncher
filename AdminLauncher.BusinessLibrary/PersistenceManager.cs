using System.Text.Json;

namespace AdminLauncher.BusinessLibrary
{
    public class PersistenceManager
    {
        private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AdminLauncherPrograms.json");
        public void SaveData(ProgramManager programManager)
        {
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, JsonSerializer.Serialize(programManager));
        }
        public ProgramManager LoadData()
        {
            if (!File.Exists(path))
                return new ProgramManager();
            var jsonText = File.ReadAllText(path);
            var x = JsonSerializer.Deserialize<ProgramManager>(jsonText);
            return x;
        }
    }
}
