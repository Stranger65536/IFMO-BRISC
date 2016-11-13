// This code was written and made available under public domain by Paul Selormey at http://www.thecodeproject.com/csharp/psdotnetmatrix.asp.


using System;
using System.Runtime.Serialization;


namespace BRISC.Matrix
{
    internal class Maths
    {
        public static double Hypot(double a, double b)
        {
            double r;
            if (Math.Abs(a) > Math.Abs(b))
            {
                r = b/a;
                r = Math.Abs(a)*Math.Sqrt(1 + r*r);
            }
            else if (b != 0)
            {
                r = a/b;
                r = Math.Abs(b)*Math.Sqrt(1 + r*r);
            }
            else
            {
                r = 0.0;
            }
            return r;
        }
    }

    // Internal Maths utility

    [Serializable]
    public class GeneralMatrix : ICloneable, ISerializable, IDisposable
    {
        public object Clone()
        {
            return Copy();
        }


        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        private void CheckMatrixDimensions(GeneralMatrix B)
        {
            if (B.m != m || B.n != n)
            {
                throw new ArgumentException("GeneralMatrix dimensions must agree.");
            }
        }

        private readonly double[][] A;
        private readonly int m;
        private readonly int n;

        public GeneralMatrix(int m, int n)
        {
            this.m = m;
            this.n = n;
            A = new double[m][];
            for (var i = 0; i < m; i++)
            {
                A[i] = new double[n];
            }
        }


