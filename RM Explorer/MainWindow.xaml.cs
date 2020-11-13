using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Runtime.Caching;

namespace RM_Explorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string search_File_Location = AppDomain.CurrentDomain.BaseDirectory + Properties.Resources.DirectorySearchFile;
        public string location
        {
            get { return (string)GetValue(locProperty); }
            set { SetValue(locProperty, value); }
        }

        public static DependencyProperty locProperty =
            DependencyProperty.Register("location", typeof(string), typeof(MainWindow), new PropertyMetadata("."));

        public FSDataContainer root
        {
            get { return (FSDataContainer)GetValue(rootProperty); }
            set { SetValue(rootProperty, value); }
        }

        // Using a DependencyProperty as the backing store for root.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty rootProperty =
            DependencyProperty.Register("root", typeof(FSDataContainer), typeof(MainWindow), new PropertyMetadata(new FSDataContainer()));

        private delegate FSDataContainer LoadFoldersDelegate(string path);


        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            location = ReadPathFromFile();
            LoadFolders();

            this.DataContext = this;
        }

        private void LoadFolders()
        {
            txtMessage.Text = $"{Properties.Resources.txtMessageSearch}\n{location}";
            txtLocation.IsEnabled = false;
            btnFilePicker.IsEnabled = false;
            LoadFoldersDelegate load_folders = ((string path) => {
                FSDataContainer newRoot = new FSDataContainer(path);
                return newRoot;
            });
            load_folders.BeginInvoke(location, FoldersLoaded, this);
        }

        private void FoldersLoaded(IAsyncResult theResults)
        {
            AsyncResult results = (AsyncResult)theResults;
            LoadFoldersDelegate source = (LoadFoldersDelegate)results.AsyncDelegate;
            this.Dispatcher.Invoke(DispatcherPriority.Background, (Action)(() =>
            {
                root = source.EndInvoke(results);
                tvDirectory.Items.Refresh();
                txtMessage.Text = $"{Properties.Resources.txtMessageDone}\n{location}";
                txtLocation.IsEnabled = true;
                btnFilePicker.IsEnabled = true;
            }));
        }

        private void btnFilePicker_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderViewer = new System.Windows.Forms.FolderBrowserDialog();
            if (folderViewer.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                location = folderViewer.SelectedPath;
                LoadFolders();
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //location = txtLocation.Text;
            if (e.Key == Key.Enter)
            {
                //location = txtLocation.Text;
                LoadFolders();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SavePathToFile();
        }

        private string ReadPathFromFile()
        {
            if (File.Exists(search_File_Location))
            {
                StreamReader fileReader = new StreamReader(search_File_Location);
                string path = fileReader.ReadLine();
                fileReader.Close();
                return path;
            }
            else
            {
                return Properties.Resources.DefaultSearchLocation;
            }
        }

        private void SavePathToFile()
        {
            StreamWriter fileWriter = new StreamWriter(search_File_Location);
            fileWriter.WriteLine(location);
            fileWriter.Close();
        }
    }
}
