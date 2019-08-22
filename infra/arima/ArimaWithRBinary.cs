using System;
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

        public ArimaWithRBinary(int p, int d, int q)
        {
            _q = q;
            _d = d;
            _p = p;
            re = REngine.GetInstance();
            re.Evaluate("require(forecast)");
        }

        public double Calculate(string[] metrics)
        {
            var start = DateTime.Now.Ticks;
            var vector = re.CreateNumericVector(metrics.Select(x => double.Parse(x.Replace(".", ","))).ToList());
            re.SetSymbol("y", vector);
            re.Evaluate($"fit=arima(y, c({_q},{_d},{_p}))");
            var resp = re.Evaluate($"f <- forecast(fit, h={forecast})");
            var r = resp.AsList();
            return r[3].AsNumeric()[forecast - 1];     
        }
    }
}