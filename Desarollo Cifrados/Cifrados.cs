﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Numerics;

namespace Desarollo_Cifrados
{
    public class Cifrados
    {
        private static Cifrados _instance = null;
        public static Cifrados Instance
        {
            get
            {
                if (_instance == null) _instance = new Cifrados();
                return _instance;
            }
        }
        string GlobalPath = string.Empty;
     
        #region Rail

        public void RailCodificar(int grado,string _path)
        {
            GlobalPath = _path;
            var decoded = new FileStream(GlobalPath, FileMode.Open);
            var reader = new StreamReader(decoded);
            var text = reader.ReadToEnd();
            var nivel = new string[grado];
            var index = 0;
            bool direction = false;
            var ciclos = text.Length / grado;
            while (text != "")
            {
                if (index < 4) //abajo
                {
                    for (int i = 0; i < grado; i++)
                    {
                        if (text == "")
                        {
                            break;
                        }
                        nivel[i] = nivel[i] + text[0].ToString();
                        text = text.Remove(0, 1);
                        index++; direction = false;
                    }
                    if (text == "")
                    {
                        break;
                    }
                }//para abajo
                else //arriba
                {
                    for (int i = grado - 2; i > 0; i--)
                    {
                        if (text == "")
                        {
                            for (int r = i; r > -1; r--)
                            {
                                nivel[r] = nivel[r] + "$";
                            }
                            break;
                        }
                        nivel[i] = nivel[i] + text[0].ToString();
                        text = text.Remove(0, 1);
                        direction = true;
                    }
                    index = 0;
                } //para arriba

            }

            //Escritura
            var nombrearchivo = $"{Path.GetDirectoryName(decoded.Name)}\\{Path.GetFileNameWithoutExtension(decoded.Name)}_ZigZag.txt";
            var encoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
            var writer = new BinaryWriter(encoded);
            var codedtext = "";
            foreach (var item in nivel)
            {
                codedtext = codedtext + item;
            }
            var temp = codedtext.ToArray();
            foreach (var item in temp)
            {
                writer.Write(item);

            }
            decoded.Close();
            encoded.Close();


            var monitor = 0;
        }
        public void RailDecodificar(int grado, string _path)
        {
            GlobalPath = _path;
            var Original = new FileStream(GlobalPath, FileMode.Open);
            var reader = new StreamReader(Original);
            var ciphertext = reader.ReadToEnd();
            var m = (ciphertext.Length + (2 * grado) - 3) / ((2 * grado) - 2);
            var midtline = (m - 1) * 2;
            var lastline = m - 1;
            var nivel = new string[grado];
            //primer nivel
            nivel[0] = ciphertext.Substring(0, m);
            ciphertext = ciphertext.Remove(0, m);
            //intermedios
            for (int i = 1; i < grado - 1; i++)
            {
                nivel[i] = ciphertext.Substring(0, midtline);
                ciphertext = ciphertext.Remove(0, midtline);

            }
            //final
            nivel[grado - 1] = ciphertext;
            var uncipher = string.Empty;
            while (nivel[0] != "" && nivel[grado - 1] != "")
            {
                var index = 0;
                for (int i = 0; i < grado; i++)
                {
                    uncipher = uncipher + nivel[i][0];
                    nivel[i] = nivel[i].Remove(0, 1);
                }
                for (int i = grado - 2; i > 0; i--)
                {
                    uncipher = uncipher + nivel[i][0];
                    nivel[i] = nivel[i].Remove(0, 1);
                }
            }
            uncipher = uncipher.Replace('$', ' ');


            var nombrearchivo = $"{Path.GetDirectoryName(Original.Name)}\\{Path.GetFileNameWithoutExtension(Original.Name)}_ZZD.txt".Replace("_ZigZag", "");
            var decoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
            var writer = new BinaryWriter(decoded);
            var codedtext = "";


            foreach (var item in uncipher)
            {
                writer.Write(item);

            }
            decoded.Close();
            Original.Close();


            var matrix = new string[grado];
        }
        // listo
        #endregion
        #region Cesar

