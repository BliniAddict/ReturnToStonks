using System.Windows;
using System.Windows.Controls;

namespace ReturnToStonks
{
    public partial class DebtWindow : Window, IView
    {
        private DebtViewModel _viewModel;
        public readonly MessageService _messageService;

        public DebtWindow(IModel model, MessageService messageService, Debt debt)
        {
            InitializeComponent();

            _messageService = messageService;
            _viewModel = new DebtViewModel(this, model, _messageService, debt);
            _messageService.RegisterView(this);
            DataContext = _viewModel;
        }
        public void OpenCategoryPopup() => NewCategoryPopup.IsOpen = true;
        public void CloseCategoryPopup()
        {
            NewCategoryPopup.IsOpen = false;
            _viewModel.GetCategories();
        }

        public void OpenPersonPopup() => NewPersonPopup.IsOpen = true;
        public void ClosePersonPopup()
        {
            NewPersonPopup.IsOpen = false;
            _viewModel.GetPersons();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Name == "CancelNewCategory")
                    CloseCategoryPopup();
                else if (btn.Name == "CancelNewPerson")
                    ClosePersonPopup();
                else
                    ((IView)this).CloseWindow();
            }
        }

    }
}
