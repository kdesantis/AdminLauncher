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
        public int CurrIndex { get; set; }

        public void AddProgram(ProgramItem program)
        {
            int index;
            var existProgram = Programs.FirstOrDefault(p => p.Index == program.Index);
            if (existProgram != null)
            {
                index = existProgram.Index;
                Programs.Remove(existProgram);
            }
            else
            {
                program.Index = CurrIndex;
                CurrIndex++;
            }
            Programs.Add(program);
        }
        public void RemoveProgram(ProgramItem program)
        {
            Programs.Remove(program);
        }
        public void AddRoutine(RoutineItem routine)
        {
            int index;
            var existRoutine = Routines.FirstOrDefault(p => p.Index == routine.Index);
            if (existRoutine != null)
            {
                index = existRoutine.Index;
                Routines.Remove(existRoutine);
            }
            else
            {
                routine.Index = CurrIndex;
                CurrIndex++;
            }
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
            try
            {
                PersistenceManager persistenceManager = new();
                var savedProgramManager = new PersistenceManager().LoadData();
                Programs = savedProgramManager.Programs;
                Routines = savedProgramManager.Routines;
                CurrIndex = savedProgramManager.CurrIndex;
            }
            catch (Exception)
            {
                //Ignore configuration File
            }
        }
    }
}
