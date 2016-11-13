// This code was written by Paul Selormey at http://www.thecodeproject.com/csharp/psdotnetmatrix.asp.


using System;
using System.Runtime.Serialization;


namespace BRISC.Matrix
{
    [Serializable]
    public class CholeskyDecomposition : ISerializable
    {
        public CholeskyDecomposition(GeneralMatrix Arg)
        {
            // Initialize.
            var A = Arg.Array;
            n = Arg.RowDimension;
            L = new double[n][];
            for (var i = 0; i < n; i++)
            {
                L[i] = new double[n];
            }
            isspd = Arg.ColumnDimension == n;
            // Main loop.
            for (var j = 0; j < n; j++)
            {
                var Lrowj = L[j];
                var d = 0.0;
                for (var k = 0; k < j; k++)
                {
                    var Lrowk = L[k];
                    var s = 0.0;
                    for (var i = 0; i < k; i++)
                    {
                        s += Lrowk[i]*Lrowj[i];
                    }
                    Lrowj[k] = s = (A[j][k] - s)/L[k][k];
                    d = d + s*s;
                    isspd = isspd & (A[k][j] == A[j][k]);
                }
                d = A[j][j] - d;
                isspd = isspd & (d > 0.0);
                L[j][j] = Math.Sqrt(Math.Max(d, 0.0));
                for (var k = j + 1; k < n; k++)
                {
                    L[j][k] = 0.0;
                }
            }
        }

        public virtual bool SPD
        {
            get { return isspd; }
        }

        // Public Properties

        // A method called when serializing this class.
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        private readonly double[][] L;


        private readonly int n;


        private readonly bool isspd;

        public virtual GeneralMatrix GetL()
        {
            return new GeneralMatrix(L, n, n);
        }


        public virtual GeneralMatrix Solve(GeneralMatrix B)
        {
            if (B.RowDimension != n)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!isspd)
            {
                throw new SystemException("Matrix is not symmetric positive definite.");
            }

            // Copy right hand side.
            var X = B.ArrayCopy;
            var nx = B.ColumnDimension;

            // Solve L*Y = B;
            for (var k = 0; k < n; k++)
            {
                for (var i = k + 1; i < n; i++)
                {
                    for (var j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j]*L[i][k];
                    }
                }
                for (var j = 0; j < nx; j++)
                {
                    X[k][j] /= L[k][k];
                }
            }

            // Solve L'*X = Y;
            for (var k = n - 1; k >= 0; k--)
            {
                for (var j = 0; j < nx; j++)
                {
                    X[k][j] /= L[k][k];
                }
                for (var i = 0; i < k; i++)
                {
                    for (var j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j]*L[k][i];
                    }
                }
            }
            return new GeneralMatrix(X, n, nx);
        }

        //  Public Methods
    }
}