using System;
using static System.Math;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;


namespace WiPW_SK_WChudoba185060
{
    class Program
    {
        public static int W(int n)      //liczba wierzchołków
        {
            int w = 3;

            if (n == 1)
                w = 3;

            if (n >= 2)
            {
                for (int i = 2; i <= n; i++)
                    w += i + 1;
            }

            return w;
        }

        public static int K(int n)      //liczba krawędzi
        {
            int k = 3;

            if (n == 1)
                k = 3;

            if (n >= 2)
            {
                for (int i = 2; i <= n; i++)
                    k += 3*i;
            }

            return k;
        }

        public static int[,] MacierzSasiedztwa(int n)
        {
            int[,] ms = new int[W(n), W(n)];

            int[] indeksyZajete = new int[W(n)];     //tablica pomocnicza dla łatwiejszego poradzenia sobie z wierzchołkami nieznajdującymi się na krańcowych krawędziach grafu
            indeksyZajete[0] = -1;
            for (int i = 1; i < indeksyZajete.Length; i++)
                indeksyZajete[i] = i;                //wartość w tablicy pomocniczej dla wziętych już pod uwagę wierzchołków będzie wynosić -1  

            for (int w = W(n) - 1; w >= 0; w--)         //wiersze macierzy
            {
                for (int k = W(n) - 1; k >= 0; k--)     //kolumny macierzy
                {
                    ms[w, k] = 0;                       //domyślna wartość w macierzy wynosi 0

                    int licznikPrzejscWl = 0;
                    int licznikPrzejscWp = 0;

                    if (w == W(n) - 1)  //prawy-dolny wierzchołek figury
                    {
                        ms[w, w - n - 1] = ms[w, w - 1] = 1;
                        indeksyZajete[W(n) - 1] = -1;
                    }

                    for (int licznikWd = W(n) - 2; licznikWd >= W(n) - n; licznikWd--) //wierzchołki niekrańcowe na dolnym boku
                    {
                        if (k == licznikWd + 1 || k == licznikWd - n || k == licznikWd - n - 1 || k == licznikWd - 1)          //jeśli pętla pada na sąsiedni wierzchołek, element przyjmuje wartość 1
                            ms[licznikWd, k] = 1;
                        indeksyZajete[licznikWd] = -1;
                    }

                    if (w == W(n) - 1 - n) //lewy-dolny wierzchołek figury
                    {
                        ms[w, w - n] = ms[w, w + 1] = 1;
                        indeksyZajete[W(n) - 1 - n] = -1;
                    }

                    for (int licznikWl = W(n) - 1 - 2 * n; licznikWl >= 1; licznikWl += -n + licznikPrzejscWl) //wierzchołki niekrańcowe na lewym boku
                    {
                        if (k == licznikWl - n + 1 + licznikPrzejscWl || k == licznikWl + 1 || k == licznikWl + n - licznikPrzejscWl || k == licznikWl + n - licznikPrzejscWl + 1)
                            ms[licznikWl, k] = 1;
                        licznikPrzejscWl++;
                        indeksyZajete[licznikWl] = -1;
                    }

                    for (int licznikWp = W(n) - 2 - n; licznikWp >= 2; licznikWp += -n - 1 + licznikPrzejscWp) //wierzchołki niekrańcowe na prawym boku
                    {
                        if (k == licznikWp - n + licznikPrzejscWp || k == licznikWp - 1 || k == licznikWp + n - licznikPrzejscWp || k == licznikWp + n - licznikPrzejscWp + 1)
                            ms[licznikWp, k] = 1;
                        licznikPrzejscWp++;
                        indeksyZajete[licznikWp] = -1;
                    }

                    for (int licznikSrodek = 1; licznikSrodek < indeksyZajete.Length; licznikSrodek++) //wierzchołki w środku figury
                    {
                        if (indeksyZajete[licznikSrodek] != -1)
                        {
                            if (k == licznikSrodek - n || k == licznikSrodek - n + 1 || k == licznikSrodek - 1 || k == licznikSrodek + 1 || k == licznikSrodek + n || k == licznikSrodek + n + 1)
                                ms[licznikSrodek, k] = 1;
                        }
                    }
                }
            }
            ms[0, 1] = ms[0, 2] = 1; // niezależnie od n, w pierwszym wierszu wartości są zawsze takie same (0, 1, 1, 0, 0, 0,...)

            return ms;
        }

