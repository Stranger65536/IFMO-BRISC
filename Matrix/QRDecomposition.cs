// This code was written and made available under public domain by Paul Selormey at http://www.thecodeproject.com/csharp/psdotnetmatrix.asp.


using System;
using System.Runtime.Serialization;


namespace BRISC.Matrix
{
    [Serializable]
    public class QRDecomposition : ISerializable
    {
        public QRDecomposition(GeneralMatrix A)
        {
            // Initialize.
            QR = A.ArrayCopy;
            m = A.RowDimension;
            n = A.ColumnDimension;
            Rdiag = new double[n];

            // Main loop.
            for (var k = 0; k < n; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (var i = k; i < m; i++)
                {
                    nrm = Maths.Hypot(nrm, QR[i][k]);
                }

                if (nrm != 0.0)
                {
                    // Form k-th Householder vector.
                    if (QR[k][k] < 0)
                    {
                        nrm = -nrm;
                    }
                    for (var i = k; i < m; i++)
                    {
                        QR[i][k] /= nrm;
                    }
                    QR[k][k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (var j = k + 1; j < n; j++)
                    {
                        var s = 0.0;
                        for (var i = k; i < m; i++)
                        {
                            s += QR[i][k]*QR[i][j];
                        }
                        s = -s/QR[k][k];
                        for (var i = k; i < m; i++)
                        {
                            QR[i][j] += s*QR[i][k];
                        }
                    }
                }
                Rdiag[k] = -nrm;
            }
        }

        //  Constructor

        // A method called when serializing this class.
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        public virtual GeneralMatrix Solve(GeneralMatrix B)
        {
            if (B.RowDimension != m)
            {
                throw new ArgumentException("GeneralMatrix row dimensions must agree.");
            }
            if (!FullRank)
            {
                throw new SystemException("Matrix is rank deficient.");
            }

            // Copy right hand side
            var nx = B.ColumnDimension;
            var X = B.ArrayCopy;

            // Compute Y = transpose(Q)*B
            for (var k = 0; k < n; k++)
            {
                for (var j = 0; j < nx; j++)
                {
                    var s = 0.0;
                    for (var i = k; i < m; i++)
                    {
                        s += QR[i][k]*X[i][j];
                    }
                    s = -s/QR[k][k];
                    for (var i = k; i < m; i++)
                    {
                        X[i][j] += s*QR[i][k];
                    }
                }
            }
            // Solve R*X = Y;
            for (var k = n - 1; k >= 0; k--)
            {
                for (var j = 0; j < nx; j++)
                {
                    X[k][j] /= Rdiag[k];
                }
                for (var i = 0; i < k; i++)
                {
                    for (var j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j]*QR[i][k];
                    }
                }
            }

            return new GeneralMatrix(X, n, nx).GetMatrix(0, n - 1, 0, nx - 1);
        }

        private readonly double[][] QR;
        private readonly int m;
        private readonly int n;
        private readonly double[] Rdiag;

        public virtual bool FullRank
        {
            get
            {
                for (var j = 0; j < n; j++)
                {
                    if (Rdiag[j] == 0)
                        return false;
                }
                return true;
            }
        }


        public virtual GeneralMatrix H
        {
            get
            {
                var X = new GeneralMatrix(m, n);
                var H = X.Array;
                for (var i = 0; i < m; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i >= j)
                        {
                            H[i][j] = QR[i][j];
                        }
                        else
                        {
                            H[i][j] = 0.0;
                        }
                    }
                }
                return X;
            }
        }


        public virtual GeneralMatrix R
        {
            get
            {
                var X = new GeneralMatrix(n, n);
                var R = X.Array;
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        if (i < j)
                        {
                            R[i][j] = QR[i][j];
                        }
                        else if (i == j)
                        {
                            R[i][j] = Rdiag[i];
                        }
                        else
                        {
                            R[i][j] = 0.0;
                        }
                    }
                }
                return X;
            }
        }


        public virtual GeneralMatrix Q
        {
            get
            {
                var X = new GeneralMatrix(m, n);
                var Q = X.Array;
                for (var k = n - 1; k >= 0; k--)
                {
                    for (var i = 0; i < m; i++)
                    {
                        Q[i][k] = 0.0;
                    }
                    Q[k][k] = 1.0;
                    for (var j = k; j < n; j++)
                    {
                        if (QR[k][k] != 0)
                        {
                            var s = 0.0;
                            for (var i = k; i < m; i++)
                            {
                                s += QR[i][k]*Q[i][j];
                            }
                            s = -s/QR[k][k];
                            for (var i = k; i < m; i++)
                            {
                                Q[i][j] += s*QR[i][k];
                            }
                        }
                    }
                }
                return X;
            }
        }

        //  Public Properties
    }
}