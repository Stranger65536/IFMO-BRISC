// This code was written and made available under public domain by Paul Selormey at http://www.thecodeproject.com/csharp/psdotnetmatrix.asp.


using System;
using System.Runtime.Serialization;


namespace BRISC.Matrix
{
    [Serializable]
    public class LUDecomposition : ISerializable
    {
        public LUDecomposition(GeneralMatrix A)
        {
            // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

            LU = A.ArrayCopy;
            m = A.RowDimension;
            n = A.ColumnDimension;
            piv = new int[m];
            for (var i = 0; i < m; i++)
            {
                piv[i] = i;
            }
            pivsign = 1;
            double[] LUrowi;
            var LUcolj = new double[m];

            // Outer loop.

            for (var j = 0; j < n; j++)
            {
                // Make a copy of the j-th column to localize references.

                for (var i = 0; i < m; i++)
                {
                    LUcolj[i] = LU[i][j];
                }

                // Apply previous transformations.

                for (var i = 0; i < m; i++)
                {
                    LUrowi = LU[i];

                    // Most of the time is spent in the following dot product.

                    var kmax = Math.Min(i, j);
                    var s = 0.0;
                    for (var k = 0; k < kmax; k++)
                    {
                        s += LUrowi[k]*LUcolj[k];
                    }

                    LUrowi[j] = LUcolj[i] -= s;
                }

                // Find pivot and exchange if necessary.

                var p = j;
                for (var i = j + 1; i < m; i++)
                {
                    if (Math.Abs(LUcolj[i]) > Math.Abs(LUcolj[p]))
                    {
                        p = i;
                    }
                }
                if (p != j)
                {
                    for (var k = 0; k < n; k++)
                    {
                        var t = LU[p][k];
                        LU[p][k] = LU[j][k];
                        LU[j][k] = t;
                    }
                    var k2 = piv[p];
                    piv[p] = piv[j];
                    piv[j] = k2;
                    pivsign = -pivsign;
                }

                // Compute multipliers.

                if (j < m & LU[j][j] != 0.0)
                {
                    for (var i = j + 1; i < m; i++)
                    {
                        LU[i][j] /= LU[j][j];
                    }
                }
            }
        }

        //  Constructor

        // A method called when serializing this class.
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        private readonly double[][] LU;
        private readonly int m;
        private readonly int n;
        private readonly int pivsign;
        private readonly int[] piv;

        public virtual bool IsNonSingular
        {
            get
            {
                for (var j = 0; j < n; j++)
                {
                    if (LU[j][j] == 0)
                        return false;
                }
                return true;
            }
        }


        public virtual GeneralMatrix L
        {
            get
            {
                var X = new GeneralMatrix(m, n);
                var L = X.Array;
                for (var i = 0; i < m; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i > j)
                        {
                            L[i][j] = LU[i][j];
                        }
                        else if (i == j)
                        {
                            L[i][j] = 1.0;
                        }
                        else
                        {
                            L[i][j] = 0.0;
                        }
                    }
                }
                return X;
            }
        }


        public virtual GeneralMatrix U
        {
            get
            {
                var X = new GeneralMatrix(n, n);
                var U = X.Array;
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i <= j)
                        {
                            U[i][j] = LU[i][j];
                        }
                        else
                        {
                            U[i][j] = 0.0;
                        }
                    }
                }
                return X;
            }
        }


        public virtual int[] Pivot
        {
            get
            {
                var p = new int[m];
                for (var i = 0; i < m; i++)
                {
                    p[i] = piv[i];
                }
                return p;
            }
        }


        public virtual double[] DoublePivot
        {
            get
            {
                var vals = new double[m];
                for (var i = 0; i < m; i++)
                {
                    vals[i] = piv[i];
                }
                return vals;
            }
        }

        public virtual double Determinant()
        {
            if (m != n)
            {
                throw new ArgumentException("Matrix must be square.");
            }
            double d = pivsign;
            for (var j = 0; j < n; j++)
            {
                d *= LU[j][j];
            }
            return d;
        }


        public virtual GeneralMatrix Solve(GeneralMatrix B)
        {
            if (B.RowDimension != m)
            {
                throw new ArgumentException("Matrix row dimensions must agree.");
            }
            if (!IsNonSingular)
            {
                throw new SystemException("Matrix is singular.");
            }

            // Copy right hand side with pivoting
            var nx = B.ColumnDimension;
            var Xmat = B.GetMatrix(piv, 0, nx - 1);
            var X = Xmat.Array;

            // Solve L*Y = B(piv,:)
            for (var k = 0; k < n; k++)
            {
                for (var i = k + 1; i < n; i++)
                {
                    for (var j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j]*LU[i][k];
                    }
                }
            }
            // Solve U*X = Y;
            for (var k = n - 1; k >= 0; k--)
            {
                for (var j = 0; j < nx; j++)
                {
                    X[k][j] /= LU[k][k];
                }
                for (var i = 0; i < k; i++)
                {
                    for (var j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j]*LU[i][k];
                    }
                }
            }
            return Xmat;
        }

        //  Public Methods
    }
}