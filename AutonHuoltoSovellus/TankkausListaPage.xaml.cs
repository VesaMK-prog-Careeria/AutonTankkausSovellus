using AutonHuoltoSovellus.Models;
using AutonHuoltoSovellus.Services;

namespace AutonHuoltoSovellus;

public partial class TankkausListaPage : ContentPage
{
	public TankkausListaPage()
	{
		InitializeComponent();
		LataaTankkaukset();
	}

    private async void LataaTankkaukset()
    {
        using (var db = new TankkausDb())
        {
            var tankkaukset = await db.GetTankkauksetAsync();
            TankkausList.ItemsSource = tankkaukset.OrderByDescending(t => t.Aika).ToList();
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var tankkaus = button?.CommandParameter as Tankkaus;

        if (tankkaus != null)
        {
            bool confirm = await DisplayAlert("Vahvista poisto", $"Haluatko varmasti poistaa tankkauksen {tankkaus.Aika:d}?", "Kyll‰", "Peruuta");
            if (confirm)
            {
                using (var db = new TankkausDb())
                {
                    await db.PoistaTankkausAsync(tankkaus);
                }
                await DisplayAlert("Poistettu", "Tankkaus poistettiin onnistuneesti.", "OK");
                LataaTankkaukset(); // P‰ivitet‰‰n lista
            }
        }
    }
}