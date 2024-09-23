// See https://aka.ms/new-console-template for more information
using AdminLauncher.BusinessLibrary;

var ProgramManager = new ProgramManager();

ProgramItem notepad = new() { Index = 0, Name = "Notepad", Path = @"C:\Program Files\WindowsApps\Microsoft.WindowsNotepad_11.2406.9.0_x64__8wekyb3d8bbwe\Notepad\Notepad.exe" };
ProgramManager.AddProgram(notepad);

ProgramManager.Save();

ProgramManager.RemoveProgram(notepad);

ProgramManager.Load();

ProgramManager.FindProgramByName("Notepad").Launch();

Console.WriteLine($"progras count:{ProgramManager.Programs.Count}");
