using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReturnToStonks
{
    class DebtViewModel : ViewModelBase
    {
        private Debt? _oldDebt;

        public DebtViewModel(IView view, IModel model, MessageService messageService, Debt? debt)
        {
            base.messageService = messageService;
            base.view = view;
            base.model = model;

            InitDebtWindow();

            ChangeCategoryCommand = new RelayCommand<Category>(InitCategoryPopup);
            SaveCategoryCommand = new RelayCommand(SaveCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);

            SaveDebtCommand = new RelayCommand(SaveDebt);
            DeleteDebtCommand = new RelayCommand(DeleteDebt);
        }

        public ICommand SaveCategoryCommand { get; }
        public ICommand ChangeCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        public ICommand SaveDebtCommand { get; }
        public ICommand DeleteDebtCommand { get; }

        #region Properties
        private Debt _selectedDebt;
        public Debt SelectedDebt
        {
            get => _selectedDebt;
            set
            {
                _selectedDebt = value;
                OnPropertyChanged();
            }
        }



        public ObservableCollection<Person> _persons;
        public ObservableCollection<Person> Persons
        {
            get => _persons;
            set
            {
                _persons = value;
                OnPropertyChanged();
            }
        }

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged();
            }
        }

        private bool _isOwedToMe;
        public bool IsOwedToMe
        {
            get => _isOwedToMe;
            set
            {
                _isOwedToMe = value;
                OnPropertyChanged();
            }
        }

        private bool _isDeleteDebtButtonEnabled;
        public bool IsDeleteDebtButtonEnabled
        {
            get => _isDeleteDebtButtonEnabled;
            set
            {
                _isDeleteDebtButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private void InitDebtWindow(Debt? transaction = null)
        {
            if (transaction == null)
                transaction = new Debt(null, string.Empty, null, 0.0, DateTime.Now);
            else
            {
                _oldDebt = new Debt(transaction);

                if (transaction.Amount < 0)
                    transaction.Amount *= -1;
                else
                    IsOwedToMe = true;
            }

            SelectedDebt = transaction;
            GetPersons();
            GetCategories();
        }

        private void SaveDebt()
        {
            throw new NotImplementedException();
        }

        private void DeleteDebt()
        {
            throw new NotImplementedException();
        }


        public override void CheckIfCategoryChanged()
        {
            IsDeleteCategoryButtonEnabled = Utilities.ArePropertiesEqual(_selectedCategory, OldCategory);
        }

        internal void GetPersons()
        {

            Persons = new ObservableCollection<Person>();
            //foreach (Person person in model.GetPersons())
            //    Persons.Add(person);

            OldCategory = null;
            Persons.Add(new Person("Add new person", " ✚", string.Empty, string.Empty));
        }
    }
}
