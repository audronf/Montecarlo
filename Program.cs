﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Flee.PublicTypes;
using org.mariuszgromada.math.mxparser;


namespace Montecarlo
{
    class Program
    {
        /* https://github.com/mariuszgromada/MathParser.org-mXparser */
        private static readonly Random getrandom = new Random();

        private static readonly Random random = new Random();

        private static double RandomNumberBetween(double minValue, double maxValue)
        {
            var next = random.NextDouble();
            return minValue + (next * (maxValue - minValue));
        }

        static float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            // choose -149 instead of -126 to also generate subnormal floats (*)
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        static double MinimoValorF(string funcion, float a, float b)
        {
            Function f = new Function("f(x) = " + funcion);
            Expression e = new Expression("f(x)", f);
            Argument arg = new Argument("x");
            e.addArguments(arg);
            double[] valores = mXparser.getFunctionValues(e, arg, (double) a, (double) b, 0.01d);
            double min = 0;
            for (int i = 0; i < valores.Length; i++)
            {
                if (valores[i] <= min)
                    min = valores[i];
            }
            return min;
        }

        static double MaximoValorF(string funcion, float a, float b)
        {
            Function f = new Function("f(x) = " + funcion);
            Expression e = new Expression("f(x)", f);
            Argument arg = new Argument("x");
            e.addArguments(arg);
            double[] valores = mXparser.getFunctionValues(e, arg, (double) a, (double) b, 0.01d);
            double max = 0;
            for (int i = 0; i < valores.Length; i++)
            {
                if (valores[i] >= max)
                    max = valores[i];
            }
            return max;
        }

        static void Main(string[] args)
        {
            float a = 0.0f;
            float b = 1.0f;
            double menorY, mayorY;
            int cantidadPuntos;
            int cantidadPuntosAdentro = 0;
            int cantidadPuntosAfuera = 0;
            double valorIntegral = 0;
            double error = 0d;
            Console.Write("Función: ");
            string function = Console.ReadLine();
            Console.Write("Valor inferior: ");
            a = Convert.ToSingle(Console.ReadLine());
            Console.Write("Valor superior: ");
            b = Convert.ToSingle(Console.ReadLine());
            Console.Write("Cantidad de puntos: ");
            cantidadPuntos = Convert.ToInt32(Console.ReadLine());
            Function f = new Function("f(x) = " + function);
            List<PointF> puntos = new List<PointF>();
            menorY = MinimoValorF(function, a, b);
            mayorY = MaximoValorF(function, a, b);
            Expression expIntegral = new Expression("int(("+function+"), x,"+a+","+b+")");
            valorIntegral = expIntegral.calculate();
            for (int i = 0; i <= cantidadPuntos; i++)
            {
                PointF p = new PointF();
                p.X = Convert.ToSingle(RandomNumberBetween((double) a, (double) b));
                p.Y = Convert.ToSingle(RandomNumberBetween(menorY, mayorY));
                puntos.Add(p);
                Console.WriteLine("Generando punto aleatoriamente ==> X: "+p.X+ " Y: " + p.Y);
            }
            foreach (PointF p in puntos)
            {
                Argument x = new Argument("x");
                Expression exp = new Expression("f(x)",f);
                x.setArgumentValue(p.X);
                exp.addArguments(x);
                double valor = exp.calculate();
                if (p.Y <= valor)
                    cantidadPuntosAdentro++;
                else 
                    cantidadPuntosAfuera++;
            }
            double resultado;
            resultado=((double)cantidadPuntosAdentro/(double)cantidadPuntos)*((double)(b-(double)a)*((double)mayorY-(double)menorY));
            error = (Math.Abs(resultado - valorIntegral)/valorIntegral)*100;
            Console.WriteLine("Se generaron "+cantidadPuntosAdentro+" en el interior de la función, de un total de "+cantidadPuntos+ " puntos.");
            Console.WriteLine("El valor aproximado de la integral es de: "+ resultado);
            Console.WriteLine("El valor calculado de la integral es: "+ valorIntegral);
            if (error != 0)
                Console.WriteLine("Por lo tanto, el error porcentual en este cálculo es de: "+error+"%.");
            else  
                Console.WriteLine("Por lo tanto, no hubo error en este cálculo.");
        }
    }
}
