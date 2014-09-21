using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ModernWPF
{
    /// <summary>
    /// The parameter passed to <see cref="ListViewUI.SortCommand"/>.
    /// </summary>
    public class GridViewSortParameter
    {
        public GridViewSortParameter(GridViewColumnHeader header, ListSortDirection? newDirection)
        {
            Header = header;
            NewSortDirection = newDirection;
        }

        /// <summary>
        /// Gets the header that was clicked.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public GridViewColumnHeader Header { get; private set; }

        /// <summary>
        /// Gets the new sort direction. If this is null then no sort should happen.
        /// </summary>
        /// <value>
        /// The new sort direction.
        /// </value>
        public ListSortDirection? NewSortDirection { get; private set; }
    }
}
