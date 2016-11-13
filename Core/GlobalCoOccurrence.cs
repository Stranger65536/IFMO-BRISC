using System;
using System.Collections;
using System.IO;

namespace BRISC.Core
{
	/// <summary>
	/// Provides methods for calculating Haralick features on LIDCNodule objects
	/// </summary>
    /// <remarks>
    /// This class contains all the "heavy-lifting" for co-occurrence and 
    /// Haralick feature calculation. To calculate the Haralick features 
    /// for a nodule, you need the primary information about it (series, 
    /// slice, nodule ID, etc) and the segmented DICOM image. Then, create 
    /// an LIDCNodule object for the nodule and call PerformHaralick() on 
    /// that nodule. This will fill the Haralick data member with all of 
    /// the feature information.
    /// </remarks>
    public class GlobalCoOccurrence : FeatureExtractor
	{
        /// <summary>
        /// List of "official" Haralick feature names
        /// </summary>
        public static string[] featureNames = new string[11] {"contrast", "correlation", "energy", "homogeneity", "entropy", 
            "thirdOrderMoment", "inverseVariance", "sumAverage", "variance", "clusterTendency", "maximumProbability" };
        
        /// <summary>
        /// Number of directions: (0, 45, 90, 135 degrees)
        /// </summary>
        private static int numDirs = 4;
        /// <summary>
        /// Number of distances: (1, 2, 3 and 4)
        /// </summary>
        private static int numDists = 4;
        /// <summary>
        /// Number of Haralick features
        /// </summary>
        private static int numFeats = featureNames.Length;

        /// <summary>
        /// Calculates Haralick co-occurrence features for a nodule (saves the data in the structure)
        /// </summary>
        /// <param name="nodule">Nodule of interest</param>
        public void ExtractFeatures(LIDCNodule nodule)
        {
            // calculate co-occurrence matrices
            GlobalCoOccurrence g = new GlobalCoOccurrence();
            int[,] data = nodule.SegmentedPixelData;
            CombineCoOccurrence c = g.PerformCoOccurrence(data, -2000, numDists);

            // calculate [ distance x direction x feature ] matrix
            double[, ,] haralick = new double[numDists, numDirs, numFeats];
            for (int dist = 0; dist < numDists; dist++)
            {
                for (int dir = 0; dir < numDirs; dir++)
                {
                    // get matrix
                    double[,] com = c.getCoOccurrence(dist + 1, dir + 1);

                    // calculate row and column means
                    double imean = 0.0, jmean = 0.0, ivar = 0.0, jvar = 0.0;
                    for (int i = 0; i < com.GetLength(0); i++)
                    {
                        for (int j = 0; j < com.GetLength(1); j++)
                        {
                            imean += i * com[i, j];
                            jmean += j * com[i, j];
                        }
                    }

                    // calculate row and column variances
                    for (int i = 0; i < com.GetLength(0); i++)
                    {
                        for (int j = 0; j < com.GetLength(1); j++)
                        {
                            ivar += (Math.Pow(i - imean, 2.0) * com[i, j]);
                            jvar += (Math.Pow(j - jmean, 2.0) * com[i, j]);
                        }
                    }

                    // calculate features
                    for (int i = 0; i < com.GetLength(0); i++)
                    {
                        for (int j = 0; j < com.GetLength(1); j++)
                        {
                            // contrast
                            haralick[dist, dir, 0] += Math.Pow(i - j, 2.0) * com[i, j];
                            if (ivar != 0.0 && jvar != 0.0)
                            {
                                // correlation
                                haralick[dist, dir, 1] += (((i - imean) * (j - jmean) * com[i, j]) / Math.Sqrt(ivar * jvar));
                            }
                            // energy
                            haralick[dist, dir, 2] += Math.Pow(com[i, j], 2.0);
                            if (1 + Math.Abs(i - j) != 0.0)
                            {
                                // homogeneity
                                haralick[dist, dir, 3] += com[i, j] / (1.0 + Math.Abs(i - j));
                            }
                            if (com[i, j] != 0.0)
                            {
                                // entropy
                                haralick[dist, dir, 4] += -(com[i, j] * Math.Log(com[i, j]));
                            }
                            // third order moment
                            haralick[dist, dir, 5] += com[i, j] * Math.Pow(i - j, 3.0);
                            if (i - j != 0.0)
                            {
                                // inverse variance
                                haralick[dist, dir, 6] += com[i, j] / Math.Pow(i - j, 2.0);
                            }
                            // sum average
                            haralick[dist, dir, 7] += 0.5 * (i * com[i, j] + j * com[i, j]);
                            // variance
                            haralick[dist, dir, 8] += 0.5 * (com[i, j] * Math.Pow(i - imean, 2.0) + com[i, j] * Math.Pow(j - jmean, 2.0));
                            // cluster tendency
                            haralick[dist, dir, 9] += com[i, j] * Math.Pow(i - imean + j - jmean, 2.0);
                            if (i == 0 && j == 0 || com[i, j] > haralick[dist, dir, 10])
                            {
                                // maximum probability
                                haralick[dist, dir, 10] = com[i, j];
                            }
                        }
                    }
                }
            }

            // average to [ distance x feature ] matrix
            double[,] avgharalick = new double[numDists, numFeats];
            for (int dist = 0; dist < numDists; dist++)
            {
                for (int dir = 0; dir < numDirs; dir++)
                    for (int f = 0; f < numFeats; f++)
                        avgharalick[dist, f] += haralick[dist, dir, f];
                for (int f = 0; f < numFeats; f++)
                    avgharalick[dist, f] /= (double)numDirs;
            }

            // take minimum to obtain nodule features
            for (int f = 0; f < numFeats; f++)
            {
                double min = double.NaN;
                for (int dist = 0; dist < numDists; dist++)
                    if (double.IsNaN(min) || avgharalick[dist, f] < min)
                        min = avgharalick[dist, f];
                if (nodule.Haralick.ContainsKey(featureNames[f]))
                    nodule.Haralick[featureNames[f]] = min;
                else
                    nodule.Haralick.Add(featureNames[f], min);
            }
        }
        
