using AutonHuoltoSovellus.Models;
using AutonHuoltoSovellus.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AutonHuoltoSovellus;

// Tässä tiedostossa on TankkausListaPage-luokka, joka vastaa tankkauslistan näyttämisestä ja hallinnasta.
public partial class TankkausListaPage : ContentPage
{
    public ObservableCollection<Tankkaus> Tankkaukset { get; set; } = new();

    public TankkausListaPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LataaTankkaukset();
        // Debuggausta
        Debug.WriteLine($"Tietokannan polku: {Path.Combine(FileSystem.AppDataDirectory, "TankkausData.db")}");
    }

    private async Task LataaTankkaukset()
    {
        using var db = new TankkausDb();
        var lista = await db.GetTankkauksetAsync();
        // Debuggausta
        Debug.WriteLine($"Ladataan {lista.Count} tankkausta listalle");

        Tankkaukset.Clear();
        foreach (var t in lista.OrderByDescending(t => t.Aika))
            Tankkaukset.Add(t);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.CommandParameter is Tankkaus tankkaus)
        {
            bool confirm = await DisplayAlert("Vahvista poisto", $"Poistetaanko tankkaus {tankkaus.Aika:d}?", "Kyllä", "Peruuta");
            if (confirm)
            {
                using var db = new TankkausDb();
                await db.PoistaTankkausAsync(tankkaus);

                await DisplayAlert("Poistettu", "Tankkaus poistettiin.", "OK");
                await LataaTankkaukset();
            }
        }
    }

    private async void OnPoistaKaikkiClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Vahvista", "Poistetaanko KAIKKI tankkaukset?", "Kyllä", "Peruuta");
        if (confirm)
        {
            using var db = new TankkausDb();
            var kaikki = await db.GetTankkauksetAsync();
            db.Tankkaukset.RemoveRange(kaikki);
            await db.SaveChangesAsync();

            await DisplayAlert("Poistettu", "Kaikki tankkaukset poistettu.", "OK");
            await LataaTankkaukset();
        }
    }
}