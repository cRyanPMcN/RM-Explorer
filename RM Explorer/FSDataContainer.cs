using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace RM_Explorer
{
    public class FSDataContainer : ObservableCollection<FSData>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;
        public FSData root
        {
            get { return Items.First(); }
            set
            {
                Items.Clear();
                Items.Add(value);
            }
        }

        private long _folderCount { get; set; }
        private long _fileCount { get; set; }
        private long _selectedFolders { get; set; }
        private long _selectedFiles { get; set; }
        private long _selectedSize { get; set; }
        public long folderCount
        {
            get { return _folderCount; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                _folderCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("folderCount"));
            }
        }
        public long fileCount
        {
            get { return _fileCount; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                _fileCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("fileCount"));
            }
        }
        public long selectedFolders
        {
            get { return _selectedFolders; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                _selectedFolders = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectedFolders"));
            }
        }
        public long selectedFiles
        {
            get { return _selectedFiles; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                _selectedFiles = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectedFiles"));
            }
        }
        public long selectedSize
        {
            get { return _selectedSize; }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                _selectedSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("selectedSize"));
            }
        }

        public FSDataContainer()
        {
            root = null;
        }
        public FSDataContainer(string path)
        {
            root = new FSData(path, this);
        }

    }
}
