using System;

namespace Omnis;

internal static class Utils
{
    public static string OpenFileDialog()
    {
        Microsoft.Win32.OpenFileDialog openFileDialog = new()
        {
            Filter = "DLL Files (*.dll)|*.dll",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Multiselect = false,
            Title = "Select DLL to inject"
        };
        openFileDialog.ShowDialog();
        return openFileDialog.FileName;
    }
}