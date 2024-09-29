using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class RoutineItem : GenericItem
    {
        private string name;
        public override string Name
        {
            get { return name.EndsWith("(routine)") ? name : name + "(routine)" ; }
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

        public override void Launch()
        {
            foreach (var program in Programs)
                program.Launch();
        }

        public override string GetIconPath()
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "list.png"));
        }
    }
}
