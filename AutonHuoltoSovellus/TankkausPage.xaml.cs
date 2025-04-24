using AutonHuoltoSovellus.Models;
using AutonHuoltoSovellus.Services;
using System.Diagnostics;
using CommunityToolkit.Maui.Alerts;

namespace AutonHuoltoSovellus;

public partial class TankkausPage : ContentPage
{
    public TankkausPage()
    {
        InitializeComponent();
        AsetaOletusKilometrit();
    }

    // HAetaan viimeisin tankkaus ja asetetaan sen kilometrit-kenttä oletusarvoksi
    private async void AsetaOletusKilometrit()
    {
        using var db = new TankkausDb();
        var viimeisin = await db.GetViimeisinTankkausAsync();
        if (viimeisin != null)
            KilometritEntry.Text = viimeisin.Kilometrit.ToString();
    }

    // Tallenna tankkaus tietokantaan ja palaa listaan
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var tankkaus = new Tankkaus
        {
            Aika = PaivaValinta.Date,
            Litrat = double.Parse(LitratEntry.Text),
            Kilometrit = double.Parse(KilometritEntry.Text)
        };

        using (var db = new TankkausDb())
        {
            db.Database.EnsureCreated();
            db.Tankkaukset.Add(tankkaus);
            db.SaveChanges();
        }

        // Debuggausta
        Console.WriteLine($"Tankkaus tallennettu: {tankkaus.Aika:d} {tankkaus.Litrat} l, {tankkaus.Kilometrit} km");

        // Community toolkit -kirjaston käyttöä
        var snack = Snackbar.Make("Tankkaus tallennettu onnistuneesti!", duration: TimeSpan.FromSeconds(3));
        await snack.Show();

        //await DisplayAlert("Tallennus", "Tankkaus tallennettu onnistuneesti!", "OK");

        await Navigation.PopAsync(); // Palaa listaan → se päivittää itsensä OnAppearing():ssa
    }
}