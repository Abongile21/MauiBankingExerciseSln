using MauiBankingExercise.ViewModels;

namespace MauiBankingExercise.Views;

public partial class CustomersListView : ContentPage
{
	public CustomersListView()
	{
		InitializeComponent();
        BindingContext = new CustomersListViewModel();
    }
}