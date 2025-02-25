using AutonHuoltoSovellus.Models;
using AutonHuoltoSovellus.Services;

namespace AutonHuoltoSovellus;

public partial class TankkausPage : ContentPage
{
	public TankkausPage()
	{
		InitializeComponent();
        AsetaOletusKilometrit();
    }

    private async void AsetaOletusKilometrit()
    {
        using (var db = new TankkausDb())
        {
            var viimeisinTankkaus = await db.GetViimeisinTankkausAsync();
            if (viimeisinTankkaus != null)
            {
                KilometritEntry.Text = viimeisinTankkaus.Kilometrit.ToString();
            }
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var tankkaus = new Tankkaus
        {
            Aika = PaivaValinta.Date, //K‰ytt‰j‰n pvm valinta
            Litrat = double.Parse(LitratEntry.Text),
            Kilometrit = double.Parse(KilometritEntry.Text)
        };

        using (var db = new TankkausDb())
        {
            db.Database.EnsureCreated();
            db.Tankkaukset.Add(tankkaus);
            db.SaveChanges();
        }

        await DisplayAlert("Tallennus", "Tankkaus tallennettu onnistuneesti!", "OK");

        //P‰ivitet‰‰n kilometrit kentt‰ viimeisimm‰n tankkauksen mukaan
        AsetaOletusKilometrit();
    }
}