        public GeneralMatrix(int m, int n, double s)
        {
            this.m = m;
            this.n = n;
            A = new double[m][];
            for (var i = 0; i < m; i++)
            {
                A[i] = new double[n];
            }
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = s;
                }
            }
        }


        public GeneralMatrix(double[][] A)
        {
            m = A.Length;
            n = A[0].Length;
            for (var i = 0; i < m; i++)
            {
                if (A[i].Length != n)
                {
                    throw new ArgumentException("All rows must have the same length.");
                }
            }
            this.A = A;
        }


        public GeneralMatrix(double[][] A, int m, int n)
        {
            this.A = A;
            this.m = m;
            this.n = n;
        }


        public GeneralMatrix(double[] vals, int m)
        {
            this.m = m;
            n = m != 0 ? vals.Length/m : 0;
            if (m*n != vals.Length)
            {
                throw new ArgumentException("Array length must be a multiple of m.");
            }
            A = new double[m][];
            for (var i = 0; i < m; i++)
            {
                A[i] = new double[n];
            }
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = vals[i + j*m];
                }
            }
        }

        public virtual double[][] Array
        {
            get { return A; }
        }


        public virtual double[][] ArrayCopy
        {
            get
            {
                var C = new double[m][];
                for (var i = 0; i < m; i++)
                {
                    C[i] = new double[n];
                }
                for (var i = 0; i < m; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        C[i][j] = A[i][j];
                    }
                }
                return C;
            }
        }


        public virtual double[] ColumnPackedCopy
        {
            get
            {
                var vals = new double[m*n];
                for (var i = 0; i < m; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        vals[i + j*m] = A[i][j];
                    }
                }
                return vals;
            }
        }


        public virtual double[] RowPackedCopy
        {
            get
            {
                var vals = new double[m*n];
                for (var i = 0; i < m; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        vals[i*n + j] = A[i][j];
                    }
                }
                return vals;
            }
        }


        public virtual int RowDimension
        {
            get { return m; }
        }


        public virtual int ColumnDimension
        {
            get { return n; }
        }

        public static GeneralMatrix Create(double[][] A)
        {
            var m = A.Length;
            var n = A[0].Length;
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                if (A[i].Length != n)
                {
                    throw new ArgumentException("All rows must have the same length.");
                }
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix Copy()
        {
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = A[i][j];
                }
            }
            return X;
        }


        public virtual double GetElement(int i, int j)
        {
            return A[i][j];
        }


        public virtual GeneralMatrix GetMatrix(int i0, int i1, int j0, int j1)
        {
            var X = new GeneralMatrix(i1 - i0 + 1, j1 - j0 + 1);
            var B = X.Array;
            try
            {
                for (var i = i0; i <= i1; i++)
                {
                    for (var j = j0; j <= j1; j++)
                    {
                        B[i - i0][j - j0] = A[i][j];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
            return X;
        }


        public virtual GeneralMatrix GetMatrix(int[] r, int[] c)
        {
            var X = new GeneralMatrix(r.Length, c.Length);
            var B = X.Array;
            try
            {
                for (var i = 0; i < r.Length; i++)
                {
                    for (var j = 0; j < c.Length; j++)
                    {
                        B[i][j] = A[r[i]][c[j]];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
            return X;
        }


        public virtual GeneralMatrix GetMatrix(int i0, int i1, int[] c)
        {
            var X = new GeneralMatrix(i1 - i0 + 1, c.Length);
            var B = X.Array;
            try
            {
                for (var i = i0; i <= i1; i++)
                {
                    for (var j = 0; j < c.Length; j++)
                    {
                        B[i - i0][j] = A[i][c[j]];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
            return X;
        }


        public virtual GeneralMatrix GetMatrix(int[] r, int j0, int j1)
        {
            var X = new GeneralMatrix(r.Length, j1 - j0 + 1);
            var B = X.Array;
            try
            {
                for (var i = 0; i < r.Length; i++)
                {
                    for (var j = j0; j <= j1; j++)
                    {
                        B[i][j - j0] = A[r[i]][j];
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
            return X;
        }


        public virtual void SetElement(int i, int j, double s)
        {
            A[i][j] = s;
        }


        public virtual void SetMatrix(int i0, int i1, int j0, int j1, GeneralMatrix X)
        {
            try
            {
                for (var i = i0; i <= i1; i++)
                {
                    for (var j = j0; j <= j1; j++)
                    {
                        A[i][j] = X.GetElement(i - i0, j - j0);
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
        }


        public virtual void SetMatrix(int[] r, int[] c, GeneralMatrix X)
        {
            try
            {
                for (var i = 0; i < r.Length; i++)
                {
                    for (var j = 0; j < c.Length; j++)
                    {
                        A[r[i]][c[j]] = X.GetElement(i, j);
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
        }


        public virtual void SetMatrix(int[] r, int j0, int j1, GeneralMatrix X)
        {
            try
            {
                for (var i = 0; i < r.Length; i++)
                {
                    for (var j = j0; j <= j1; j++)
                    {
                        A[r[i]][j] = X.GetElement(i, j - j0);
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
        }


        public virtual void SetMatrix(int i0, int i1, int[] c, GeneralMatrix X)
        {
            try
            {
                for (var i = i0; i <= i1; i++)
                {
                    for (var j = 0; j < c.Length; j++)
                    {
                        A[i][c[j]] = X.GetElement(i - i0, j);
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException("Submatrix indices", e);
            }
        }


        public virtual GeneralMatrix Transpose()
        {
            var X = new GeneralMatrix(n, m);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[j][i] = A[i][j];
                }
            }
            return X;
        }


        public virtual double Norm1()
        {
            double f = 0;
            for (var j = 0; j < n; j++)
            {
                double s = 0;
                for (var i = 0; i < m; i++)
                {
                    s += Math.Abs(A[i][j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }


        public virtual double Norm2()
        {
            return new SingularValueDecomposition(this).Norm2();
        }


        public virtual double NormInf()
        {
            double f = 0;
            for (var i = 0; i < m; i++)
            {
                double s = 0;
                for (var j = 0; j < n; j++)
                {
                    s += Math.Abs(A[i][j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }


        public virtual double NormF()
        {
            double f = 0;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    f = Maths.Hypot(f, A[i][j]);
                }
            }
            return f;
        }


        public virtual GeneralMatrix UnaryMinus()
        {
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = -A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix Add(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = A[i][j] + B.A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix AddEquals(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = A[i][j] + B.A[i][j];
                }
            }
            return this;
        }


        public virtual GeneralMatrix Subtract(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = A[i][j] - B.A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix SubtractEquals(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = A[i][j] - B.A[i][j];
                }
            }
            return this;
        }


        public virtual GeneralMatrix ArrayMultiply(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = A[i][j]*B.A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix ArrayMultiplyEquals(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = A[i][j]*B.A[i][j];
                }
            }
            return this;
        }


        public virtual GeneralMatrix ArrayRightDivide(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = A[i][j]/B.A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix ArrayRightDivideEquals(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = A[i][j]/B.A[i][j];
                }
            }
            return this;
        }


        public virtual GeneralMatrix ArrayLeftDivide(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = B.A[i][j]/A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix ArrayLeftDivideEquals(GeneralMatrix B)
        {
            CheckMatrixDimensions(B);
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = B.A[i][j]/A[i][j];
                }
            }
            return this;
        }


        public virtual GeneralMatrix Multiply(double s)
        {
            var X = new GeneralMatrix(m, n);
            var C = X.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    C[i][j] = s*A[i][j];
                }
            }
            return X;
        }


        public virtual GeneralMatrix MultiplyEquals(double s)
        {
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    A[i][j] = s*A[i][j];
                }
            }
            return this;
        }


        public virtual GeneralMatrix Multiply(GeneralMatrix B)
        {
            if (B.m != n)
            {
                throw new ArgumentException("GeneralMatrix inner dimensions must agree.");
            }
            var X = new GeneralMatrix(m, B.n);
            var C = X.Array;
            var Bcolj = new double[n];
            for (var j = 0; j < B.n; j++)
            {
                for (var k = 0; k < n; k++)
                {
                    Bcolj[k] = B.A[k][j];
                }
                for (var i = 0; i < m; i++)
                {
                    var Arowi = A[i];
                    double s = 0;
                    for (var k = 0; k < n; k++)
                    {
                        s += Arowi[k]*Bcolj[k];
                    }
                    C[i][j] = s;
                }
            }
            return X;
        }

        public static GeneralMatrix operator +(GeneralMatrix m1, GeneralMatrix m2)
        {
            return m1.Add(m2);
        }


        public static GeneralMatrix operator -(GeneralMatrix m1, GeneralMatrix m2)
        {
            return m1.Subtract(m2);
        }


        public static GeneralMatrix operator *(GeneralMatrix m1, GeneralMatrix m2)
        {
            return m1.Multiply(m2);
        }

        //Operator Overloading

        public virtual LUDecomposition LUD()
        {
            return new LUDecomposition(this);
        }


        public virtual QRDecomposition QRD()
        {
            return new QRDecomposition(this);
        }


        public virtual CholeskyDecomposition chol()
        {
            return new CholeskyDecomposition(this);
        }


        public virtual SingularValueDecomposition SVD()
        {
            return new SingularValueDecomposition(this);
        }


        public virtual EigenvalueDecomposition Eigen()
        {
            return new EigenvalueDecomposition(this);
        }


        public virtual GeneralMatrix Solve(GeneralMatrix B)
        {
            return m == n ? new LUDecomposition(this).Solve(B) : new QRDecomposition(this).Solve(B);
        }


        public virtual GeneralMatrix SolveTranspose(GeneralMatrix B)
        {
            return Transpose().Solve(B.Transpose());
        }


        public virtual GeneralMatrix Inverse()
        {
            return Solve(Identity(m, m));
        }


        public virtual double Determinant()
        {
            return new LUDecomposition(this).Determinant();
        }


        public virtual int Rank()
        {
            return new SingularValueDecomposition(this).Rank();
        }


        public virtual double Condition()
        {
            return new SingularValueDecomposition(this).Condition();
        }


        public virtual double Trace()
        {
            double t = 0;
            for (var i = 0; i < Math.Min(m, n); i++)
            {
                t += A[i][i];
            }
            return t;
        }


        public static GeneralMatrix Random(int m, int n)
        {
            var random = new Random();

            var A = new GeneralMatrix(m, n);
            var X = A.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    X[i][j] = random.NextDouble();
                }
            }
            return A;
        }


        public static GeneralMatrix Identity(int m, int n)
        {
            var A = new GeneralMatrix(m, n);
            var X = A.Array;
            for (var i = 0; i < m; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    X[i][j] = i == j ? 1.0 : 0.0;
                }
            }
            return A;
        }

        public void Dispose()
        {
            Dispose(true);
        }


        private void Dispose(bool disposing)
        {
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            if (disposing)
                GC.SuppressFinalize(this);
        }


        ~GeneralMatrix()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        //  Implement IDisposable
    }
}