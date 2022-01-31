namespace OntologyCSharpSDK.Wallet
{
    public class Scrypt
    {
        private int _n = 16384;
        private int _r = 8;
        private int _p = 8;

        public Scrypt()
        {
        }

        public Scrypt(int n, int r, int p)
        {
            this._n = n;
            this._r = r;
            this._p = p;
        }
    }
}
