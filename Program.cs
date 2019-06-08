using System;
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
            Console.Write("Función: ");
            string function = Console.ReadLine();
            Console.Write("Valor inferior: ");
            a = Convert.ToSingle(Console.ReadLine());
            Console.Write("Valor superior: ");
            b = Convert.ToSingle(Console.ReadLine());
            Console.Write("Cantidad de puntos: ");
            cantidadPuntos = Convert.ToInt32(Console.ReadLine());
            Function f = new Function("f(x) = " + function);
            Expression e = new Expression("f(2)", f);
            List<PointF> puntos = new List<PointF>();
            menorY = MinimoValorF(function, a, b);
            mayorY = MaximoValorF(function, a, b);
            for (int i = 0; i <= cantidadPuntos; i++)
            {
                PointF p = new PointF();
                p.X = Convert.ToSingle(RandomNumberBetween((double) a, (double) b));
                p.Y = Convert.ToSingle(RandomNumberBetween(menorY, mayorY));
                puntos.Add(p);
                Console.WriteLine("X: "+p.X+ " Y: " + p.Y);
            }

            // Ahora si Montecarlo

            foreach (PointF p in puntos)
            {
                Argument x = new Argument("x");
                Expression exp = new Expression("f(x)",f);
                x.setArgumentValue(p.X);
                exp.addArguments(x);
                Console.WriteLine(p.Y);
                double valor = exp.calculate();
                if (p.Y >= valor)
                    cantidadPuntosAdentro++;
                else 
                    cantidadPuntosAfuera++;
            }
            Console.WriteLine("El valor aproximado de la integral es de: "+ (double)cantidadPuntosAdentro/(double)cantidadPuntosAfuera);
        }
    }
}
