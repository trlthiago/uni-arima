using System;
using System.Text;
using RDotNet;
using System.Linq;

namespace uni_elastic_manager.Infra
{
    public class ArimaWithRBinary : IArima
    {
        static int forecast = 8;
        private readonly int _p;
        private readonly int _d;
        private readonly int _q;
        public REngine re { get; private set; }
        public double[,] values;

        static string output_path = @"C:\PROJECTS\DiCloudArima\DiCloudArima\output.txt";
        static string commands_path = @"C:\PROJECTS\DiCloudArima\DiCloudArima\commands.txt";
        static string y;
        public StringBuilder sb;
        public StringBuilder rpipe;

        public ArimaWithRBinary(int p, int d, int q)
        {
            _q = q;
            _d = d;
            _p = p;
        }

        public int Calculate(string[] metrics)
        {
            sb = new StringBuilder();
            rpipe = new StringBuilder();

            var arg = new string[] { "--no-save" };

            //var re = new REngine(arg, true, null);
            re = REngine.GetInstance();

            re.Evaluate("require(forecast)");
            rpipe.AppendLine("require(forecast)");

            values = new double[27, 150];

            var start = DateTime.Now.Ticks;

            process(metrics);

            return 0;
        }

        public double arima(int a, int b, int g)
        {
            var fit = $"fit=arima(y, c({a},{b},{g}))";
            rpipe.AppendLine(fit);
            var f = $"f <- forecast(fit, h={forecast})";
            rpipe.AppendLine(f);

            try
            {
                re.Evaluate($"fit=arima(y, c({a},{b},{g}))");
            }
            catch
            {
            }

            var resp = re.Evaluate($"f <- forecast(fit, h={forecast})");
            var r = resp.AsList();
            return r[3].AsNumeric()[forecast - 1];
        }

        public void process(string[] aux)
        {
            for (var g = 0; g < 3; g++)
            {
                for (var b = 0; b < 3; b++)
                {
                    for (var a = 0; a < 3; a++)
                    {
                        //Console.WriteLine("Arima(" + a + "," + b + "," + g + ")");
                        for (var i = 0; i < aux.Length; i++)
                        {
                            var aux2 = new string[(i + 1)];
                            for (var j = 0; j <= i; j++)
                            {
                                aux2[j] = aux[j];
                            }

                            var aux2String = string.Join(",", aux2.Select(x => x.ToString()));
                            y = $"y <- c({aux2String})";
                            rpipe.AppendLine(y);
                            //re.assign("y", aux2);
                            var vector = re.CreateNumericVector(aux2.Select(x => double.Parse(x)).ToList());
                            //var vector = re.CreateNumericMatrix(aux)
                            re.SetSymbol("y", vector);
                            if (i > 5)
                            {
                                values[(a * 9) + (b * 3) + g, i + forecast] = arima(a, b, g);
                            }
                        }
                    }
                }
            }

            sb.AppendLine(@"Sequência;Real;Arima(0,0,0);Arima(0,0,1);Arima(0,0,2);Arima(0,1,0);Arima(0,1,1);Arima(0,1,2);Arima(0,2,0);Arima(0,2,1);Arima(0,2,2);Arima(1,0,0);Arima(1,0,1);Arima(1,0,2);Arima(1,1,0);Arima(1,1,1);Arima(1,1,2);Arima(1,2,0);Arima(1,2,1);Arima(1,2,2);Arima(2,0,0);Arima(2,0,1);Arima(2,0,2);Arima(2,1,0);Arima(2,1,1);Arima(2,1,2);Arima(2,2,0);Arima(2,2,1);Arima(2,2,2);");
            for (var i = 0; i < aux.Length; i++)
            {
                var valores = "";
                for (var j = 0; j < 27; j++)
                {
                    valores = valores + ";" + values[j, i];
                }

                sb.AppendLine(i + ";" + aux[i] + valores);
            }

            //Console.WriteLine(rpipe.ToString());c:
        }
    }
}