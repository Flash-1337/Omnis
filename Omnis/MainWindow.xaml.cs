using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Omnis.Injection;

namespace Omnis;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        //Access the app settings to make sure that they have injected before 
        if (Properties.Settings.Default.prevouisInjectPaths == null)
        {
            Properties.Settings.Default.prevouisInjectPaths = new StringCollection();
            return;
        }

        foreach (var path in Properties.Settings.Default.prevouisInjectPaths)
            //Add the path to the list box
            ListBox.Items.Add(path);

        StatusLabel.Content = "Status: Ready";
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void Close_Button_Click(object sender, RoutedEventArgs e)
    {
        // Save all new paths to the app settings
        Properties.Settings.Default.prevouisInjectPaths
            .Clear(); // We have to do it this way because of it being a string collection
        Properties.Settings.Default.prevouisInjectPaths.AddRange(ListBox.Items.Cast<string>().ToArray());
        Properties.Settings.Default.Save();

        Environment.Exit(0);
    }

    private void Minimize_Button_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Add_Button_Click(object sender, RoutedEventArgs e)
    {
        var filePath = Utils.OpenFileDialog();

        if (filePath == "") return;

        ListBox.Items.Add(filePath);
        StatusLabel.Content = "Status: File added";
    }


    private void Remove_Button_Click(object sender, RoutedEventArgs e)
    {
        if (ListBox.SelectedItem == null) return;
        ListBox.Items.Remove(ListBox.SelectedItem);
        StatusLabel.Content = "Status: File Removed";
    }

    private void Inject_Button_Click(object sender, RoutedEventArgs e)
    {
        if (ListBox.SelectedItem == null)
        {
            StatusLabel.Content = "Status: Select a file";
            return;
        }

        if (!Process.GetProcessesByName("Minecraft.Windows").Any())
        {
            StatusLabel.Content = "Status: Please open Minecraft";
            return;
        }

        InjectorFactory.GetInjector(Injectors.LoadLibrary)
            .Inject(ListBox.SelectedItem.ToString() ?? throw new InvalidOperationException());
        StatusLabel.Content = "Status: Injected";
    }
}