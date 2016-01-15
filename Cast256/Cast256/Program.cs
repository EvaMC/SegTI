using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/******************************************************************************************************************************************/
/*                                                                                                                                        */
/*                               Programa usado para probar los siguientes casos                                                          */
/*                                        1-Clave fija y textos aleatorios                                                                */
/*                                        2-Clave a aleatoria y texto fijos                                                               */
/*Para probar ambos, se ha comentado texto, pero es encunetra todo este documento, solo hay que descomentar lo necesario                  */
/*                                                                                                                                        */
/******************************************************************************************************************************************/

namespace Cast256
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Programa de prueba para el efecto avalancha del cifrador CAST-256\n");

            Cast256.BETA[] Kr = new Cast256.BETA[Cast256.CAST_ROUNDS];
            Cast256.BETA[] Km = new Cast256.BETA[Cast256.CAST_ROUNDS];

            Cast256 cast256 = new Cast256();
            Random a = new Random();
            int aux;

            int [] hist = new int[128];

            //Inicializamos el array del histograma a 0
            for (int i = 0; i < 128; i++)
                hist[i] = 0;

            Cast256.KAPPA uKey = new Cast256.KAPPA();
            Cast256.KAPPA uKeyAux = new Cast256.KAPPA();
            // Defined user key -> Contraseña de uno de los Vectores prueba del "paper" original y usado como CLAVE FIJA
            /*uKey.A = 0x2342bb9e;
            uKey.B = 0xfa38542c;
            uKey.C = 0xbed0ac83;
            uKey.D = 0x940ac298;
            uKey.E = 0x8d7c47ce;
            uKey.F = 0x26490846;
            uKey.G = 0x1cc1b513;
            uKey.H = 0x7ae6b604;*/

            uKey.A = 0x00000000;
            uKey.B = 0x00000000;
            uKey.C = 0x00000000;
            uKey.D = 0x00000000;
            uKey.E = 0x00000000;
            uKey.F = 0x00000000;
            uKey.G = 0x00000000;
            uKey.H = 0x00000000;

            uKeyAux.A = 0x00000000;
            uKeyAux.B = 0x00000000;
            uKeyAux.C = 0x00000000;
            uKeyAux.D = 0x00000000;
            uKeyAux.E = 0x00000000;
            uKeyAux.F = 0x00000000;
            uKeyAux.G = 0x00000000;
            uKeyAux.H = 0x00000000;

            //Textos en claro iniciales y usados como TEXTO FIJOS
            Cast256.BETA Plain1 = new Cast256.BETA();
            Plain1.A = 0x00000000;
            Plain1.B = 0x00000000;
            Plain1.C = 0x00000000;
            Plain1.D = 0x00000000;

            Cast256.BETA Plain2 = new Cast256.BETA();
            Plain2.A = 0x00000000;
            Plain2.B = 0x00000000;
            Plain2.C = 0x00000000;
            Plain2.D = 0x00000000;

            //Generamos 4 bloques diferentes aleatorios -> TEXTO ALEATORIOS
            while ((Plain1.A == Plain2.A) && (Plain1.B == Plain2.B) && (Plain1.C == Plain2.C) && (Plain1.D == Plain2.D))
            {
                //2147483647 valor maximo de un entero en C#
                Plain1.A = Plain1.A + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                Plain1.B = Plain1.B + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                Plain1.C = Plain1.C + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                Plain1.D = Plain1.D + Convert.ToUInt32(a.Next(0, Int32.MaxValue));

                Plain2.A = Plain2.A + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                Plain2.B = Plain2.B + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                Plain2.C = Plain2.C + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                Plain2.D = Plain2.D + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
            }


            Console.WriteLine("Empecemos!!!");

            //Este bucle nos vale para el caso de clave aleatoria y texto fijos. En el otro caso no existiría
            int cont = 0;
            while (cont< 10000000)
            {
                //Cave para cifrar aleatoria
                while(((uKey.A == uKeyAux.A) && (uKey.B == uKeyAux.B) && (uKey.C == uKeyAux.C) && (uKey.D == uKeyAux.D) && (uKey.E == uKeyAux.E) && (uKey.F == uKeyAux.F) && (uKey.G == uKeyAux.G) && (uKey.H == uKeyAux.H)))
                {
                    uKey.A = uKey.A + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    uKey.B = uKey.B + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    uKey.C = uKey.C + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    uKey.D = uKey.D + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    uKey.E = uKey.E + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    uKey.F = uKey.F + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    uKey.G = uKey.G + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    uKey.H = uKey.H + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                }

                //Console.WriteLine("Yo soy ukey   :" + uKey.A + "  " + uKey.B + "  " + uKey.C + "  " + uKey.D + "  " + uKey.E + "  " + uKey.F + "  " + uKey.G + "  " + uKey.H);
                //Console.WriteLine("Yo soy ukeyAUX:" + uKeyAux.A + "  " + uKeyAux.B + "  " + uKeyAux.C + "  " + uKeyAux.D + "  " + uKeyAux.E + "  " + uKeyAux.F + "  " + uKeyAux.G + "  " + uKeyAux.H);

                // Initialize tables and keys
                cast256.CAST256TableInit();
                cast256.CAST256KeyInit(Kr, Km, uKey);

                //Bucle que provoca el efecto avalancha -> usado para cifrar los texto aleatorios.
                //for (int i = 0; i < 10000000; i++)
                //{
                    //Encriptamos los dos textos en claro
                    cast256.CAST256Encrypt(Kr, Km, ref Plain1);
                    cast256.CAST256Encrypt(Kr, Km, ref Plain2);

                    //Calculamos la distancia de Hamming
                    aux = cast256.distanciaHamming(ref Plain1, ref Plain2);
                    hist[aux] += 1;

                //Modificamos aleatoriamente Plain1 y Plain2
                while ((Plain1.A == Plain2.A) && (Plain1.B == Plain2.B) && (Plain1.C == Plain2.C) && (Plain1.D == Plain2.D))
                {

                    Plain1.A = Plain1.A + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    Plain1.B = Plain1.B + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    Plain1.C = Plain1.C + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    Plain1.D = Plain1.D + Convert.ToUInt32(a.Next(0, Int32.MaxValue));

                    Plain2.A = Plain2.A + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    Plain2.B = Plain2.B + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    Plain2.C = Plain2.C + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                    Plain2.D = Plain2.D + Convert.ToUInt32(a.Next(0, Int32.MaxValue));
                }

                //}
                uKeyAux.A = uKey.A;
                uKeyAux.B = uKey.B;
                uKeyAux.C = uKey.C;
                uKeyAux.D = uKey.D;
                uKeyAux.E = uKey.E;
                uKeyAux.F = uKey.F;
                uKeyAux.G = uKey.G;
                uKeyAux.H = uKey.H;
                cont++;
            }

            Console.WriteLine("IMPRIMIMOS LOS DATOS PARA LA PRÁCTICA");
            StreamWriter sw = new StreamWriter("C: \\Users\\EvaMc\\OneDrive\\Documentos\\SeguridadTI\\Practica\\Proyecto\\Avalancaha.csv", false);
        
            //Imprimimos con formato necesario para hacer un documento .csv
            for (int i = 0; i < 128; i++)
            {
                //Console.WriteLine("hist[" + i + "]=" + hist[i]);
                sw.Write("{0};{1};\n", i, hist[i]);
            }
            sw.Close();

            Console.WriteLine("He llegado al final/n");
            Console.Read();
        }
    }
}
