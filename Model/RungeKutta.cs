using System;
using System.Linq;

namespace ModelLibrary
{
    public static class RungeKuttaIteration
    {
        public static double[] Get(double[] X, Func<double[], double, double[]> F, double t, double dt)
        {
            var K = F(X, t).Select(x => x * dt).ToArray();
            var L = F(X.Zip(K, (x, k) => x + k / 2).ToArray(), t + dt / 2).Select(x => x * dt).ToArray();
            var M = F(X.Zip(L, (x, l) => x + l / 2).ToArray(), t + dt / 2).Select(x => x * dt).ToArray();
            var N = F(X.Zip(M, (x, m) => x + m).ToArray(), t + dt).Select(x => x * dt).ToArray();
            double[] Delta_X = new double[X.Length];
            for (int i = 0; i < X.Length; i++)
                Delta_X[i] = 1.0 / 6 * (K[i] + 2 * L[i] + 2 * M[i] + N[i]);
            return Delta_X;
        }
    }
}
