using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ModernWPF
{
    /// <summary>
    /// Contains various attached properties for <see cref="MenuItem"/>.
    /// </summary>
    public class MenuItemUI : DependencyObject
    {

        #region group name extension
        // from http://stackoverflow.com/questions/3652688/mutually-exclusive-checkable-menu-items

        static Dictionary<MenuItem, String> ElementToGroupNames = new Dictionary<MenuItem, String>();


        /// <summary>
        /// Attached propert for grouped check behavior.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.RegisterAttached("GroupName",
                                         typeof(String),
                                         typeof(MenuItemUI),
                                         new PropertyMetadata(String.Empty, OnGroupNameChanged));

        /// <summary>
        /// Sets the GroupName property for this object.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetGroupName(MenuItem element, String value)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            element.SetValue(GroupNameProperty, value);
        }

        /// <summary>
        /// Gets the GroupName property for this object.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static String GetGroupName(MenuItem element)
        {
            if (element == null) { throw new ArgumentNullException("element"); }
            return element.GetValue(GroupNameProperty).ToString();
        }

        private static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Add an entry to the group name collection
            var menuItem = d as MenuItem;

            if (menuItem != null)
            {
                String newGroupName = e.NewValue.ToString();
                String oldGroupName = e.OldValue.ToString();
                if (String.IsNullOrEmpty(newGroupName))
                {
                    //Removing the toggle button from grouping
                    RemoveCheckboxFromGrouping(menuItem);
                }
                else
                {
                    //Switching to a new group
                    if (newGroupName != oldGroupName)
                    {
                        if (!String.IsNullOrEmpty(oldGroupName))
                        {
                            //Remove the old group mapping
                            RemoveCheckboxFromGrouping(menuItem);
                        }
                        ElementToGroupNames.Add(menuItem, e.NewValue.ToString());
                        menuItem.Checked += MenuItemChecked;
                    }
                }
            }
        }

        private static void RemoveCheckboxFromGrouping(MenuItem checkBox)
        {
            ElementToGroupNames.Remove(checkBox);
            checkBox.Checked -= MenuItemChecked;
        }


        static void MenuItemChecked(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            foreach (var item in ElementToGroupNames)
            {
                if (item.Key != menuItem && item.Value == GetGroupName(menuItem))
                {
                    item.Key.IsChecked = false;
                }
            }
        }

        #endregion
    }
}