        public void CifrarCesar(string clave,string _path)//recibe el archivo a cifrar
        {
            GlobalPath = _path;
            var Alfabeto = LlenarAlfabeto_Cifrado();
            var write = "";
            var AlfabetoCifrado = new Dictionary<int, char>();
            byte[] KEYENCRYPTER = Encoding.ASCII.GetBytes(clave);
            var n = 0;
            foreach (var item in KEYENCRYPTER)
            {
                if (!AlfabetoCifrado.ContainsValue((char)item))
                {
                    AlfabetoCifrado.Add(n, (char)item);
                    n++;
                }
            }
            AlfabetoCifrado = LlenarCesar_Cifrado(AlfabetoCifrado);

            #region Variables De Acceso
            var Original = new FileStream(GlobalPath, FileMode.Open); //archivo
            var reader = new StreamReader(Original);
            var nombrearchivo = $"{Path.GetDirectoryName(Original.Name)}\\Ces_{Path.GetFileNameWithoutExtension(Original.Name)}.txt";
            var encoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
            var writer = new BinaryWriter(encoded);

            #endregion
            var text = reader.ReadToEnd();
            foreach (var item in text)
            {
                if (Alfabeto.ContainsKey(item) == false)
                {
                    Alfabeto.Add(item, Alfabeto.Count);
                }
                if (AlfabetoCifrado.ContainsValue(item) == false)
                {
                    AlfabetoCifrado.Add(AlfabetoCifrado.Count, item);
                }

                var monitor = Alfabeto[item];
                var monitorcecar = AlfabetoCifrado[monitor];
                write = $"{write}{monitorcecar}";
            }
            foreach (var item in write)
            {
                writer.Write(item);
            }
            Original.Close();
            encoded.Close();
        }
        public void DecifrarCesar(string codigo, string _path)
        {
            GlobalPath = _path;

            var Alfabeto = LlenarAlfabeto_DeCifrado();
            var write = "";
            var AlfabetoCifrado = new Dictionary<char, int>();
            byte[] KEYENCRYPTER = Encoding.ASCII.GetBytes(codigo);
            var n = 0;
            foreach (var item in KEYENCRYPTER)
            {
                if (!AlfabetoCifrado.ContainsValue((char)item))
                {
                    AlfabetoCifrado.Add((char)item, n);
                    n++;
                }
            }
            AlfabetoCifrado = LlenarCesar_DeCifrado(AlfabetoCifrado);

            var Cifrado = new FileStream(GlobalPath, FileMode.Open); //archivo
            var reader = new StreamReader(Cifrado);
            var nombrearchivo = $"{Path.GetDirectoryName(Cifrado.Name)}\\{Path.GetFileNameWithoutExtension(Cifrado.Name)}_Des.txt".Replace("Ces_","");

            var decoded = new FileStream(nombrearchivo, FileMode.OpenOrCreate); //archivo
            var writer = new BinaryWriter(decoded);


            var text = reader.ReadToEnd();
            foreach (var item in text)
            {
                if (Alfabeto.ContainsValue(item) == false)
                {
                    Alfabeto.Add(Alfabeto.Count, item);
                }
                if (AlfabetoCifrado.ContainsKey(item) == false)
                {
                    AlfabetoCifrado.Add(item, AlfabetoCifrado.Count);
                }

                var monitor = AlfabetoCifrado[item];
                var monitorcecar = Alfabeto[monitor];
                write = $"{write}{monitorcecar}";
            }
            foreach (var item in write)
            {
                writer.Write(item);
            }
            decoded.Close();
            Cifrado.Close();
        }
        Dictionary<char, int> LlenarAlfabeto_Cifrado()
        {
            var temp = new Dictionary<char, int>();
            int cont = 0;
           // var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
           // var reader = new StreamReader(fTemp);
            var bufer = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";

            foreach (var item in bufer)
            {
                if (!temp.ContainsKey(item))
                {
                    temp.Add((char)item, cont);
                    cont++;
                }

            }
           // fTemp.Close();
            return temp;
        }
        Dictionary<int, char> LlenarCesar_Cifrado(Dictionary<int, char> actual)
        {

           // var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
          //  var reader = new StreamReader(fTemp);
            var texto = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";
            foreach (var item in texto)
            {
                if (actual.ContainsValue(item) == false)
                {
                    actual.Add(actual.Count, item);
                }

            }


          //  fTemp.Close();
            return actual;
        }
        Dictionary<int, char> LlenarAlfabeto_DeCifrado()
        {
            var temp = new Dictionary<int, char>();
            int cont = 0;
          //  var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
          //  var reader = new StreamReader(fTemp);
            var bufer = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";

            foreach (var item in bufer)
            {
                if (!temp.ContainsKey(item))
                {
                    temp.Add(cont, (char)item);
                    cont++;
                }

            }
          //  fTemp.Close();
            return temp;
        }
        Dictionary<char, int> LlenarCesar_DeCifrado(Dictionary<char, int> actual)
        {

          //  var fTemp = new FileStream("C:\\Users\\roche\\Desktop\\Lab22_Cifrado\\Alfabeto.txt", FileMode.Open);
          //  var reader = new StreamReader(fTemp);
            var texto = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZÁÉÍÓÚabcdefghijklmnñopqrstuvwxyzáéíóú";
            foreach (var item in texto)
            {
                if (actual.ContainsKey(item) == false)
                {
                    actual.Add(item, actual.Count);
                }

            }


           // fTemp.Close();
            return actual;
        }
        #endregion

    } //listo
}