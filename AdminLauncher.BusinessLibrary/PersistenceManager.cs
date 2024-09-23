using System.Text.Json;

namespace AdminLauncher.BusinessLibrary
{
    public class PersistenceManager
    {
        private string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AdminLauncherPrograms.json");
        public void SaveData(List<ProgramItem> programs)
        {
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, JsonSerializer.Serialize(programs));
        }
        public List<ProgramItem> LoadData()
        {
            if (!File.Exists(path))
                return new List<ProgramItem>();
            return JsonSerializer.Deserialize<List<ProgramItem>>(File.ReadAllText(path));
        }
    }
}
