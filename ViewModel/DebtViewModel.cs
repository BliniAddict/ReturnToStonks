using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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

            InitDebtWindow(debt);

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
            get
            {
                Debt? tempTransaction = _oldDebt == null ? null : new Debt(_oldDebt)
                { Amount = Math.Abs(_oldDebt.Amount) };
                IsDeleteDebtButtonEnabled = Utilities.ArePropertiesEqual(_selectedDebt, tempTransaction);

                return _selectedDebt;
            }
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

                if (value?.Name == "✚ Add new person")
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
                debt = new Debt(string.Empty, null, null, 0.0);
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
            else if (SelectedPerson?.Name == "✚ Add new person") //new person
                SelectedPerson = new Person();

            view.OpenPersonPopup();
        }

        private void SaveDebt()
        {
            SelectedDebt.Person = SelectedPerson;
            SelectedDebt.Category = SelectedCategory;

            if (!IsOwedToMe)
                SelectedDebt.Amount *= -1;

            string message = model.SaveDebt(SelectedDebt, _oldDebt);
            messageService.ShowMessage(message, true);
            view.CloseWindow();
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
            if (messageService.HasUserConfirmed("delete", SelectedDebt))
            {
                if (!IsOwedToMe)
                    SelectedDebt.Amount = SelectedDebt.Amount * -1;

                string message = model.DeleteDebt(SelectedDebt);
                messageService.ShowMessage(message, true);

                view.CloseWindow();
            }
        }
        private void DeletePerson()
        {
            if (messageService.HasUserConfirmed("delete", SelectedPerson))
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
            Persons.Add(new Person("✚ Add new person", string.Empty, string.Empty));

            if (SelectedDebt.Person != null)
                SelectedPerson = Persons.FirstOrDefault(name => SelectedDebt.Person.Name == name.Name);
        }

        public override void GetCategories()
        {
            base.GetCategories();
            if (SelectedDebt.Category != null)
                SelectedCategory = Categories.FirstOrDefault(name => SelectedDebt.Category.Name == name.Name);
        }

        public override void CheckIfPropertyChanged()
        {
            Debt? tempDebt = _oldDebt == null ? null : new Debt(_oldDebt)
            {
                Amount = Math.Abs(_oldDebt.Amount),
                Person = _selectedPerson,
                Category = _selectedCategory
            };

            IsDeleteDebtButtonEnabled = Utilities.ArePropertiesEqual(_selectedDebt, tempDebt);
            IsDeletePersonButtonEnabled = Utilities.ArePropertiesEqual(_selectedPerson, _oldPerson);
            IsDeleteCategoryButtonEnabled = Utilities.ArePropertiesEqual(_selectedCategory, OldCategory);
        }
        #endregion
    }
}