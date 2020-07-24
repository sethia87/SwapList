using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ion.Core.Toolkit.Command;

namespace ListViewSample1
{
    public class SwapListControl1 : Control
    {
        private Dictionary<string, string> MasterOrderTypeDict { get; set; }
        private Dictionary<int, string> _indexedItem = new Dictionary<int, string>();
        private Dictionary<int, string> _indexedSelectedShownItem = new Dictionary<int, string>();
        private Dictionary<int, string> _indexedOrderType = new Dictionary<int, string>();
        private Dictionary<int, string> _indexedSelectedShownOrderType = new Dictionary<int, string>();
        private readonly Dictionary<string, string> _initialValues = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _changedValues = new Dictionary<string, string>();
        public RelayCommand<object> SelectedAvailableOrderTypesListCommand { get; set; }
        public RelayCommand<object> SelectedShownOrderTypeListCommand { get; set; }


        public SwapListControl1()
        {
            MasterOrderTypeDict = new Dictionary<string, string>();
            ShownOrderTypeList = new ObservableCollection<string>();
            SelectedAvailableOrderTypesListCommand = new RelayCommand<object>(GetSelectedAvailableItem);
            SelectedShownOrderTypeListCommand = new RelayCommand<object>(GetSelectedShownItem);
        }


        public static readonly DependencyProperty AvailableOrderTypesListProperty = DependencyProperty.Register(
            "AvailableOrderTypesList", typeof(ObservableCollection<string>), typeof(SwapListControl1), null);

        public ObservableCollection<string> AvailableOrderTypesList
        {
            get { return (ObservableCollection<string>)GetValue(AvailableOrderTypesListProperty); }
            set { SetValue(AvailableOrderTypesListProperty, value); }
        }

        public static readonly DependencyProperty ShownOrderTypeListProperty = DependencyProperty.Register(
            "ShownOrderTypeList", typeof(ObservableCollection<string>), typeof(SwapListControl1), null);

        public ObservableCollection<string> ShownOrderTypeList
        {
            get { return (ObservableCollection<string>)GetValue(ShownOrderTypeListProperty); }
            set { SetValue(ShownOrderTypeListProperty, value); }
        }

        public static readonly DependencyProperty SelectedAvailableOrderTypesProperty = DependencyProperty.Register(
            "SelectedAvailableOrderTypes", typeof(IList), typeof(SwapListControl1), new FrameworkPropertyMetadata(OnSelectedAvailableOrderTypesChanged));

        private static void OnSelectedAvailableOrderTypesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwapListControl1 control = d as SwapListControl1;
            if (control == null)
                return;

            var value = (IList)e.NewValue;
           
            control._indexedOrderType.Clear();

            foreach (string orderType in value)
            {
                var index = control.AvailableOrderTypesList.IndexOf(orderType);
                control._indexedOrderType.Add(index + 1, orderType);
            }
            control._indexedOrderType = control._indexedOrderType.OrderBy((ot) => ot.Key).Select((orderType) => orderType).ToDictionary(x => x.Key, x => x.Value);
        }

        public static readonly DependencyProperty SelectedShownOrderTypesProperty = DependencyProperty.Register(
            "SelectedShownOrderTypes", typeof(IList), typeof(SwapListControl1), new FrameworkPropertyMetadata(OnSelectedShownOrderTypesChanged));

        private static void OnSelectedShownOrderTypesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwapListControl1 control = d as SwapListControl1;
            if (control == null)
                return;

            var value = (IList)e.NewValue;


