using System;
using System.Linq;
using System.Text;

namespace ObliczanieWzoru
{
    internal class Program
    {
        private static int licznik = 1;
        private static char[] cyfry = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
        private static char[] dostepneZnaki = new char[] { '+', '-', '/', '*', '^', '(', ')', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
        private static double[] pierwiastkiNieparzyste = new double[100];


        private static void UzupelnijPierwiastki()
        {
            for (int i = 1; i <= pierwiastkiNieparzyste.Length; i++)
            {
                pierwiastkiNieparzyste[i - 1] = Math.Round(1d / (2d * i + 1d), 8);
            }
        }

        private static string WyliczNawiasy(string wzor)
        {
            if (!wzor.Contains('('))
            {
                licznik--;
                return Policz(wzor);
            }
            else
                licznik++;

            int licznikNawiasow = 0;

            bool czyTworzycNawias = false;

            string nawias = string.Empty;
            string wynik = string.Empty;

            for (int i = 0; i < wzor.Length; i++)
            {
                bool tmp = false;
                bool tmp2 = false;
                if (wzor[i] == '(')
                {
                    licznikNawiasow++;
                    if (licznikNawiasow == 1)
                    {
                        tmp = true;
                        czyTworzycNawias = true;
                    }
                    else if (licznikNawiasow == 0)
                        czyTworzycNawias = false;
                }
                else if (wzor[i] == ')')
                {
                    licznikNawiasow--;
                    if (licznikNawiasow == 0)
                    {
                        tmp2 = true;
                        wynik += WyliczNawiasy(nawias);
                        nawias = string.Empty;
                        czyTworzycNawias = false;
                    }
                }
                else
                    tmp = false;

                if (czyTworzycNawias && !tmp)
                    nawias += wzor[i];
                else if (!czyTworzycNawias && !tmp2)
                    wynik += wzor[i];
            }

            if (licznik >= 0)
                return WyliczNawiasy(wynik);
            else
                return wynik;
        }

        private static void ZnajdzLiczby(string wzor, char znak, out double lewaLiczba, out double prawaLiczba, out int lewyIndex, out int prawyIndex)
        {
            StringBuilder lewa = new StringBuilder();
            StringBuilder prawa = new StringBuilder();

            lewyIndex = 0;
            prawyIndex = 0;
            int licznik = 0;
            int licznikMinusow = 0;
            int licznikKropek = 0;
            int znakIndex = wzor.LastIndexOf(znak);

            do
            {
                licznik++;
                if ((znakIndex - licznik) >= 0 && (cyfry.Contains(wzor[znakIndex - licznik]) || wzor[znakIndex - licznik] == '-'))
                {
                    if (wzor[znakIndex - licznik] == '.')
                        licznikKropek++;
                    else if (wzor[znakIndex - licznik] == '-')
                        licznikMinusow++;

                    if (licznikKropek <= 1)
                    {
                        lewa.Append(wzor[znakIndex - licznik]);
                        lewyIndex = znakIndex - licznik;
                    }

                    if (licznikMinusow == 1)
                        break;
                }
                else
                    break;

            } while (true);

            licznikMinusow = 0;
            licznik = 0;

            do
            {
                licznik++;
                if ((znakIndex + licznik) < wzor.Length && (cyfry.Contains(wzor[znakIndex + licznik]) || (wzor[znakIndex + 1] == '-' && licznikMinusow < 1)))
                {
                    if (wzor[znakIndex + licznik] == '-')
                        licznikMinusow++;

                    if (licznikMinusow <= 1)
                    {
                        prawa.Append(wzor[znakIndex + licznik]);
                        prawyIndex = znakIndex + licznik;
                    }
                }
                else
                    break;
            } while (true);

            char[] lewaCharArr = Convert.ToString(lewa).ToCharArray();
            Array.Reverse(lewaCharArr);
            string lewaString = new string(lewaCharArr);

            if (lewaString == string.Empty)
                lewaLiczba = 0;
            else
                lewaLiczba = Convert.ToDouble(lewaString);

            if (Convert.ToString(prawa) == string.Empty)
                prawaLiczba = 0;
            else
                prawaLiczba = Convert.ToDouble(Convert.ToString(prawa));
        }

        private static string Policz(string wzor)
        {
            if (wzor.Contains("ERR"))
                return wzor;

            string wynik = string.Empty;

            if (wzor.Contains('^'))
            {
                ZnajdzLiczby(wzor, '^', out double lewa, out double prawa, out int lewyIndex, out int prawyIndex);

                double wynikDzialania = 0;

                if (lewa < 0)
                {
                    if (prawa < 1 && prawa > 0)
                    {
                        if (pierwiastkiNieparzyste.Contains(Math.Round(prawa, 8)))
                            return Policz("ERR - Pierwiastek z liczby ujemnej");
                    }
                    wynikDzialania = Math.Pow(lewa, prawa);
                }

                wynikDzialania = Math.Pow(lewa, prawa);

                string lewaCzesc = wzor.Remove(lewyIndex);
                string prawaCzesc = wzor.Remove(0, prawyIndex + 1);

                wynik += lewaCzesc + wynikDzialania.ToString("F10") + prawaCzesc;

                return Policz(wynik);
            }

            if (wzor.Contains('*'))
            {
                ZnajdzLiczby(wzor, '*', out double lewa, out double prawa, out int lewyIndex, out int prawyIndex);
                double wynikDzialania = lewa * prawa;
                string stringWynikDzialania = string.Empty;

                if ((lewa < 0 && prawa < 0) || wynikDzialania == 0)
                    stringWynikDzialania += '+';

                stringWynikDzialania += Convert.ToString(wynikDzialania);


                string lewaCzesc = wzor.Remove(lewyIndex);
                string prawaCzesc = wzor.Remove(0, prawyIndex + 1);
                wynik += lewaCzesc + stringWynikDzialania + prawaCzesc;

                return Policz(wynik);
            }

            if (wzor.Contains('/'))
            {
                ZnajdzLiczby(wzor, '/', out double lewa, out double prawa, out int lewyIndex, out int prawyIndex);

                double wynikDzialania;

                if (prawa != 0)
                    wynikDzialania = lewa / prawa;
                else
                    return Policz("ERR - Dzielenie przez 0");

                string lewaCzesc = wzor.Remove(lewyIndex);
                string prawaCzesc = wzor.Remove(0, prawyIndex + 1);
                wynik += lewaCzesc + Convert.ToString(wynikDzialania) + prawaCzesc;

                return Policz(wynik);
            }

            if (wzor.Contains('+'))
            {
                ZnajdzLiczby(wzor, '+', out double lewa, out double prawa, out int lewyIndex, out int prawyIndex);
                double wynikDzialania = lewa + prawa;

                string lewaCzesc = wzor.Remove(lewyIndex);
                string prawaCzesc = wzor.Remove(0, prawyIndex + 1);
                wynik += lewaCzesc + Convert.ToString(wynikDzialania) + prawaCzesc;

                return Policz(wynik);
            }

            if (wzor.Contains('-') && wzor.LastIndexOf('-') != 0)
            {
                ZnajdzLiczby(wzor, '-', out double lewa, out double prawa, out int lewyIndex, out int prawyIndex);
                double wynikDzialania = lewa - prawa;

                string lewaCzesc = wzor.Remove(lewyIndex);
                string prawaCzesc = wzor.Remove(0, prawyIndex + 1);
                wynik += lewaCzesc + Convert.ToString(wynikDzialania) + prawaCzesc;

                return Policz(wynik);
            }

            return wzor;
        }

        private static void Main(string[] args)
        {
            UzupelnijPierwiastki();

            string enter = Environment.NewLine;
            string wynik = string.Empty;
            const string space = " ";

            Console.WriteLine("-=-=-= Konsolowy kalkulator działań =-=-=-");
            Console.WriteLine(enter + "Działanie może zawierac:");
            Console.WriteLine("\t> Liczby (również ułamki dziesiętne)");
            Console.WriteLine("\t> Znaki działań: +, -, /, *, ^");
            Console.WriteLine("\t> Nawiasy okrągłe: (, )");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\t> ! Przed nawiasem należy umieścić jeden z znaków działań !");
            Console.ResetColor();
            Console.WriteLine("\t> Działanie może również zawierać spacje między znakami.");
            Console.WriteLine("\t> Wpisz Q aby wyjść.");


            do
            {
                Console.WriteLine(enter + "Podaj działanie:");
                string wzorString = Console.ReadLine();
                wzorString = wzorString.Replace(space, string.Empty).ToLower();

                if (wzorString == "q")
                    break;

                int otwartyNawias = 0;
                int zamknietyNawias = 0;
                for (int i = 0; i < wzorString.Length; i++)
                {
                    if (wzorString[i] == '(')
                        otwartyNawias++;

                    if (wzorString[i] == ')')
                        zamknietyNawias++;
                }

                bool blednyZnak = false;
                foreach (char znak in wzorString)
                {
                    if (!dostepneZnaki.Contains(znak))
                    {
                        blednyZnak = true;
                        break;
                    }
                }

                if (!blednyZnak)
                {
                    if (otwartyNawias == zamknietyNawias)
                    {
                        if (wzorString == string.Empty)
                            wynik = "0";
                        else
                            wynik = Policz(WyliczNawiasy(wzorString));

                        if (wynik.Contains("ERR"))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(enter + wynik);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine(enter + "Wynik = " + wynik);
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(enter + "Błędna liczba nawiasów!");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(enter + "Działanie zawiera błędne znaki!");
                    Console.ResetColor();
                }

                licznik = 0;

            } while (true);
        }
    }
}