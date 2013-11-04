using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ModernWPF
{
    /// <summary>
    /// Specifies an accent color.
    /// </summary>
    public sealed class Accent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Accent"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="color">The color.</param>
        public Accent(string name, Color color)
        {
            Name = name;
            Color = color;

            // todo: instead of alpha modify in intensity

            Brush = GetBrush(0xff, color);
            Brush2 = GetBrush(0x90, color);
            Brush3 = GetBrush(0x80, color);
            Brush4 = GetBrush(0x70, color);
            Brush5 = GetBrush(0x60, color);
            Brush6 = GetBrush(0x50, color);
            Brush7 = GetBrush(0x40, color);
            Brush8 = GetBrush(0x30, color);
            Brush9 = GetBrush(0x20, color);
            Brush10 = GetBrush(0x10, color);
        }

        private SolidColorBrush GetBrush(byte alpha, Color color)
        {
            var brush = new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
            brush.Freeze();
            return brush;
        }

        /// <summary>
        /// Gets the accent name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the base color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color { get; private set; }
        /// <summary>
        /// Gets the accent brush.
        /// </summary>
        /// <value>
        /// The brush.
        /// </value>
        public Brush Brush { get; private set; }
        /// <summary>
        /// Gets the accent brush 2.
        /// </summary>
        /// <value>
        /// The brush2.
        /// </value>
        public Brush Brush2 { get; private set; }
        /// <summary>
        /// Gets the accent brush 3.
        /// </summary>
        /// <value>
        /// The brush3.
        /// </value>
        public Brush Brush3 { get; private set; }
        /// <summary>
        /// Gets the accent brush 4.
        /// </summary>
        /// <value>
        /// The brush4.
        /// </value>
        public Brush Brush4 { get; private set; }
        /// <summary>
        /// Gets the accent brush 5.
        /// </summary>
        /// <value>
        /// The brush5.
        /// </value>
        public Brush Brush5 { get; private set; }
        /// <summary>
        /// Gets the accent brush 6.
        /// </summary>
        /// <value>
        /// The brush6.
        /// </value>
        public Brush Brush6 { get; private set; }
        /// <summary>
        /// Gets the accent brush 7.
        /// </summary>
        /// <value>
        /// The brush7.
        /// </value>
        public Brush Brush7 { get; private set; }
        /// <summary>
        /// Gets the accent brush 8.
        /// </summary>
        /// <value>
        /// The brush8.
        /// </value>
        public Brush Brush8 { get; private set; }
        /// <summary>
        /// Gets the accent brush 9.
        /// </summary>
        /// <value>
        /// The brush9.
        /// </value>
        public Brush Brush9 { get; private set; }
        /// <summary>
        /// Gets the accent brush 10.
        /// </summary>
        /// <value>
        /// The brush10.
        /// </value>
        public Brush Brush10 { get; private set; }
    }
}
