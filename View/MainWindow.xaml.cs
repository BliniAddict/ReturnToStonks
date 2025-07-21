using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ReturnToStonks
{
    public partial class MainWindow : Window, IView
    {
        private MainViewModel _viewModel;
        public readonly MessageService _messageService;
        public MainWindow()
        {
            InitializeComponent();

            _messageService = new MessageService();
            _viewModel = new MainViewModel(this, new Model(), _messageService);
            _messageService.RegisterView(this);
            DataContext = _viewModel;
        }

        private async void TransactionViews_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListView list && list.View is GridView gridView)
            {
                //make "purpose"-column width="*" since writing it in xaml is not accepted
                var semiLastColumn = gridView.Columns[gridView.Columns.Count - 2];

                double availableWidth = list.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                foreach (var column in gridView.Columns)
                {
                    if (column != semiLastColumn)
                        availableWidth -= column.ActualWidth;
                }
                semiLastColumn.Width = availableWidth;

                if (list.Items.Count > 0)
                {
                    //scroll to today
                    string todayDate = DateTime.Today.ToString("d", new CultureInfo("en-US"));
                    var todayGroup = list.Items.Groups
                      .OfType<CollectionViewGroup>()
                      .FirstOrDefault(g => g.Name is string date && date == todayDate);

                    if (todayGroup != null)
                    {
                        list.SelectedItem = todayGroup.Items[0];
                        list.ScrollIntoView(todayGroup.Items[0]);
                        list.Focus();
                    }
                }
            }
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView list)
            {
                if (list.SelectedItem is Transaction transaction)
                    _viewModel.OpenTransactionWindow(transaction);

                else if (list.SelectedItem is Debt debt)
                    _viewModel.OpenDebtWindow(debt);
            }
        }


        //private void InitGridView()
        //{
        //    if (sender is ListView list && list.View is GridView gridView)
        //    {
        //        //make "purpose"-column width="*" since writing it in xaml is not accepted
        //        var semiLastColumn = gridView.Columns[gridView.Columns.Count - 2];

        //        double availableWidth = list.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        //        foreach (var column in gridView.Columns)
        //        {
        //            if (column != semiLastColumn)
        //                availableWidth -= column.ActualWidth;
        //        }
        //        semiLastColumn.Width = availableWidth;
        //    }
        //}
    }
}