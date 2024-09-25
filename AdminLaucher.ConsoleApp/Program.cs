// See https://aka.ms/new-console-template for more information
using AdminLauncher.BusinessLibrary;

var ProgramManager = new ProgramManager();

ProgramItem notepad = new() { Index = 0, Name = "Notepad", Path = @"C:\Program Files\Notepad++\notepad++.exe" };
ProgramManager.AddProgram(notepad);
for (int i = 0; i < 50; i++)
{
    ProgramItem VS = new() { Index = 0, Name = $"Visual Studio 2022-{i}", Path = @"C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe" };
    ProgramManager.AddProgram(VS);

}
ProgramManager.Save();

//ProgramManager.RemoveProgram(notepad);

//ProgramManager.Load();

//ProgramManager.FindProgramByName("Notepad").Launch();

Console.WriteLine($"programs count:{ProgramManager.Programs.Count}");