            control._indexedSelectedShownOrderType.Clear();
            foreach (string orderType in value)
            {
                var index = control.ShownOrderTypeList.IndexOf(orderType);
                control._indexedSelectedShownOrderType.Add(index, orderType);
            }
            control._indexedSelectedShownOrderType = control._indexedSelectedShownOrderType.OrderBy((ot) => ot.Key).Select((orderType) => orderType).ToDictionary(x => x.Key, x => x.Value);
        }

        public IList SelectedAvailableOrderTypes
        {
            get { return (IList)GetValue(SelectedAvailableOrderTypesProperty); }
            set { SetValue(SelectedAvailableOrderTypesProperty, value); }
        }

        public IList SelectedShownOrderTypes
        {
            get { return (IList)GetValue(SelectedShownOrderTypesProperty); }
            set { SetValue(SelectedShownOrderTypesProperty, value); }
        }


        #region Command Triggers
        private void GetSelectedAvailableItem(object values)
        {
            _indexedItem.Clear();

            var listOfValues = values as IList;
            if (listOfValues != null)
            {
                foreach (string item in listOfValues)
                {
                    var index = AvailableOrderTypesList.IndexOf(item);
                    _indexedItem.Add(index + 1, item);
                }
                _indexedItem = _indexedItem.OrderBy((ot) => ot.Key).Select((item) => item).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private void GetSelectedShownItem(object values)
        {
            _indexedSelectedShownItem.Clear();

            var listOfValues = values as IList;
            if (listOfValues != null)
            {
                foreach (string item in listOfValues)
                {
                    var index = ShownOrderTypeList.IndexOf(item);
                    _indexedSelectedShownItem.Add(index, item);
                }
                _indexedSelectedShownItem = _indexedSelectedShownItem.OrderBy((ot) => ot.Key).Select((item) => item).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public void MoveRightCommandTriggered()
        {
            var tempList = new List<string>(_indexedItem.Values);
            foreach (string selectedAvailableItem in tempList)
            {
                ShownOrderTypeList.Add(selectedAvailableItem);
                AvailableOrderTypesList.Remove(selectedAvailableItem);
            }
        }

        public void MoveLeftCommandTriggered()
        {
            if (_indexedSelectedShownItem.Values.Contains("Algorithms"))
                return;
            var tempList = new List<string>(_indexedSelectedShownItem.Values);
            foreach (string selectedAvailableItem in tempList)
            {
                if (!AvailableOrderTypesList.Contains(selectedAvailableItem))
                    AvailableOrderTypesList.Add(selectedAvailableItem);
                if (ShownOrderTypeList.Contains(selectedAvailableItem))
                    ShownOrderTypeList.Remove(selectedAvailableItem);

            }
        }

        public void MoveAllUpCommandTriggered()
        {
            //if (SelectedShownOrderTypes.Count > 0)
            {
                var diffOfShownSelectedList = new List<string>(ShownOrderTypeList.Except(_indexedSelectedShownItem.Values));
                var selectedAvlList = new List<string>(_indexedSelectedShownItem.Values);
                ShownOrderTypeList.Clear();
                foreach (var item in selectedAvlList.Union(diffOfShownSelectedList))
                {
                    ShownOrderTypeList.Add(item);
                }
            }
        }

        public void MoveAllDownCommandTriggered()
        {
            //if (SelectedShownOrderTypes.Count > 0)
            {
                var diffOfShownSelectedList = new List<string>(ShownOrderTypeList.Except(_indexedSelectedShownItem.Values));
                var selectedAvlList = new List<string>(_indexedSelectedShownItem.Values);
                ShownOrderTypeList.Clear();
                diffOfShownSelectedList.AddRange(selectedAvlList);
                foreach (var item in diffOfShownSelectedList)
                {
                    ShownOrderTypeList.Add(item);
                }
            }
        }

        public void MoveUpCommandTriggered()
        {
            //if (SelectedShownOrderTypes.Count > 0)
            {
                var selectedAvlList = new List<string>(_indexedSelectedShownItem.Values);

                int indexOfFirstItem = _indexedSelectedShownItem.FirstOrDefault().Key;
                int newIndexOfFirstItem = indexOfFirstItem - 1;

                if (newIndexOfFirstItem == -1)
                {
                    newIndexOfFirstItem = 0;
                    foreach (var item in selectedAvlList)
                    {
                        ShownOrderTypeList.Remove(item);
                        ShownOrderTypeList.Insert(newIndexOfFirstItem, item);
                        newIndexOfFirstItem++;
                    }
                }
                else
                {
                    foreach (var item in selectedAvlList)
                    {
                        if (newIndexOfFirstItem >= 0)
                        {
                            ShownOrderTypeList.Remove(item);
                            ShownOrderTypeList.Insert(newIndexOfFirstItem, item);
                        }
                        newIndexOfFirstItem++;
                    }
                }
            }
        }

        public void MoveDownCommandTriggered()
        {
            // if (SelectedShownOrderTypes.Count > 0)
            {
                var selectedAvlList = new List<string>(_indexedSelectedShownItem.Values);
                int indexOfLastItem = _indexedSelectedShownItem.LastOrDefault().Key;
                int newIndexOfLastItem = (indexOfLastItem < ShownOrderTypeList.Count - 1) ? indexOfLastItem + 1 : indexOfLastItem;

                if (newIndexOfLastItem == ShownOrderTypeList.Count - 1)
                {
                    foreach (var item in selectedAvlList)
                    {
                        ShownOrderTypeList.Remove(item);
                        ShownOrderTypeList.Add(item);
                    }
                }
                else
                {
                    string itemInsertionValue = ShownOrderTypeList[newIndexOfLastItem];
                    foreach (var item in selectedAvlList)
                    {
                        ShownOrderTypeList.Remove(item);
                    }
                    int lastShownItemIndex = ShownOrderTypeList.IndexOf(itemInsertionValue) + 1;
                    foreach (var item in selectedAvlList)
                    {
                        ShownOrderTypeList.Insert(lastShownItemIndex, item);

                        if (lastShownItemIndex < 4)
                            lastShownItemIndex++;
                    }
                }
            }
        }
        #endregion

    }
}
