using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Omnis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            //Access the app settings to make sure that they have injected before 
            if (Properties.Settings.Default.prevouisInjectPaths == null)
            {
                Properties.Settings.Default.prevouisInjectPaths = new();
                return;
            }

            foreach (string? path in Properties.Settings.Default.prevouisInjectPaths)
            {
                //Add the path to the list box
                ListBox.Items.Add(path);
            }

            statusLabel.Content = "Status: Ready";

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            // Save all new paths to the app settings
            Properties.Settings.Default.prevouisInjectPaths.Clear(); // We have to do it this way because of it being a string collection
            Properties.Settings.Default.prevouisInjectPaths.AddRange(ListBox.Items.Cast<string>().ToArray()); 
            Properties.Settings.Default.Save();

            Environment.Exit(0);
        }

        private void Minimize_Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Utils.OpenFileDialog();

            if (filePath == "") return;
            
            ListBox.Items.Add(filePath);
            statusLabel.Content = "Status: File added";
        }


        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox.SelectedItem == null) return;
            
            ListBox.Items.Remove(ListBox.SelectedItem);
            
            statusLabel.Content = "Status: File Removed";
        }

        private void Inject_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {
                if (Process.GetProcessesByName("Minecraft.Windows").Count() == 0)
                {
                    statusLabel.Content = "Status: Please open Minecraft";
                    return;
                }
                Utils.Inject(ListBox.SelectedItem.ToString());
                statusLabel.Content = "Status: Injected";
            }
            else
                statusLabel.Content = "Status: Select a file";
        }
    }
}
