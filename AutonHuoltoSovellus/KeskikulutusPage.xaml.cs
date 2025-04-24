using AutonHuoltoSovellus.Models;
using AutonHuoltoSovellus.Services;

namespace AutonHuoltoSovellus;

public partial class KeskikulutusPage : ContentPage
{
    public KeskikulutusPage()
    {
        InitializeComponent();
        LaskeKeskikulutus();
    }
    private async void LaskeKeskikulutus()
    {
        using (var db = new TankkausDb())
        {
            var tankkaukset = await db.GetTankkauksetAsync();

            // Ryhmitellään kuukausittain
            var kulutukset = tankkaukset
            .OrderBy(t => t.Aika)
            .GroupBy(t => new { t.Aika.Year, t.Aika.Month })
            .Select(g =>
            {
                // Jos ryhmässä on vain yksi tankkaus, ei voida laskea kulutusta
                var lista = g.ToList();
                if (lista.Count < 2)
                {
                    return new
                    {
                        Kuukausi = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Kulutus = "Ei tarpeeksi tietoa"
                    };
                }

                double yhteisLitrat = 0;
                double ajettuKm = 0;

                // Lasketaan ajettu matka ja kulutus
                for (int i = 1; i < lista.Count; i++)
                {
                    double matka = lista[i].Kilometrit - lista[i - 1].Kilometrit;
                    if (matka > 0)
                    {
                        yhteisLitrat += lista[i].Litrat;
                        ajettuKm += matka;
                    }
                }

                double kulutus = ajettuKm > 0 ? (yhteisLitrat * 100) / ajettuKm : 0;

                return new
                {
                    Kuukausi = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Kulutus = kulutus > 0 ? $"{kulutus:F2} l/100 km" : "0 l/100 km"
                };
            })
            .ToList();

            ConsumptionList.ItemsSource = kulutukset;
        }
    }
}