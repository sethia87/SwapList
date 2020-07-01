using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using Ion.Core.Toolkit.Collections;

namespace ListViewSample1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Swaplist
        private string _market;
        private Dictionary<string, string> MasterOrderTypeDict { get; set; }
        private readonly Dictionary<string, string> _initialValues = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _changedValues = new Dictionary<string, string>();
        private ObservableCollection<string> _orderTypesList;
        private ObservableCollection<string> _availableOrderTypeList;


        public ObservableCollection<string> ShownOrderTypeList
        {
            get { return _orderTypesList; }
            set
            {
                _orderTypesList = value;
                OnPropertyChanged("ShownOrderTypeList");
            }
        }

        public ObservableCollection<string> AvailableOrderTypesList
        {
            get { return _availableOrderTypeList; }
            set
            {
                _availableOrderTypeList = value;

                OnPropertyChanged("AvailableOrderTypesList");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public MainWindowViewModel()
        {
            AvailableOrderTypesList = new ObservableCollection<string>();
            ShownOrderTypeList = new ObservableCollection<string>() {"A","B","C","D"};
        }
    }
}
