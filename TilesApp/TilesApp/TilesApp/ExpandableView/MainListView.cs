using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TilesApp.ExpandableView;

namespace ExpendableListView
{
    public class MainListView
    {
        private Unit _oldItem;
        String CollapseUp = "\uf077";
        String CollapseDown = "\uf078";
        public ObservableCollection<Unit> Units { get; set; }
        public bool IsVisible { get; set; }
        private Unit _selectedItem { get; set; }
        public Unit SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    ShoworHiddenProducts(_selectedItem);
                }
            }
        }

        public MainListView()
        {

            Units = new ObservableCollection<Unit>
            {
                new Unit(){ Id = 1, IsVisible=false, FontAwsomeName=CollapseDown, Ref = "23412", SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "16 u", Steps = "1 to", Date="Oct. 10th 2019", WOId="23465", Progress=0.8},
                new Unit(){ Id = 2, IsVisible=false, FontAwsomeName=CollapseDown,Ref = "05448", SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "10 u", Steps = "1 to", Date="Oct. 11th 2019 (Today)", WOId="67955", Progress=0.1},
                new Unit(){ Id = 3, IsVisible=false, FontAwsomeName=CollapseDown,Ref = "23412",SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "9 u", Steps = "2 to", Date="Oct. 11th 2019 (Today)", WOId="46991", Progress=0.75},
                new Unit(){ Id = 4, IsVisible=false, FontAwsomeName=CollapseDown,Ref = "74854",SalesRef= "Tile Type 00000/00000/03/00/00/00/00000/00000", Manufacture = "6 u", Steps = "3 to", Date="Oct. 12th 2019", WOId="17693", Progress=0.33}

            };
        }
        //FAChevronDown
        public void ShoworHiddenProducts(Unit Unit)
        {
            if (_oldItem == Unit)
            {
                Unit.IsVisible = !Unit.IsVisible;
                if (Unit.FontAwsomeName == CollapseDown)
                {
                    Unit.FontAwsomeName = CollapseUp;
                }
                else
                {
                    Unit.FontAwsomeName = CollapseDown;
                }
                UpDateProducts(Unit);
            }
            else
            {
                if (_oldItem != null)
                {
                    _oldItem.IsVisible = false;
                    _oldItem.FontAwsomeName = CollapseDown;
                    UpDateProducts(_oldItem);

                }
                Unit.IsVisible = true;
                Unit.FontAwsomeName = CollapseUp;
                UpDateProducts(Unit);
            }
            _oldItem = Unit;
        }

        private void UpDateProducts(Unit Unit)
        {

            var Index = Units.IndexOf(Unit);
            Units.Remove(Unit);
            Units.Insert(Index, Unit);

        }

        public class Unit : INotifyPropertyChanged
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
            public string WOId { get; set; }
            public string Date { get; set; }
            public double Progress { get; set; }

            public bool IsVisible { get; set; }
            public string FontAwsomeName { get; set; }

            public Xamarin.Forms.Color CellColor { get; set; }
        }
    }
}