        public static int[,] Diagonal(int[,] macierz, int n)
        {
            int[,] diagonalMacierz = new int[W(n), W(n)];

            int[] indeksyZajete = new int[W(n)];
            for (int i = 0; i < indeksyZajete.Length; i++)
                indeksyZajete[i] = i;

            for (int i = 0; i < W(n); i++)
            {
                for (int j = 0; j < W(n); j++)
                    diagonalMacierz[i, j] = macierz[i, j];
            }

            for (int w = 0; w < macierz.Length; w++)
            {
                for (int k = 0; k < macierz.Length; k++)
                {

                    if (w == 0 && k == 0)
                    {
                        diagonalMacierz[0, 0] = 2;
                        indeksyZajete[w] = -1;
                    }

                    if (w == W(n) - 1 && k == W(n) - 1)
                    {
                        diagonalMacierz[w, k] = 2;
                        indeksyZajete[w] = -1;
                    }

                    if (w == W(n) - 1 - n && k == W(n) - 1 - n)
                    {
                        diagonalMacierz[w, k] = 2;
                        indeksyZajete[w] = -1;
                    }

                    for (int licznikWd = 0; licznikWd <= n - 2; licznikWd++) 
                    {
                        if (w == W(n) - 2 - licznikWd && k == W(n) - 2 - licznikWd)
                        {
                            diagonalMacierz[w, k] = 4;
                            indeksyZajete[w] = -1;
                        }
                    }

                    for (int licznikWl = 1; licznikWl <= W(n) - 1 - 2 * n; licznikWl += licznikWl + 1) //wierzchołki niekrańcowe na lewym boku
                    {
                        if (w == licznikWl && k == licznikWl)
                        {
                            diagonalMacierz[w, k] = 4;
                            indeksyZajete[w] = -1;
                        }
                    }

                    for (int licznikWp = 2; licznikWp <= W(n) - 2 - n; licznikWp += licznikWp + 1) //wierzchołki niekrańcowe na prawym boku
                    {
                        if (w == licznikWp && k == licznikWp)
                        {
                            diagonalMacierz[w, k] = 4;
                            indeksyZajete[w] = -1;
                        }
                    }

                    for (int licznikSrodek = 0; licznikSrodek < indeksyZajete.Length; licznikSrodek++) //wierzchołki w środku figury
                    {
                        if (indeksyZajete[licznikSrodek] != -1)
                            diagonalMacierz[indeksyZajete[licznikSrodek], indeksyZajete[licznikSrodek]] = 6;
                    }
                }
            }
            return diagonalMacierz;
        }

        public static int[,] DopAlg(int[,] macierz, int n)
        {
            int[,] coFactor = new int[W(n), W(n)];

            for (int w = 0; w < W(n); w++)
            {
                for (int k = 0; k < W(n); k++)
                {
                    coFactor[w, k] = macierz[w, k];
                    if (coFactor[w, k] == 1)
                        coFactor[w, k] = -1;
                }
            }

            return coFactor;
        }

        private static int[,] StworzMac(int wier, int kol)
        {
            int[,] wynik = new int[wier, kol];
            return wynik;
        }

        private static int[,] DekMac(int[,] macierz, out int[] perm, out int toggle)
        {
            int wier = macierz.GetLength(0);
            int kol = macierz.GetLength(1);

            if (wier != kol)
                throw new Exception("Macierz nie jest kwadratowa!");

            int[,] wynik = macierzDuplicate(macierz);

            perm = new int[wier];
            for (int i = 0; i < wier; ++i) { perm[i] = i; }

            toggle = 1;

            for (int j = 0; j < wier - 1; ++j)
            {
                int kolmax = Math.Abs(wynik[j, j]);
                int pwier = j;
                for (int i = j + 1; i < wier; ++i)
                {
                    if (wynik[i, j] > kolmax)
                    {
                        kolmax = wynik[i, j];
                        pwier = i;
                    }
                }

                if (pwier != j)
                {
                    int[] wierPtr = new int[wynik.GetLength(1)];

                    for (int k = 0; k < wynik.GetLength(1); k++)
                    {
                        wierPtr[k] = wynik[pwier, k];
                    }

                    for (int k = 0; k < wynik.GetLength(1); k++)
                    {
                        wynik[pwier, k] = wynik[j, k];
                    }

                    for (int k = 0; k < wynik.GetLength(1); k++)
                    {
                        wynik[j, k] = wierPtr[k];
                    }

                    int tmp = perm[pwier];
                    perm[pwier] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle;
                }

                if (Math.Abs(wynik[j, j]) < 1.0E-20)
                    return null;

                for (int i = j + 1; i < wier; ++i)
                {
                    wynik[i, j] /= wynik[j, j];
                    for (int k = j + 1; k < wier; ++k)
                    {
                        wynik[i, k] -= wynik[i, j] * wynik[j, k];
                    }
                }
            }

            return wynik;
        }

