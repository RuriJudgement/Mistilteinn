using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mistilteinn.Controls
{
    public class CustomPanel : Panel
    {
        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
            "Spacing", typeof(Double), typeof(CustomPanel), new PropertyMetadata(default(Double)));

        public Double Spacing
        {
            get { return (Double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size requestSize = new Size(availableSize.Width, 0);
            Size provideSize = new Size((availableSize.Width - (Children.Count * Spacing)) / Children.Count, Double.PositiveInfinity);
            foreach (UIElement child in Children)
            {
                child.Measure(provideSize);
                if (child.DesiredSize.Height > requestSize.Height)
                {
                    requestSize.Height = child.DesiredSize.Height;
                }
            }

            return requestSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size provideSize = new Size((finalSize.Width - (Children.Count * Spacing)) / Children.Count, finalSize.Height);

            for (int index = 0; index < Children.Count; index++)
            {
                UIElement child = Children[index];

                child.Arrange(new Rect(new Point(index * (provideSize.Width + Spacing), 0),provideSize));
            }
            return finalSize;
        }
    }
}