        /// <summary>
        /// Calculate all co-occurrence matrices
        /// </summary>
        /// <param name="mOriginal">Original two-dimensional matrix</param>
        /// <param name="nNoValueIndicate">Background signal value</param>
        /// <param name="numDistance">Number of displacements to use</param>
        /// <returns>CombineCoOccurrence structure with all matrices</returns>
        public CombineCoOccurrence PerformCoOccurrence(int[,] mOriginal, int nNoValueIndicate, int numDistance)
		{
			int nDirectionNum = 4; // 1..4

            // get list of co-occurrence row/column values
			ArrayList arrHeader = getCoOccurrenceHeader(mOriginal, nNoValueIndicate);
            int[] mHeader = new int[arrHeader.Count];
            for (int i = 0; i < mHeader.GetLength(0); i++)
                mHeader[i] = (int)arrHeader[i];


            // generate all co-occurrence matrices and store them in an ArrayList
			ArrayList arrAllCoOccurrence = new ArrayList();
            for (int i = 0; i < numDistance; i++)
            {
                ArrayList arrCoOccurrence = new ArrayList();
                for (int j = 0; j < nDirectionNum; j++)
                    arrCoOccurrence.Add(getCoOccurrence(mOriginal, arrHeader, j + 1, i + 1));
                arrAllCoOccurrence.Add(arrCoOccurrence);
            }
			
            CombineCoOccurrence combineCo = new CombineCoOccurrence(mHeader, mHeader, arrAllCoOccurrence);
			
			return combineCo;

		}

