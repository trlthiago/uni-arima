namespace uni_elastic_manager.Infra
{
    public class ArimaWithRBinary : IArima
    {
        private readonly int _p;
        private readonly int _d;
        private readonly int _q;
        public ArimaWithRBinary(int p, int d, int q)
        {
            _q = q;
            _d = d;
            _p = p;
        }

        public int Calculate(string[] metrics)
        {
            throw new System.NotImplementedException();
        }
    }
}