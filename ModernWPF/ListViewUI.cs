using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernWPF
{
    public static class ListViewUI
    {
        #region sort cmd

        /// <summary>
        /// The sort command property on a <see cref="ListView"/> when clicking on the <see cref="GridView"/> view columns.
        /// The command parameter will be <see cref="GridViewSortParameter"/>.
        /// </summary>
        public static readonly DependencyProperty SortCommandProperty =
            DependencyProperty.RegisterAttached
            (
                "SortCommand",
                typeof(ICommand),
                typeof(ListViewUI),
                new PropertyMetadata(null, OnSortCommandPropertyChanged)
            );
        public static ICommand GetSortCommand(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (ICommand)obj.GetValue(SortCommandProperty);
        }
        public static void SetSortCommand(DependencyObject obj, ICommand value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(SortCommandProperty, value);
        }

        private static void OnSortCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) { return; }

            var lv = d as ListView;
            if (lv != null)
            {
                if (e.NewValue != null)
                {
                    lv.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(HandleHeaderClick));
                }
                else
                {
                    lv.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(HandleHeaderClick));
                    _lastSortTracker.Remove(lv);
                }
            }
        }

        static void HandleHeaderClick(object sender, RoutedEventArgs e)
        {
            var lv = sender as ListView;
            var head = e.OriginalSource as GridViewColumnHeader;
            if (lv != null && head != null && head.Role != GridViewColumnHeaderRole.Padding)
            {
                ICommand cmd = GetSortCommand(lv);
                if (cmd != null)
                {
                    // if there was a previously sorted column, check if it's the same as current

                    GridViewColumnHeader lastHead;
                    ListSortDirection? newSort = ListSortDirection.Ascending;

                    if (_lastSortTracker.TryGetValue(lv, out lastHead) && head == lastHead)
                    {
                        // same as last one, new sort cyles through asc->desc->none
                        var lastSort = ListViewUI.GetSortDirection(head);
                        if (lastSort.HasValue)
                        {
                            switch (lastSort.Value)
                            {
                                case ListSortDirection.Ascending:
                                    newSort = ListSortDirection.Descending;
                                    break;
                                case ListSortDirection.Descending:
                                    newSort = null;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // new column
                        if (lastHead != null)
                        {
                            lastHead.ClearValue(SortDirectionProperty);
                        }
                    }
                    SetSortDirection(head, newSort);
                    _lastSortTracker[lv] = head;
                    cmd.Execute(new GridViewSortParameter(head, newSort));
                }
            }
        }

        static readonly Dictionary<ListView, GridViewColumnHeader> _lastSortTracker = new Dictionary<ListView, GridViewColumnHeader>();

        #endregion

        #region sort dir (should only set by code in this class unless you know what you're doing)


        /// <summary>
        /// The sort direction property on a <see cref="GridViewColumnHeader"/>.
        /// </summary>
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.RegisterAttached
            (
                "SortDirection",
                typeof(ListSortDirection?),
                typeof(ListViewUI),
                new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets the sort direction.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public static ListSortDirection? GetSortDirection(DependencyObject obj)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            return (ListSortDirection?)obj.GetValue(SortDirectionProperty);
        }

        /// <summary>
        /// Sets the sort direction.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentNullException">obj</exception>
        public static void SetSortDirection(DependencyObject obj, ListSortDirection? value)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }
            obj.SetValue(SortDirectionProperty, value);
        }


        #endregion
    }
}
