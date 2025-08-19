using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class CustomerView : ContentPage
{
	public CustomerView(int customerId)
	{
        InitializeComponent();
        BindingContext = new CustomerViewModel(customerId);
    }
}