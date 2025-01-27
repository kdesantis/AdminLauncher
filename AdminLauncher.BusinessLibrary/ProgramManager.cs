using NLog;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramManager : ICloneable
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public List<ProgramItem> Programs { get; set; } = [];
        public List<RoutineItem> Routines { get; set; } = [];
        public int CurrIndex { get; set; }

        public void AddProgram(ProgramItem program)
        {
            logger.Info("Add program {program.Name}", program.Name);
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
            logger.Info("Added program {program.Index}.{program.Name}", program.Index, program.Name);
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
        public GenericItem? FindItemByIndex(int index)
        {
            if (Programs.Any(program => program.Index == index))
                return Programs.FirstOrDefault(e => e.Index == index);
            else if (Routines.Any(routine => routine.Index == index))
                return Routines.FirstOrDefault(e => e.Index == index);
            else
                return null;
        }
        public List<ProgramItem> GetFilteredPrograms(string filter)
        {
            return Programs.Where(program => program.Name.ToLower().Contains(filter.ToLower())).ToList();
        }
        public List<RoutineItem> GetFilteredRoutines(string filter)
        {
            return Routines.Where(routine => routine.Name.ToLower().Contains(filter.ToLower())).ToList();
        }

        public object Clone()
        {
            var clonedPrograms = Programs.Select(program => (ProgramItem)program.Clone()).ToList();
            var clonedRoutines = Routines.Select(routine => (RoutineItem)routine.Clone()).ToList();
            return new ProgramManager
            {
                Programs = clonedPrograms,
                Routines = clonedRoutines,
                CurrIndex = this.CurrIndex
            };
        }
    }
}
