using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class CustomerView : ContentPage
{
	public CustomerView()
	{
        InitializeComponent();
        BindingContext = new CustomerViewModel();
    }
}