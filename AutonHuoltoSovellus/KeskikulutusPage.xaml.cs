using AutonHuoltoSovellus.Models;
using AutonHuoltoSovellus.Services;

namespace AutonHuoltoSovellus;

// Tässä tiedostossa on KeskikulutusPage-luokka, joka vastaa keskikulutuksen laskemisesta ja näyttämisestä.
public partial class KeskikulutusPage : ContentPage
{
    public KeskikulutusPage()
    {
        InitializeComponent(); // Lataa XAML-näkymän
        LaskeKeskikulutus(); // Käynnistää kulutuslaskennan automaattisesti
    }

    // Tämä metodi laskee keskikulutuksen kk
    private async void LaskeKeskikulutus() 
    {
        using (var db = new TankkausDb()) // Avataan tietokantayhteys
        {
            var tankkaukset = await db.GetTankkauksetAsync(); // Haetaan kaikki tankkaukset tietokannasta

            // Ryhmitellään tankkaukset vuoden ja kuukauden mukaan
            var kulutukset = tankkaukset
            .OrderBy(t => t.Aika) // Järjestetään tankkaukset aikajärjestykseen
            .GroupBy(t => new { t.Aika.Year, t.Aika.Month }) // Ryhmitellään tankkaukset vuoden ja kuukauden mukaan
            .Select(g =>
            {
                // Jos kuukaudessa on vain yksi tankkaus, ei voida laskea kulutusta
                var lista = g.ToList();
                if (lista.Count < 2)
                {
                    return new
                    {
                        Kuukausi = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Kulutus = "Ei tarpeeksi tietoa"
                    };
                }

                double yhteisLitrat = 0; // Yhteensä tankatut litrat
                double ajettuKm = 0; // Ajettu matka kilometreinä

                // Lasketaan ajettu matka ja kulutus
                for (int i = 1; i < lista.Count; i++)
                {
                    double matka = lista[i].Kilometrit - lista[i - 1].Kilometrit;
                    if (matka > 0) // Ei lasketa neg kilometrejä
                    {
                        yhteisLitrat += lista[i].Litrat; // Lasketaan aina nykyisestä tankkauksesta
                        ajettuKm += matka;
                    }
                }

                // Varsinainen kulutuksen laskenta vain jos ajettu matka on suurempi kuin 0
                double kulutus = ajettuKm > 0 ? (yhteisLitrat * 100) / ajettuKm : 0;

                // Palautetaan kuukauden ja kulutuksen yhteenveto
                return new
                {
                    Kuukausi = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Kulutus = kulutus > 0 ? $"{kulutus:F2} l/100 km" : "0 l/100 km"
                };
            })
            .ToList();

            // Asetetaan laskettu lista käyttöliittymän CollectionViewiin
            ConsumptionList.ItemsSource = kulutukset;
        }
    }
}