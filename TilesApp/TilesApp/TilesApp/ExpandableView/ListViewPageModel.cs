using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace TilesApp.ExpandableView
{
    class ListViewPageModel
    {
        public ObservableCollection<Item> ItemsList { get; set; }

        public Item PreviousSelectedItem { get; set; }
        private Item _selectedItem { get; set; }
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    ExpandOrCollapseSelectedItem();
                }
            }
        }

        private void ExpandOrCollapseSelectedItem()
        {
            if (PreviousSelectedItem != null)
            {
                ItemsList.Where(t => t.Id == PreviousSelectedItem.Id).FirstOrDefault().IsVisible =
                false;
            }

            ItemsList.Where(t => t.Id == SelectedItem.Id).FirstOrDefault().IsVisible =
                true;
            PreviousSelectedItem = SelectedItem;
        }

        public ListViewPageModel()
        {
            ItemsList = new ObservableCollection<Item>
            {
                new Item(){ Id = 1, Ref = "23412", SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "16 u", Steps = "1 to" },
                new Item(){ Id = 2, Ref = "05448", SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "10 u", Steps = "1 to" },
                new Item(){ Id = 3, Ref = "23412",SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "9 u", Steps = "2 to" },
                new Item(){ Id = 4, Ref = "74854",SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "6 u", Steps = "3 to" }
            };
        }

    }

    public class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int Id { get; set; }
        public string Ref { get; set; }
        public string SalesRef { get; set; }
        public string Manufacture { get; set; }
        public string Steps { get; set; }

        private bool _isVisible { get; set; }
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
