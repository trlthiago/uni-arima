using System;
using RDotNet;
using System.Linq;
using log4net;

namespace uni_elastic_manager.infra
{
    public class ArimaWithRBinary : IArima
    {
        static int forecast = 8;
        private readonly int _p;
        private readonly int _d;
        private readonly int _q;
        private ILog _log;
        public REngine re { get; private set; }

        public ArimaWithRBinary(int p, int d, int q, ILog log)
        {
            _log = log;
            _q = q;
            _d = d;
            _p = p;
            re = REngine.GetInstance();
            re.Evaluate("require(forecast)");
        }

        public double Calculate(string[] metrics)
        {
            if (metrics.Length < 5)
                return 0;

            var start = DateTime.UtcNow.Ticks;
            try
            {
                var vector = re.CreateNumericVector(metrics.Select(x => double.Parse(x, System.Globalization.CultureInfo.CurrentCulture)).ToList());
                re.SetSymbol("y", vector);
                re.Evaluate($"fit=arima(y, c({_q},{_d},{_p}))");
                var resp = re.Evaluate($"f <- forecast(fit, h={forecast})");
                var r = resp.AsList();
                _log.Info($"Valor predito: {r[3].AsNumeric()[forecast - 1]}");
                return r[3].AsNumeric()[forecast - 1];
            }
            catch (System.Exception e)
            {
                _log.Error($"Ocorreu a exceção: {e}");
                return 0;
            }
        }
    }
}