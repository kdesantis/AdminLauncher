﻿namespace AdminLauncher.BusinessLibrary
{

    public abstract class GenericItem
    {
        public int Index { get; set; }

        private string name;

        public virtual string Name
        {
            get { return name; }
            set { name = string.IsNullOrEmpty(value) ? $"GenericProgram" : value; }
        }

        public string CustomIconPath { get; set; }
        public abstract LaunchResult Launch();
        public abstract string GetIconPath();

        public override string ToString() => name;
    }
}
