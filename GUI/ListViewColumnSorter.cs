using System;
using System.Collections;
using System.Windows.Forms;

using BRISC.Core;

namespace BRISC.GUI
{
    /// <summary>
    /// Custom sorter class for nodule list views.
    /// </summary>
    /// <remarks>
    /// Sorts by text, integer or float. Base code obtained from http://support.microsoft.com/kb/319401/EN-US/
    /// </remarks>
    class ListViewColumnSorter : IComparer
    {
        /// <summary>
        /// Types of sorts (case insensitive text, integer number, floating-point number)
        /// </summary>
        public enum SortStyle { String, IString, Integer, Float };

        /// <summary>
        /// Specifies the column to be sorted
        /// </summary>
        private int ColumnToSort;

        /// <summary>
        /// Specifies the order in which to sort (i.e. 'Ascending').
        /// </summary>
        private SortOrder OrderOfSort;

        /// <summary>
        /// Case sensitive comparer object
        /// </summary>
        private Comparer ObjectCaseCompare;

        /// <summary>
        /// Case insensitive comparer object
        /// </summary>
        private CaseInsensitiveComparer ObjectCompare;

        /// <summary>
        /// Current sort type
        /// </summary>
        private SortStyle sorting = SortStyle.String;

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            ColumnToSort = 0;

            // Initialize the sort order to 'none'
            OrderOfSort = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();

            // Initialize the Comparer object
            ObjectCaseCompare = new Comparer(System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares 
        /// the two objects passed using a comparison based on the current SortStyle.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items
            if (sorting == SortStyle.Integer)
            {
                compareResult = int.Parse(listviewX.SubItems[ColumnToSort].Text) - int.Parse(listviewY.SubItems[ColumnToSort].Text);
            }
            else if (sorting == SortStyle.Float)
            {
                double diff = double.Parse(listviewX.SubItems[ColumnToSort].Text) - double.Parse(listviewY.SubItems[ColumnToSort].Text);
                if (Math.Abs(diff) < double.Epsilon)
                    compareResult = 0;
                else if (diff < 0)
                    compareResult = -1;
                else
                    compareResult = 1;
            }
            else if (sorting == SortStyle.String)
            {
                compareResult = ObjectCaseCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
            }
            else
            {
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);
            }

            // Calculate correct return value based on object comparison
            if (OrderOfSort == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            else if (OrderOfSort == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return (-compareResult);
            }
            else
            {
                // Return '0' to indicate they are equal
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn
        {
            set
            {
                ColumnToSort = value;
            }
            get
            {
                return ColumnToSort;
            }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set
            {
                OrderOfSort = value;
            }
            get
            {
                return OrderOfSort;
            }
        }

        /// <summary>
        /// Gets or sets the type of sort (integer, floating-point, string or case-insensitive string)
        /// </summary>
        public SortStyle SortType
        {
            set
            {
                sorting = value;
            }
            get
            {
                return sorting;
            }
        }

    }
}