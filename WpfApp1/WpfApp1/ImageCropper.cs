using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    public sealed class ImageCropper : Control
    {
        public static readonly DependencyProperty SourceImageProperty = DependencyProperty.Register(nameof(SourceImage), typeof(WriteableBitmap), typeof(ImageCropper), new PropertyMetadata(default(WriteableBitmap), OnSourceImageChanged));

        private static void OnSourceImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        static ImageCropper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageCropper), new FrameworkPropertyMetadata(typeof(ImageCropper)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.layoutRoot = this.GetTemplateChild("PART_LayoutRoot") as Grid;

            this.selectRegion = this.GetTemplateChild("PART_SelectRegion") as Path;
            selectRegion.IsManipulationEnabled = true;
            selectedRegion = new SelectedRegion { MinSelectRegionSize = 2 * CornerSize };
            this.DataContext = selectedRegion;
        }

        private SelectedRegion selectedRegion;

        private Grid layoutRoot;

        private Shape selectRegion;

        public WriteableBitmap SourceImage
        {
            get
            {
                return (WriteableBitmap)GetValue(SourceImageProperty);
            }
            set
            {
                SetValue(SourceImageProperty, value);
            }
        }
    }
}