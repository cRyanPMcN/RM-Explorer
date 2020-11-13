using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using IconLib;

namespace RM_Explorer
{
    public class FSData : ObservableCollection<FSData>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;
        private FileSystemInfo _node { get; }
        private FSData _parent { get; }
        private FSDataContainer _owner { get; set; }
        private bool isFolder { get { return _node is DirectoryInfo; } }
        private bool isFile { get { return _node is FileInfo; } }

        public IList<FSData> children { get { return Items; } }

        // Dynamically creates a new ImageSource based on the Associated System Icon for a File or a static Image for a Folder
        public ImageSource icon
        {
            get
            {
                if (isFile)
                {
                    return Icon.ExtractAssociatedIcon(_node.FullName).ToBitmapImage();
                }
                else
                {
                    return Properties.Resources.Folder.ToBitmapImage();
                }
            }
        }
        private bool? _isChecked { get; set; }
        public bool? isChecked
        {
            get { return _isChecked; }
            set
            {
                if (value != null)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isChecked"));

                    foreach (FSData child in children)
                    {
                        child.SetFromParent(value);
                    }

                    if (_parent != null)
                    {
                        _parent.UpdateFromChild();
                    }
                    else
                    {
                        UpdateSelectedCounts();
                    }
                }
            }
        }

        private void SetFromParent(bool? value)
        {
            _isChecked = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isChecked"));
            foreach (FSData child in children)
            {
                child.SetFromParent(value);
            }
        }

        private void SetFromChild(bool? value)
        {
            _isChecked = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isChecked"));
            if (_parent != null)
            {
                _parent.SetFromChild(value);
            }
            else
            {
                UpdateSelectedCounts();
            }
        }

        private void UpdateFromChild()
        {
            bool? checkVal = null;

            if (children.Count(child => { return child._isChecked == true; }) == children.Count)
            {
                checkVal = true;
            }
            if (children.Count(child => { return child._isChecked == false; }) == children.Count)
            {
                checkVal = false;
            }

            _isChecked = checkVal;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("isChecked"));

            if (_parent != null)
            {
                if (checkVal == null)
                {
                    _parent.SetFromChild(null);
                }
                else
                {
                    _parent.UpdateFromChild();
                }
            }
            else
            {
                UpdateSelectedCounts();
            }
        }

        public void UpdateSelectedCounts()
        {
            _owner.selectedFolders = CountSelectedFolders();
            _owner.selectedFiles = CountSelectedFiles();
            _owner.selectedSize = CountSelectedSize();
        }

        private long CountSelectedFolders()
        {
            long selectedFolders = _isChecked == true ? 1 : 0;
            foreach (FSData child in children)
            {
                if(child.isFolder)
                {
                    selectedFolders += child.CountSelectedFolders();
                }
            }
            return selectedFolders;
        }

        private long CountSelectedFiles()
        {
            long selectedFiles = children.Count(child => { return child.isFile && child._isChecked == true; });
            foreach (FSData child in children)
            {
                if (child.isFolder)
                {
                    selectedFiles += child.CountSelectedFiles();
                }
            }
            return selectedFiles;
        }

        private long CountSelectedSize()
        {
            long selectedSize = 0;
            foreach (FSData child in children)
            {
                if (child.isFolder)
                {
                    selectedSize += child.CountSelectedSize();
                }
                else
                {
                    if (child._isChecked == true)
                    {
                        selectedSize += ((FileInfo)child._node).Length;
                    }
                }
            }

            return selectedSize;
        }

        public FSData(FileSystemInfo d, FSDataContainer owner)
        {
            _isChecked = false;
            _parent = null;
            _node = d;
            _owner = owner;
            LoadChildren();
        }

        public FSData(FileSystemInfo d, FSData parent, FSDataContainer owner)
        {
            _isChecked = false;
            _parent = parent;
            _node = d;
            _owner = owner;
            LoadChildren();
        }

        public FSData(string path, FSDataContainer owner)
        {
            _isChecked = false;
            _parent = null;
            _owner = owner;
            try
            {
                _node = new DirectoryInfo(path);
            }
            catch (IOException)
            {
                try
                {
                    _node = new FileInfo(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error: Unhandled Exception", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error: Unhandled Exception", MessageBoxButton.OK);
            }

            LoadChildren();
        }

        public void LoadChildren()
        {
            if (_node is DirectoryInfo)
            {
                ++_owner.folderCount;
                try
                {
                    foreach (DirectoryInfo di in ((DirectoryInfo)_node).EnumerateDirectories())
                    {
                        children.Add(new FSData(di, this, _owner));
                    }
                }
                catch(UnauthorizedAccessException)
                {
                    // Dont display folders the program is not allowed to access
                }

                foreach (FileInfo fi in ((DirectoryInfo)_node).EnumerateFiles())
                {
                    children.Add(new FSData(fi, this, _owner));
                }
            }
            else
            {
                ++_owner.fileCount;
            }
        }

        public string Name
        {
            get
            {
                return _node.Name;
            }
        }
    }
}
