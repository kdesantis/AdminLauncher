using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramManager
    {
        public List<ProgramItem> Programs { get; set; } = new();
        public List<RoutineItem> Routines { get; set; } = new();

        public void AddProgram(ProgramItem program)
        {
            Programs.Add(program);
        }
        public void RemoveProgram(ProgramItem program)
        {
            Programs.Remove(program);
        }

        public void AddRoutine(RoutineItem routine)
        {
            Routines.Add(routine);
        }
        public void RemoveRoutine(RoutineItem routine)
        {
            Routines.Remove(routine);
        }

        public ProgramItem FindProgramByName(string name)
        {
            return Programs.FirstOrDefault(program => program.Name == name);
        }
        public void Save()
        {
            PersistenceManager persistenceManager = new();
            persistenceManager.SaveData(this);
        }
        public void Load()
        {
            PersistenceManager persistenceManager = new();
            var savedProgramManager = new PersistenceManager().LoadData();
            Programs = savedProgramManager.Programs;
            Routines = savedProgramManager.Routines;
        }
    }
}
