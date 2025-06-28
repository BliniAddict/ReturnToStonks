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
        private Person? _oldPerson;
        private Debt? _oldDebt;

        public DebtViewModel(IView view, IModel model, MessageService messageService, Debt? debt)
        {
            base.messageService = messageService;
            base.view = view;
            base.model = model;

            InitDebtWindow();

            ChangePersonCommand = new RelayCommand<Person>(InitPersonPopup);
            SavePersonCommand = new RelayCommand(SavePerson);
            DeletePersonCommand = new RelayCommand(DeletePerson);

            ChangeCategoryCommand = new RelayCommand<Category>(InitCategoryPopup);
            SaveCategoryCommand = new RelayCommand(SaveCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);

            SaveDebtCommand = new RelayCommand(SaveDebt);
            DeleteDebtCommand = new RelayCommand(DeleteDebt);
        }

        #region Commands
        public ICommand SavePersonCommand { get; }
        public ICommand ChangePersonCommand { get; }
        public ICommand DeletePersonCommand { get; }

        public ICommand SaveCategoryCommand { get; }
        public ICommand ChangeCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        public ICommand SaveDebtCommand { get; }
        public ICommand DeleteDebtCommand { get; }
        #endregion

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
            get
            {
                CheckIfPropertyChanged();
                return _selectedPerson;
            }
            set
            {
                _selectedPerson = value;
                OnPropertyChanged();

                if (value?.Last_Name == " ✚")
                    InitPersonPopup();
            }
        }

        private bool _isDeletePersonButtonEnabled;
        public bool IsDeletePersonButtonEnabled
        {
            get => _isDeletePersonButtonEnabled;
            set
            {
                _isDeletePersonButtonEnabled = value;
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

        #region Methods
        private void InitDebtWindow(Debt? debt = null)
        {
            if (debt == null)
                debt = new Debt(null, string.Empty, null, 0.0, DateTime.Now);
            else
            {
                _oldDebt = new Debt(debt);

                if (debt.Amount < 0)
                    debt.Amount *= -1;
                else
                    IsOwedToMe = true;
            }

            SelectedDebt = debt;
            GetPersons();
            GetCategories();
        }
        private void InitPersonPopup(Person per = null)
        {
            if (per != null) //change (not yet) selected person
            {
                _oldPerson = new Person(per);
                SelectedPerson = per;
            }
            else if (SelectedPerson?.Last_Name == " ✚") //new person
                SelectedPerson = new Person();

            view.OpenPersonPopup();
        }

        private void SaveDebt()
        {
            throw new NotImplementedException();
        }
        private void SavePerson()
        {
            string message = model.SavePerson(SelectedPerson, _oldPerson);
            messageService.ShowMessage(message);

            view.ClosePersonPopup();
            SelectedPerson = Persons[^2];
        }

        private void DeleteDebt()
        {
            throw new NotImplementedException();
        }
        private void DeletePerson()
        {
            if (HasUserConfirmed("delete", SelectedPerson))
            {
                string message = model.DeletePerson(SelectedPerson);
                messageService.ShowMessage(message);

                view.ClosePersonPopup();
            }
        }


        public void GetPersons()
        {
            Persons = new ObservableCollection<Person>();
            foreach (Person person in model.GetPersons())
                Persons.Add(person);

            OldCategory = null;
            Persons.Add(new Person("Add new person", " ✚", string.Empty, string.Empty));
        }
        public override void CheckIfPropertyChanged()
        {


            IsDeletePersonButtonEnabled = Utilities.ArePropertiesEqual(_selectedPerson, _oldPerson);
            IsDeleteCategoryButtonEnabled = Utilities.ArePropertiesEqual(_selectedCategory, OldCategory);
        }

        #endregion
    }
}