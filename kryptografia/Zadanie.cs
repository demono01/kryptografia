using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kryptografia
{
    public class Zadanie
    {
        //asdsdsdads
        //asdsdsdads
        //asdsdsdads
        private List<List<string>> readyCryptograms { get; set; }
        private List<string> toDecode { get; set; }
        private List<List<string>> propablysKeys { get; set; }

        public void run(string indeks)
        {
            PrepareToKeySearching(DoFormat(getPageSource(indeks)));
            SearchKey();
            printPosibilities();
        }

        private String getPageSource(string numerIndeksu)
        {
            WebClient webClient = new WebClient();
            Stream data = webClient.OpenRead("http://zagorski.im.pwr.wroc.pl/courses/kbk2015/l1.php?id=" + numerIndeksu);
            StreamReader reader = new StreamReader(data);
            string text = reader.ReadToEnd();
            data.Close();
            reader.Close();

            return text;
        }

        private List<string> DoFormat(string text)
        {
            text = Regex.Replace(text, "<.+?>", String.Empty);
            text = Regex.Replace(text, "kryptogram nr [0-9]+:", "|");

            string[] table = text.Split('(');

            toDecode = table[1].Substring(table[1].IndexOf(":") + 1).Split(' ').ToList(); //Ostatani kryptogram, czyli ten który należy odszyfrować
            toDecode.RemoveAt(toDecode.Count - 1);

            List<string> cryptograms = table[0].Substring(1).Split('|').ToList();//lista 20 kryptogramów;

            return cryptograms;
        }

        private void PrepareToKeySearching(List<string> list)
        {
            readyCryptograms = list.Select((x => x.Substring(0, x.Length - 1).Split(' ').ToList())).ToList(); //kryptogramy podzielone na 8 bitowe elementy
        }

        private void SearchKey()
        {
            propablysKeys = new List<List<string>>();
            for (int k = 0; k < toDecode.Count(); k++)
            {
                propablysKeys.Add(new List<string>());

                for (int i = 0; i <= 255; i++)
                {
                    string wynik = "";

                    foreach (var lista in readyCryptograms)
                    {
                        int decimalAsciiCode = Convert.ToInt32(lista[k], 2) ^ i;//mnoży 1 ciag 8 bit przez 0 - 255 jak wyjdzie liczba to convertuje poczym sprawdza czy to litera jak tak wrzyca do rozwiazania


                        if (!CheckCode(decimalAsciiCode))
                        {
                            break;
                        }
                        else
                        {
                            wynik += Convert.ToChar(decimalAsciiCode);
                        }
                    }
                    if (wynik.Length == 20)
                    {
                        propablysKeys[k].Add(Convert.ToString(i, 2).PadLeft(8, '0'));

                    }
                }
            }
        }
        private void printPosibilities()
        {
            int k = 0;
            foreach (var d in toDecode)
            {
                for (int i = 0; i < propablysKeys[k].Count; i++)
                {
                    int a = Convert.ToInt32(d, 2) ^ Convert.ToInt32(propablysKeys[k][i], 2);
                    if (CheckCode(a))
                    {
                        Console.Write(Convert.ToChar(a) + "_");
                    }
                }
                Console.WriteLine();
                k++;
            }
            Console.WriteLine("finish");
            Console.WriteLine("finish");
        }

        private bool CheckCode(int decimalAsciiCode)
        {
            //return (decimalAsciiCode <= 126 && decimalAsciiCode >= 32) ? true : false;
            if (48 <= decimalAsciiCode && decimalAsciiCode <= 57)//liczby 0-9
            {
                return true;
            }
            else if (63 <= decimalAsciiCode && decimalAsciiCode <= 90)//?@A-Z
            {
                return true;
            }
            else if (97 <= decimalAsciiCode && decimalAsciiCode <= 122)//a-z
            {
                return true;
            }
            else if (32 <= decimalAsciiCode && decimalAsciiCode <= 34)//spacja ! "
            {
                return true;
            }
            else if (44 <= decimalAsciiCode && decimalAsciiCode <= 46)// , - .
            {
                return true;
            }
            else if (58 <= decimalAsciiCode && decimalAsciiCode <= 59)//: ;
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private string XOR(string a, string b)
        {
            string c = "";

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i])
                {
                    c += "0";
                }
                else
                {
                    c += "1";
                }
            }
            return c;
        }
    }
}