using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public abstract class GenericItem
    {
        public int Index { get; set; }
        public virtual string Name { get; set; }

        public abstract LaunchResult Launch();
        public abstract string GetIconPath();
    }
}