        /// <summary>
        /// Calculate one co-occurrence matrix and normalize it
        /// </summary>
        /// <param name="mOriginal">Original two-dimensional matrix</param>
        /// <param name="arrHeader">List of unique matrix values</param>
        /// <param name="direction">Direction (1=0,2=45,3=90,4=135 degrees)</param>
        /// <param name="distance">Displacement</param>
        /// <returns>Normalized co-occurrence matrix</returns>
		private double [,] getCoOccurrence(int [,] mOriginal, ArrayList arrHeader, int direction, int distance)
		{
			//  4   3   2
			//   \  |  /
			//    \ | /
			//      X ----- 1
			double [,] mCoOccurrence = new double[arrHeader.Count, arrHeader.Count];
			int iElement, jElement, iPos, jPos, nTotalPair;
			
			int rBegin		= -1;
			int rIncrement	= -1;
			int rEnd		= -1;
			int cBegin		= -1;
			int cIncrement	= -1;
			int cEnd		= -1;

			switch (direction)
			{
				case 1: 
					rBegin	= 0;
					cBegin	= 0;
					rEnd	= mOriginal.GetLength(0); // row
					cEnd	= mOriginal.GetLength(1) - distance; //column
					rIncrement = 0;
					cIncrement = distance;
					break;
				case 2: 
					rBegin	= distance;
					cBegin	= 0;
					rEnd	= mOriginal.GetLength(0); // row
					cEnd	= mOriginal.GetLength(1) - distance; //column
					rIncrement = - distance;
					cIncrement = distance;
					break;
				case 3: 
					rBegin	= distance;
					cBegin	= 0;
					rEnd	= mOriginal.GetLength(0); // row
					cEnd	= mOriginal.GetLength(1); //column
					rIncrement = - distance;
					cIncrement = 0;
					break;
				case 4: 
					rBegin	= distance;
					cBegin	= distance;
					rEnd	= mOriginal.GetLength(0); // row
					cEnd	= mOriginal.GetLength(1); //column
					rIncrement = - distance;
					cIncrement = - distance;
					break;
			}

			// Initialize Matrix
			for(int r = 0; r<mCoOccurrence.GetLength(0); r++)
				for(int c = 0; c<mCoOccurrence.GetLength(1); c++)
					mCoOccurrence[r,c] = 0;
			
			// Check Pair
			nTotalPair = 0;
			for(int r = rBegin; r<rEnd; r++)
			{
				for(int c = cBegin; c<cEnd; c++)
				{
					iElement	= mOriginal[r,c];
					jElement	= mOriginal[r + rIncrement,c + cIncrement];
					iPos		= arrHeader.IndexOf(iElement);
					jPos		= arrHeader.IndexOf(jElement);
					if((iPos>=0) && (jPos>=0))
					{
						mCoOccurrence[iPos,jPos] = mCoOccurrence[iPos,jPos] + 1;
						nTotalPair++;
					}
				}
			}

			// Normalize CoOccurrence
			for(int r = 0; r<mCoOccurrence.GetLength(0); r++)
				for(int c = 0; c<mCoOccurrence.GetLength(1); c++)
					mCoOccurrence[r,c] = mCoOccurrence[r,c]/((double)nTotalPair);
			
			return mCoOccurrence;
		}

        /// <summary>
        /// Get a list of unique values in the matrix (these will become the 
        /// labels for the rows and columns of the co-occurrence matrix)
        /// </summary>
        /// <param name="mOriginal">Original two-dimensional matrix</param>
        /// <param name="nNoValueIndicate">Background signal value</param>
        /// <returns>ArrayList of unique ints</returns>
		public ArrayList getCoOccurrenceHeader(int [,] mOriginal, int nNoValueIndicate)
		{
			int [] m = new int[(mOriginal.GetLength(0) * mOriginal.GetLength(1))];
			int idx = 0;
			ArrayList arrDistinct = new ArrayList();
			int nTemp = nNoValueIndicate;

			// ------------------------
			// Get to 1 Dimension Array
			// For sorting purpose
			for(int i = 0; i< mOriginal.GetLength(0); i++)
			{
				for(int j = 0; j< mOriginal.GetLength(1); j++)
				{
					m[idx] = mOriginal[i,j];
					idx++;					
				}
			}
			Array.Sort(m);

			// -------------------------------
			// Get Distinct value
			// For Co-Occurrence matrix header
			for(int i = 0; i< m.GetLength(0); i++)
			{
				if(m[i] != nTemp)
				{
					nTemp = m[i];
					arrDistinct.Add(nTemp);
				}
			}
			
			return arrDistinct;
				 
		}
	}

