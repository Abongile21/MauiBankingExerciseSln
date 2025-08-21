using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class CustomerView : ContentPage
{
    private readonly CustomerViewModel _viewModel;

    public CustomerView()
    {
        InitializeComponent();
        _viewModel = new CustomerViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
