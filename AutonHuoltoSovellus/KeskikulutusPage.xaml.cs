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

            // Ryhmitell‰‰n kuukausittain
            var kulutusPerKuukausi = tankkaukset
                .GroupBy(t => new { t.Aika.Year, t.Aika.Month })
                .Select(g =>
                {
                    var list = g.OrderBy(t => t.Aika).ToList();

                    if (list.Count < 2)
                    {
                        // Tarvitaan v‰hint‰‰n 2 tankkausta, jotta voidaan laskea ajettu matka
                        return new
                        {
                            Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                            Consumption = "Ei tarpeeksi tietoa"
                        };
                    }

                    double yhteisLitrat = list.Skip(1).Sum(t => t.Litrat); // Skipataan ensimm‰inen tankkaus
                    double ajettuMatka = list.Last().Kilometrit - list.First().Kilometrit;

                    // Lasketaan keskikulutus
                    double kulutus = ajettuMatka > 0 ? (yhteisLitrat * 100) / ajettuMatka : 0;

                    return new
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Consumption = kulutus > 0 ? $"{kulutus:F2} l/100km" : "0 l/100km"
                    };
                })
                .ToList();

            // N‰ytet‰‰n laskettu kulutus listassa
            ConsumptionList.ItemsSource = kulutusPerKuukausi;
        }
    }
}