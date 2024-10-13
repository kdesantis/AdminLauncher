using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class Manager
    {
        public ProgramManager programManager { get; set; } = new ProgramManager();
        public SettingsManager settingsManager { get; set; } = new SettingsManager();

        public void Save()
        {
            PersistenceManager persistenceManager = new();
            persistenceManager.SaveData(this);
        }
        public bool Load()
        {
            bool succes = true;
            try
            {
                var savedData = new PersistenceManager().LoadData();
                programManager.Programs = savedData.programManager.Programs;
                programManager.Routines = savedData.programManager.Routines;
                programManager.CurrIndex = savedData.programManager.CurrIndex;

                settingsManager.ButtonsOrientation = savedData.settingsManager.ButtonsOrientation;
            }
            catch (Exception)
            {
                //Ignore configuration File
                succes = false;
            }
            return succes;
        }
    }
}
