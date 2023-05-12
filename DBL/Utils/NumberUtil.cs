using System;
using System.Collections.Generic;
using System.Text;

namespace BITPay.DBL
{
   public class NumberUtil
    {
        //REF:https://www.c-sharpcorner.com/article/convert-numeric-value-into-words-currency-in-c-sharp/
        public static string ToWords(string number)
        {
            try
            {
                number = Convert.ToDouble(number.Replace(",","").Trim()).ToString();

                string isNegative = "";
                if (number.Contains("-"))
                {
                    isNegative = "Minus ";
                    number = number.Substring(1, number.Length - 1);
                }
                if (number == "0")
                {
                    return "Zéro Francs";
                }

                return isNegative + ConvertToWords(number);
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        private static String ConvertToWords(String numb)
        {
            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            String endStr = "Francs";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = "and";// just to separate whole numbers from points/cents  
                        endStr = "Paisa " + endStr;//Cents  
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }

        private static String ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range

                            word = Ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = Tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Cent ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Mille ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Milliard ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros
                        //if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "Soixante-seize";
                }
            }
            catch { }
            return word.Trim();
        }

        private static String Tens(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = null;
            switch (_Number)
            {
                case 10:
                    name = "Dix";
                    break;
                case 11:
                    name = "Onze";
                    break;
                case 12:
                    name = "Douze";
                    break;
                case 13:
                    name = "Treize";
                    break;
                case 14:
                    name = "Quatorze";
                    break;
                case 15:
                    name = "Quinze";
                    break;
                case 16:
                    name = "Seize";
                    break;
                case 17:
                    name = "Dix-sept";
                    break;
                case 18:
                    name = "Dix-huit";
                    break;
                case 19:
                    name = "Dix-neuf";
                    break;
                case 20:
                    name = "Vingt";
                    break;
                case 30:
                    name = "Trente";
                    break;
                case 40:
                    name = "Quarante";
                    break;
                case 50:
                    name = "Cinquante";
                    break;
                case 60:
                    name = "Soixante";
                    break;
                case 70:
                    name = "Soixante-dix";
                    break;
                case 71:
                    name = "Soixante-onze";
                    break;
                case 72:
                    name = "Soixante-douze";
                    break;
                case 73:
                    name = "Soixante-treize";
                    break;
                case 74:
                    name = "Soixante-quatorze";
                    break;
                case 75:
                    name = "Soixante-quinze";
                    break;
                case 76:
                    name = "Soixante-seize";
                    break;
                case 77:
                    name = "Soixante-dix-sept";
                    break;
                case 78:
                    name = "Soixante-dix-huit";
                    break;
                case 79:
                    name = "Soixante-dix-neuf";
                    break;
                case 80:
                    name = "Quatre-vingt";
                    break;
                case 90:
                    name = "Quatre-vingt-dix";
                    break;
                case 91:
                    name = "Quatre-vingt-onze";
                    break;
                case 92:
                    name = "Quatre-vingt-douze";
                    break;
                case 93:
                    name = "Quatre-vingt-treize";
                    break;
                case 94:
                    name = "Quatre-vingt-quatorze";
                    break;
                case 95:
                    name = "Quatre-vingt-quinze";
                    break;
                case 96:
                    name = "Quatre-vingt-seize";
                    break;
                case 97:
                    name = "Quatre-vingt-dix-sept";
                    break;
                case 98:
                    name = "Quatre-vingt-dix-huit";
                    break;
                case 99:
                    name = "Quatre-vingt-dix-neuf";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = Tens(Number.Substring(0, 1) + "0") + " " + Ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }

        private static String Ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "Un";
                    break;
                case 2:
                    name = "Deux";
                    break;
                case 3:
                    name = "Trois";
                    break;
                case 4:
                    name = "Quatre";
                    break;
                case 5:
                    name = "Cinq";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Sept";
                    break;
                case 8:
                    name = "Huit";
                    break;
                case 9:
                    name = "Neuf";
                    break;
            }
            return name;
        }

        private static String ConvertDecimals(String number)
        {
            String cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zéro";
                }
                else
                {
                    engOne = Ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }
    }
}
