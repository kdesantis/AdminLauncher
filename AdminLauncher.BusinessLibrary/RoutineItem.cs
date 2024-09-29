using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class RoutineItem
    {
        public int Index { get; set; }
        private string name;

        public string Name
        {
            get { return $"{name}(routine)"; }
            set { name = value; }
        }

        public List<ProgramItem> Programs { get; set; }

        public void AddProgram(ProgramItem program)
        {
            if (!Programs.Contains(program))
                Programs.Add(program);
        }
        public void RemoveProgram(ProgramItem program)
        {
            if (Programs.Contains(program))
                Programs.Remove(program);
        }

        public void Launch()
        {
            foreach (var program in Programs)
                program.Launch();
        }
    }
}
