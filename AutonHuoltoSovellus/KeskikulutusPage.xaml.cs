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
            var kulutusPerKuukausi = tankkaukset
                .GroupBy(t => new { t.Aika.Year, t.Aika.Month })
                .Select(g =>
                {
                    var list = g.OrderBy(t => t.Aika).ToList();

                    if (list.Count < 2)
                    {
                        // Tarvitaan vähintään 2 tankkausta, jotta voidaan laskea ajettu matka
                        return new
                        {
                            Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                            Consumption = "Ei tarpeeksi tietoa"
                        };
                    }

                    //double yhteisLitrat = list.Skip(1).Sum(t => t.Litrat); // Skipataan ensimmäinen tankkaus
                    //double ajettuMatka = list.Last().Kilometrit - list.First().Kilometrit;

                    double yhteisLitrat = 0;
                    double ajettuMatka = 0;

                    // Lasketaan kulutus vertaamalla jokaista tankkausta edelliseen
                    for (int i = 1; i < list.Count; i++)
                    {
                        double matka = list[i].Kilometrit - list[i - 1].Kilometrit;
                        if (matka > 0)
                        {
                            yhteisLitrat += list[i].Litrat;
                            ajettuMatka += matka;
                        }
                    }

                    // Lasketaan keskikulutus
                    double kulutus = ajettuMatka > 0 ? (yhteisLitrat * 100) / ajettuMatka : 0;

                    return new
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Consumption = kulutus > 0 ? $"{kulutus:F2} l/100km" : "0 l/100km"
                    };
                })
                .ToList();

            // Näytetään laskettu kulutus listassa
            ConsumptionList.ItemsSource = kulutusPerKuukausi;
        }
    }
}