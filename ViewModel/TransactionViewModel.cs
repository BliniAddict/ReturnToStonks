﻿using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ReturnToStonks
{
  public class TransactionViewModel : ViewModelBase
  {
    private readonly MessageService _messageService;
    private readonly IView _view;
    private readonly IModel _model;
    private Transaction? _oldTransaction;
    private Category? _oldCategory;

    public TransactionViewModel(IView view, IModel model, MessageService messageService, Transaction? transaction)
    {
      _view = view;
      _model = model;
      _messageService = messageService;

      InitTransactionWindow(transaction);

      SaveTransactionCommand = new RelayCommand<Window>(SaveTransaction);
      SaveCategoryCommand = new RelayCommand(SaveCategory);
      ChangeCategoryCommand = new RelayCommand<Category>(InitCategoryPopup);
      DeleteTransactionCommand = new RelayCommand<Window>(DeleteTransaction);
      DeleteCategoryCommand = new RelayCommand(DeleteCategory);
    }

    public ICommand SaveTransactionCommand { get; }
    public ICommand DeleteTransactionCommand { get; }

    public ICommand SaveCategoryCommand { get; }
    public ICommand ChangeCategoryCommand { get; }
    public ICommand DeleteCategoryCommand { get; }

    #region Properties

    private Transaction _selectedTransaction;
    public Transaction SelectedTransaction
    {
      get
      {
        Transaction? tempTransaction = _oldTransaction == null ? null : new Transaction(_oldTransaction)
        { Amount = Math.Abs(_oldTransaction.Amount) };
        IsDeleteTransactionButtonEnabled = Utilities.ArePropertiesEqual(_selectedTransaction, tempTransaction);

        return _selectedTransaction;
      }
      set
      {
        _selectedTransaction = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<Category> _categories;
    public ObservableCollection<Category> Categories
    {
      get => _categories;
      set
      {
        _categories = value;
        OnPropertyChanged();
      }
    }

    private Category? _selectedCategory;
    public Category? SelectedCategory
    {
      get
      {
        Transaction? tempTransaction = _oldTransaction == null ? null : new Transaction(_oldTransaction)
        { Amount = Math.Abs(_oldTransaction.Amount) };
        IsDeleteTransactionButtonEnabled = Utilities.ArePropertiesEqual(_selectedTransaction, tempTransaction);
        IsDeleteCategoryButtonEnabled = Utilities.ArePropertiesEqual(_selectedCategory, _oldCategory);

        return _selectedCategory;
      }
      set
      {
        _selectedCategory = value;
        OnPropertyChanged();

        if (value?.Symbol == " ✚")
          InitCategoryPopup();
      }
    }

    private bool _isIncome = false;
    public bool IsIncome
    {
      get
      {
        IsDeleteTransactionButtonEnabled = _isIncome == (_oldTransaction?.Amount < 0);
        return _isIncome;
      }
      set
      {
        _isIncome = value;
        OnPropertyChanged();
      }
    }

    private bool _isDeleteTransactionButtonEnabled;
    public bool IsDeleteTransactionButtonEnabled
    {
      get => _isDeleteTransactionButtonEnabled;
      set
      {
        _isDeleteTransactionButtonEnabled = value;
        OnPropertyChanged();
      }
    }

    private bool _isDeleteCategoryButtonEnabled;
    public bool IsDeleteCategoryButtonEnabled
    {
      get => _isDeleteCategoryButtonEnabled;
      set
      {
        _isDeleteCategoryButtonEnabled = value;
        OnPropertyChanged();
      }
    }
    #endregion

    #region Methods
    private void InitTransactionWindow(Transaction? transaction = null)
    {
      if (transaction == null)
        transaction = new Transaction(string.Empty, null, 0, DateTime.Now, false, new("month", 1));
      else
      {
        transaction.Recurrence ??= new("month", 1);
        _oldTransaction = new Transaction(transaction);

        if (transaction.Amount < 0)
          transaction.Amount *= -1;
        else
          IsIncome = true;
      }

      SelectedTransaction = transaction;
      GetCategories();
    }
    private void InitCategoryPopup(Category cat = null)
    {
      if (cat != null) //change (not yet) selected category
      {
        _oldCategory = new Category(cat.Name, cat.Symbol);
        SelectedCategory = cat;
      }
      else if (SelectedCategory?.Symbol == " ✚") //new category
        SelectedCategory = new Category(string.Empty, "❓");

      _view.OpenCategoryPopup();
    }

    private void SaveTransaction(Window window)
    {
      SelectedTransaction.Category = SelectedCategory;

      if (!IsIncome)
        SelectedTransaction.Amount *= -1;

      string message = _model.SaveTransaction(SelectedTransaction, _oldTransaction);
      _messageService.ShowMessage(message, true);
      _view.CloseWindow(window);
    }
    private void SaveCategory()
    {
      string message = _model.SaveCategory(SelectedCategory, _oldCategory);
      _messageService.ShowMessage(message);

      _view.CloseCategoryPopup();
      SelectedCategory = Categories[^2];
    }

    public void GetCategories()
    {
      Categories = new ObservableCollection<Category>();
      foreach (Category category in _model.GetCategories())
        Categories.Add(category);

      _oldCategory = null;
      if (SelectedTransaction.Category != null)
        SelectedCategory = Categories.FirstOrDefault(name => SelectedTransaction.Category.Name == name.Name);

      Categories.Add(new Category("Add new category", " ✚"));
    }

    private void DeleteTransaction(Window window)
    {
      string? additionaMessage = SelectedTransaction.IsRecurring ? "Recurring Transactions will also be affected." : null;
      if (HasUserConfirmed("delete", SelectedTransaction, additionaMessage))
      {
        if (!IsIncome)
          SelectedTransaction.Amount = SelectedTransaction.Amount * -1;

        if (!SelectedTransaction.IsRecurring)
          SelectedTransaction.Recurrence = null;

        string message = _model.DeleteTransaction(SelectedTransaction);
        _messageService.ShowMessage(message, true);

        _view.CloseWindow(window);
      }
    }
    private void DeleteCategory()
    {
      if (HasUserConfirmed("delete", SelectedCategory))
      {
        string message = _model.DeleteCategory(SelectedCategory);
        _messageService.ShowMessage(message);

        _view.CloseCategoryPopup();
      }
    }
    #endregion
  }
}