        private static int WyzMac(int[,] macierz)
        {
            int[] perm;
            int toggle;
            int[,] lum = DekMac(macierz, out perm, out toggle);
            if (lum == null)
                throw new Exception("Nie można obliczyć wyznacznika!");
            int wynik = toggle;
            for (int i = 0; i < lum.GetLength(0); ++i)
                wynik *= lum[i, i];

            return wynik;
        }

        private static int[,] macierzDuplicate(int[,] macierz)
        {
            int[,] wynik = StworzMac(macierz.GetLength(0), macierz.GetLength(1));
            for (int i = 0; i < macierz.GetLength(0); ++i)
                for (int j = 0; j < macierz.GetLength(1); ++j)
                    wynik[i, j] = macierz[i, j];
            return wynik;
        }

        private static int[,] Niz(int[,] macierz)
        {
            int wier = macierz.GetLength(0); int kol = macierz.GetLength(1);
            int[,] wynik = StworzMac(wier, kol);
            for (int i = 0; i < wier; ++i)
            {
                for (int j = 0; j < kol; ++j)
                {
                    if (i == j)
                        wynik[i, j] = 1;
                    else if (i > j)
                        wynik[i, j] = macierz[i, j];
                }
            }
            return wynik;
        }

        private static int[,] Wyz(int[,] macierz)
        {
            int wier = macierz.GetLength(0); int kol = macierz.GetLength(1);
            int[,] wynik = StworzMac(wier, kol);
            for (int i = 0; i < wier; ++i)
            {
                for (int j = 0; j < kol; ++j)
                {
                    if (i <= j)
                        wynik[i, j] = macierz[i, j];
                }
            }
            return wynik;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Podaj wartość n:");
            int n = int.Parse(Console.ReadLine());
            int[,] macierzSasiedztwa = MacierzSasiedztwa(n);
            int[,] diagonalmacierz = Diagonal(MacierzSasiedztwa(n), n);
            Console.WriteLine("\nObwód naszej figury to: " + 3*n);
            Console.WriteLine("Ilość jego wierzchołków to: " + W(n));
            Console.WriteLine("Ilość jego krawędzi to: " + K(n));
            Console.WriteLine("\nMacierz sąsiedztwa naszego grafu:");

            for (int i = 0; i < W(n); i++)
            {
                for (int j = 0; j < W(n); j++)
                    Console.Write(macierzSasiedztwa[i, j] + " ");
                Console.WriteLine();
            }

            Console.WriteLine("\nMacierz sąsiedztwa po zamianie diagonalnych elementów na odpowiednie stopnie wierzchołków naszego grafu:");
            for (int i = 0; i < W(n); i++)
            {
                for (int j = 0; j < W(n); j++)
                    Console.Write(diagonalmacierz[i, j] + " ");
                Console.WriteLine();
            }

            int[,] dopAlg = DopAlg(diagonalmacierz, n);
            Console.WriteLine("\nMacierz z zamienionymi wartościami 1 na -1 i przygotowana do obliczenia ilości drzew rozpinających w grafie, dzięki dopełnieniu algebraicznemu:");

            for (int i = 0; i < W(n); i++)
            {
                for (int j = 0; j < W(n); j++)
                    Console.Write(dopAlg[i, j] + " ");
                Console.WriteLine();
            }

            int[,] macierz = StworzMac(W(n), W(n));
            macierz = dopAlg;

            int[] perm;
            int toggle;

            int[,] da = DekMac(macierz, out perm, out toggle);

            int[,] niz = Niz(da);
            int[,] wyz = Wyz(da);

            int det = WyzMac(macierz);

            Console.WriteLine("Dzięki obliczeniu dopełnienia algebraicznego, dowiadujemy się, że liczba drzew rozpinających w grafie wynosi: " + det);

            Console.ReadKey();
        }
    }
}
