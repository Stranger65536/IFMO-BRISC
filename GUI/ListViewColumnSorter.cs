using System;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;


namespace BRISC.GUI
{
    internal class ListViewColumnSorter : IComparer
    {
        public enum SortStyle
        {
            String,
            IString,
            Integer,
            Float
        }


        private readonly Comparer ObjectCaseCompare;


        private readonly CaseInsensitiveComparer ObjectCompare;


        public ListViewColumnSorter()
        {
            // Initialize the column to '0'
            SortColumn = 0;

            // Initialize the sort order to 'none'
            Order = SortOrder.None;

            // Initialize the CaseInsensitiveComparer object
            ObjectCompare = new CaseInsensitiveComparer();

            // Initialize the Comparer object
            ObjectCaseCompare = new Comparer(CultureInfo.CurrentCulture);
        }


        public int SortColumn { set; get; }


        public SortOrder Order { set; get; }


        public SortStyle SortType { set; get; } = SortStyle.String;


        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem) x;
            listviewY = (ListViewItem) y;

            // Compare the two items
            if (SortType == SortStyle.Integer)
            {
                compareResult = int.Parse(listviewX.SubItems[SortColumn].Text) - int.Parse(listviewY.SubItems[SortColumn].Text);
            }
            else if (SortType == SortStyle.Float)
            {
                var diff = double.Parse(listviewX.SubItems[SortColumn].Text) - double.Parse(listviewY.SubItems[SortColumn].Text);
                if (Math.Abs(diff) < double.Epsilon)
                    compareResult = 0;
                else if (diff < 0)
                    compareResult = -1;
                else
                    compareResult = 1;
            }
            else if (SortType == SortStyle.String)
            {
                compareResult = ObjectCaseCompare.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);
            }
            else
            {
                compareResult = ObjectCompare.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);
            }

            // Calculate correct return value based on object comparison
            if (Order == SortOrder.Ascending)
            {
                // Ascending sort is selected, return normal result of compare operation
                return compareResult;
            }
            if (Order == SortOrder.Descending)
            {
                // Descending sort is selected, return negative result of compare operation
                return -compareResult;
            }
            // Return '0' to indicate they are equal
            return 0;
        }
    }
}