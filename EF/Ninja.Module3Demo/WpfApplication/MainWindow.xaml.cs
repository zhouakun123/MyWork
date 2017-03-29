using NinjaDomain.Classes;
using NinjiaDomain.DataModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ConnectedRepository _repo = new ConnectedRepository();
        private Ninja _currentNinja;
        private bool _isLoading;
        private bool _isNinjaListChanging;
        private ObjectDataProvider _ninjaViewSource;
        private ObservableCollection<NinjaEquipment> _observableEquipment = new ObservableCollection<NinjaEquipment>();

        public bool ShouldRefresh
        {
            get
            {
                var continueProcess = true;
                if (_currentNinja != null)
                {
                    if (_currentNinja.IsDirty)
                    {
                        switch (MessageBox.Show("Save current ninja?", "Ninja Entry", MessageBoxButton.YesNoCancel))
                        {
                            case MessageBoxResult.Cancel:
                                continueProcess = false;
                                break;
                            case MessageBoxResult.Yes:
                                _repo.Save();
                                break;
                            case MessageBoxResult.No:
                                break;
                        }
                    }
                }
                return continueProcess;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoading = true;
            ninjaListBox.ItemsSource = _repo.NinjasInMemory();
            SortNinjaList();
            clanComboBox.ItemsSource = _repo.GetClanList();
            _ninjaViewSource = (ObjectDataProvider)FindResource("ninjaViewSource");
            ninjaListBox.SelectedIndex = 0;
            _isLoading = false;
        }

        private void SortNinjaList()
        {
            var dataView = CollectionViewSource.GetDefaultView(ninjaListBox.ItemsSource);
            dataView.SortDescriptions.Clear();
            var sd = new SortDescription("Name", ListSortDirection.Ascending);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
            ninjaListBox.SelectedItem = _currentNinja;
        }

        private void ninjaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool continueProcess;
            if (_isLoading)
            {
                continueProcess = true;
            }
            else
            {
                continueProcess = ShouldRefresh;
            }
            if (!continueProcess) return;
            _currentNinja = _repo.GetNinjaWithEquipment((int)(ninjaListBox.SelectedValue));
            RefreshNinja();
            _isNinjaListChanging = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _repo.Save();
            SortNinjaList();
        }

        private void RefreshNinja()
        {
            _isNinjaListChanging = true;
            _ninjaViewSource.ObjectInstance = _currentNinja;
            _observableEquipment = new ObservableCollection<NinjaEquipment>
                (_currentNinja.EquipmentOwned);
            equipmentOwnedDataGrid.ItemsSource = _observableEquipment;
            _observableEquipment.CollectionChanged += EquimentCollectionChanged;

            clanComboBox.SelectedValue = _currentNinja.ClanId;
            _currentNinja.IsDirty = false;
            _isNinjaListChanging = false;
        }

        private void EquimentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(_isLoading||_isNinjaListChanging)
            {
                return;
            }
            if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                _repo.DeleteEquipment(e.OldItems);
                SetNinjaDirty();
            }
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                _currentNinja.EquipmentOwned.AddRange(e.NewItems.Cast<NinjaEquipment>());
                SetNinjaDirty();
            }
        }
        private void btnNewNinja_Click(object sender, RoutedEventArgs e)
        {
            if(ShouldRefresh)
            {
                _currentNinja = _repo.NewNinja();
                RefreshNinja();
            }
        }

        private void DeleteNinja(object sender, RoutedEventArgs e)
        {
            switch(MessageBox.Show("Delete?Really?", "Ninja Entry", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Yes:
                    var ninjaToDelete = _currentNinja;
                    ninjaListBox.SelectedIndex = 0;
                    _repo.DeleteCurrentNinja(ninjaToDelete);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void clanComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!_isLoading&& !_isNinjaListChanging)
            {
                _currentNinja.ClanId = (int)clanComboBox.SelectedValue;
            }
            SetNinjaDirty();
        }

        private void nameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetNinjaDirty();
        }

        private void dateOfBirthDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetNinjaDirty();
        }

        private void servedInOniwabanCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetNinjaDirty();
        }

        private void servedInOniwabancheckedBox_Checked(object sender, RoutedEventArgs e)
        {
            SetNinjaDirty();
        }
        private void SetNinjaDirty()
        {
            if (!_isLoading && !_isNinjaListChanging)
            {
                _currentNinja.IsDirty = true;
            }
        }
    }
}