    /// <summary>
    /// Helper class for co-occurrence calculation
    /// </summary>
    /// <remarks>
    /// This class holds all co-occurrence matrices for a particular matrix, in the form of nested ArrayLists (first by distance and then by direction).
    /// </remarks>
	public class CombineCoOccurrence
	{
        /// <summary>
        /// Co-occurrence row values
        /// </summary>
		private int[]	x_I;

        /// <summary>
        /// Co-occurrence column values
        /// </summary>
		private int[]	x_J;

        /// <summary>
        /// ArrayList (by displacement) of ArrayLists (by direction) of two-dimensional double arrays (co-occurrence matrices)
        /// </summary>
		private ArrayList x_AllCoOccurrence;

        /// <summary>
        /// Create a new co-occurrence combination object
        /// </summary>
        /// <param name="iHeader">Co-occurrence row values</param>
        /// <param name="jHeader">Co-occurrence column values</param>
        /// <param name="arrAllCoOccurrence">ArrayList (by displacement) of ArrayLists (by direction) of two-dimensional double arrays (co-occurrence matrices)</param>
		public CombineCoOccurrence(int[] iHeader, int[] jHeader, ArrayList arrAllCoOccurrence)
		{
			x_I = iHeader;
			x_J = jHeader;
			x_AllCoOccurrence = arrAllCoOccurrence;
		}

        /// <summary>
		/// Get All CoOccurrence at Specific distance
		/// Output Arraylist[1..4] in which	
		///		[1] -->correspond to --> direction 1
		///			in which contain [,] of Occurrence inside
		///		[2] -->correspond to --> direction 2
		///		[3] -->correspond to --> direction 3
		///		[4] -->correspond to --> direction 4
		/// E.g. (double[,]) object[1] 
		///			--> Will get CoOccurrence Matrix of direction 1
        /// </summary>
        /// <param name="distance">Desired displacement</param>
        /// <returns>ArrayList of four co-occurrence matrices, one for each direction</returns>
        public ArrayList getCoOccurrence(int distance)
        {
            //  4   3   2
            //   \  |  /
            //    \ | /
            //      X ----- 1
            if ((distance > 0) && (distance <= x_AllCoOccurrence.Count))
                return (ArrayList)x_AllCoOccurrence[distance - 1];
            else
                return null;
        }
        
        /// <summary>
        /// Get a particular co-occurrence matrix, given displacement and direction
        /// </summary>
        /// <param name="distance">Desired displacement</param>
        /// <param name="direction">Desired direction</param>
        /// <returns>Two-dimensional array of normalized co-occurrence data</returns>
        public double[,] getCoOccurrence(int distance, int direction)
        {
            //  4   3   2
            //   \  |  /
            //    \ | /
            //      X ----- 1
            if ((distance > 0) && (distance <= x_AllCoOccurrence.Count))
            {
                ArrayList arr = (ArrayList)x_AllCoOccurrence[distance - 1];
                if ((direction > 0) && (direction <= arr.Count))
                    return (double[,])arr[direction - 1];
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Get array of co-occurrence row values
        /// </summary>
        public int[] I
        {
            get
            {
                return x_I;
            }
        }

        /// <summary>
        /// Get array of co-occurrence column values
        /// </summary>
        public int[] J
        {
            get
            {
                return x_J;
            }
        }
	}
}
