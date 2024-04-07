﻿using System.Windows.Controls;
using CommunityToolkit.Mvvm.Messaging;
using DataGridWithCommunityToolKit.ViewModels;

namespace DataGridWithCommunityToolKit.Views
{
    /// <summary>
    /// Interaction logic for LinksListView.xaml
    /// </summary>
    public partial class LinksListView : UserControl
    {
        public LinksListView(IMessenger messenger)
        {
            InitializeComponent();
            IsVisibleChanged += (o, e) =>
            {
                if (e.NewValue is bool isVisible)
                {
                    VM.IsVisible = isVisible;
                }
            };
            Loaded += (o, e) =>
            {
                VM.IsLoaded = true;
            };
            Unloaded += (o, e) =>
            {
                VM.IsLoaded = false;
            };

            dataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;

            var vm = new LinksListViewModel<Uri>(messenger);
            DataContext = vm;
            vm.Dispatcher = this.Dispatcher;
            vm.View = this;
        }

        public List<T> GetSelectedItems<T>()
        {
            var list = dataGrid.SelectedItems.Cast<T>().ToList();
            return list;
        }

        public void SelectAll()
        {
            dataGrid.SelectAll();
        }

        public int SelectedIndex
        {
            get
            {
                return dataGrid.SelectedIndex;
            }
            set
            {
                try
                {
                    dataGrid.SelectedIndex = value;
                }
                catch (Exception ex)
                {
                    VM.LastException = ex;
                }
            }
        }

        private LinksListViewModel<Uri> VM
        {
            get
            {
                return (LinksListViewModel<Uri>)DataContext;
            }
        }
    }
}
