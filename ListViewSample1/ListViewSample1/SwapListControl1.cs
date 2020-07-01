using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ListViewSample1
{
    public class SwapListControl1 : Control
    {
        private Dictionary<string, string> MasterOrderTypeDict { get; set; }
        private Dictionary<int, string> _indexedOrderType = new Dictionary<int, string>();
        private Dictionary<int, string> _indexedSelectedShownOrderType = new Dictionary<int, string>();
        private readonly Dictionary<string, string> _initialValues = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _changedValues = new Dictionary<string, string>();

        public RelayCommand SelectedShownOrderTypesCommand { get; set; }


        public SwapListControl1()
        {
            MasterOrderTypeDict = new Dictionary<string, string>();
            ShownOrderTypeList = new ObservableCollection<string>();
            SelectedShownOrderTypesCommand = new RelayCommand(Method);
        }

        private void Method(object obj)
        {
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
        public void MoveRightCommandTriggered()
        {
            var tempList = new List<string>(_indexedOrderType.Values);
            foreach (string selectedAvailableOrderType in tempList)
            {
                ShownOrderTypeList.Add(selectedAvailableOrderType);
                AvailableOrderTypesList.Remove(selectedAvailableOrderType);
            }
            //ClearSelection();
            //MarkSelectedOrderTypes(tempList);
        }

        public void MoveLeftCommandTriggered()
        {
            if (_indexedSelectedShownOrderType.Values.Contains("Algorithms"))
                return;
            var tempList = new List<string>(_indexedSelectedShownOrderType.Values);
            foreach (string selectedAvailableOrderType in tempList)
            {
                if (!AvailableOrderTypesList.Contains(selectedAvailableOrderType))
                    AvailableOrderTypesList.Add(selectedAvailableOrderType);
                if (ShownOrderTypeList.Contains(selectedAvailableOrderType))
                    ShownOrderTypeList.Remove(selectedAvailableOrderType);

            }
            //ClearSelection();
            //MarkSelectedAvlOrderTypes(tempList);
        }

        public void MoveAllUpCommandTriggered()
        {
            if (SelectedShownOrderTypes.Count > 0)
            {
                var diffOfOrderType = new List<string>(ShownOrderTypeList.Except(_indexedSelectedShownOrderType.Values));
                var selectedAvlOrderTypes = new List<string>(_indexedSelectedShownOrderType.Values);
                ShownOrderTypeList.Clear();
                foreach (var orderType in selectedAvlOrderTypes.Union(diffOfOrderType))
                {
                    ShownOrderTypeList.Add(orderType);
                }
                //MarkSelectedOrderTypes(selectedAvlOrderTypes);
            }
        }

        public void MoveAllDownCommandTriggered()
        {
            if (SelectedShownOrderTypes.Count > 0)
            {
                var diffOfOrderType = new List<string>(ShownOrderTypeList.Except(_indexedSelectedShownOrderType.Values));
                var selectedAvlOrderTypes = new List<string>(_indexedSelectedShownOrderType.Values);
                ShownOrderTypeList.Clear();
                diffOfOrderType.AddRange(selectedAvlOrderTypes);
                foreach (var orderType in diffOfOrderType)
                {
                    ShownOrderTypeList.Add(orderType);
                }
                // MarkSelectedOrderTypes(selectedAvlOrderTypes);
            }
        }

        public void MoveUpCommandTriggered()
        {
            if (SelectedShownOrderTypes.Count > 0)
            {
                var selectedAvlOrderTypes = new List<string>(_indexedSelectedShownOrderType.Values);

                int indexOfFirstItem = _indexedSelectedShownOrderType.FirstOrDefault().Key;
                int newIndexOfFirstItem = indexOfFirstItem - 1;

                if (newIndexOfFirstItem == -1)
                {
                    newIndexOfFirstItem = 0;
                    foreach (var orderType in selectedAvlOrderTypes)
                    {
                        ShownOrderTypeList.Remove(orderType);
                        ShownOrderTypeList.Insert(newIndexOfFirstItem, orderType);
                        newIndexOfFirstItem++;
                    }
                }
                else
                {
                    foreach (var orderType in selectedAvlOrderTypes)
                    {
                        if (newIndexOfFirstItem >= 0)
                        {
                            ShownOrderTypeList.Remove(orderType);
                            ShownOrderTypeList.Insert(newIndexOfFirstItem, orderType);
                        }
                        newIndexOfFirstItem++;
                    }
                }
                //MarkSelectedOrderTypes(selectedAvlOrderTypes);
            }
        }

        public void MoveDownCommandTriggered()
        {
            if (SelectedShownOrderTypes.Count > 0)
            {
                var selectedAvlOrderTypes = new List<string>(_indexedSelectedShownOrderType.Values);
                int indexOfLastItem = _indexedSelectedShownOrderType.LastOrDefault().Key;
                int newIndexOfLastItem = (indexOfLastItem < ShownOrderTypeList.Count - 1) ? indexOfLastItem + 1 : indexOfLastItem;

                if (newIndexOfLastItem == ShownOrderTypeList.Count - 1)
                {
                    foreach (var orderType in selectedAvlOrderTypes)
                    {
                        ShownOrderTypeList.Remove(orderType);
                        ShownOrderTypeList.Add(orderType);
                    }
                }
                else
                {
                    string orderTypeInsertionValue = ShownOrderTypeList[newIndexOfLastItem];
                    foreach (var orderType in selectedAvlOrderTypes)
                    {
                        ShownOrderTypeList.Remove(orderType);
                    }
                    int lastShownItemIndex = ShownOrderTypeList.IndexOf(orderTypeInsertionValue) + 1;
                    foreach (var orderType in selectedAvlOrderTypes)
                    {
                        ShownOrderTypeList.Insert(lastShownItemIndex, orderType);

                        if (lastShownItemIndex < 4)
                            lastShownItemIndex++;
                    }
                }
                //MarkSelectedOrderTypes(selectedAvlOrderTypes);
            }
        }

        //private void GotFocusShownOrderTypeTriggered()
        //{
        //    SelectedAvlOrderTypes.Clear();
        //}

        //private void GotFocusAvailableOrderTypeTriggered()
        //{
        //    SelectedOrderTypes.Clear();
        //}
        #endregion

    }
}
