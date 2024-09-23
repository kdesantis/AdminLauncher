using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramManager
    {
        public List<ProgramItem> Programs { get; private set; } = new();

        public void AddProgram(ProgramItem program)
        {
            Programs.Add(program);
        }
        public void RemoveProgram(ProgramItem program)
        {
            Programs.Remove(program);
        }
        public ProgramItem FindProgramByName(string name)
        {
            return Programs.FirstOrDefault(program => program.Name == name);
        }
        public void Save()
        {
            PersistenceManager persistenceManager = new();
            persistenceManager.SaveData(Programs);
        }
        public void Load()
        {
            PersistenceManager persistenceManager = new();
            Programs = persistenceManager.LoadData();
        }
    }
}
