using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp2
{
    public class ImageCropper : Control
    {
        public static readonly DependencyProperty CornerTemplateProperty = DependencyProperty.Register(nameof(CornerTemplate), typeof(DataTemplate), typeof(ImageCropper), new PropertyMetadata(default(DataTemplate)));

        private const string CanvasTemplateName = "PART_Canvas";

        private const string LeftBottomCornerGridTemplateName = "PART_LeftBottomCornerGrid";
        private const string LeftBottomCornerThumbTemplateName = "PART_LeftBottomCornerThumb";
        private const string LeftTopCornerGridTemplateName = "PART_LeftTopCornerGrid";
        private const string LeftTopCornerThumbTemplateName = "PART_LeftTopCornerThumb";
        private const string MaskPathTemplateName = "PART_MaskPath";

        private const string RightBottomCornerGridTemplateName = "PART_RightBottomCornerGrid";
        private const string RightBottomCornerThumbTemplateName = "PART_RightBottomCornerThumb";
        private const string RightTopCornerGridTemplateName = "PART_RightTopCornerGrid";
        private const string RightTopCornerThumbTemplateName = "PART_RightTopCornerThumb";
        private const string SelectedRegionThumbTemplateName = "PART_SelectedRegionThumb";

        private Canvas _canvas;

        private Grid _leftBottomCornerGrid;
        private Thumb _leftBottomCornerThumb;
        private Grid _leftTopCornerGrid;
        private Thumb _leftTopCornerThumb;
        private Path _maskPath;

        private Grid _rightBottomCornerGrid;
        private Thumb _rightBottomCornerThumb;
        private Grid _rightTopCornerGrid;
        private Thumb _rightTopCornerThumb;
        private Thumb _selectedRegionThumb;

        static ImageCropper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageCropper), new FrameworkPropertyMetadata(typeof(ImageCropper)));
        }

        public ImageCropper()
        {
            this.SizeChanged += ImageCropper_SizeChanged;
        }

        public DataTemplate CornerTemplate
        {
            get
            {
                return (DataTemplate)GetValue(CornerTemplateProperty);
            }
            set
            {
                SetValue(CornerTemplateProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _canvas = (Canvas)GetTemplateChild(CanvasTemplateName);

            _maskPath = (Path)GetTemplateChild(MaskPathTemplateName);

            _selectedRegionThumb = (Thumb)GetTemplateChild(SelectedRegionThumbTemplateName);
            _selectedRegionThumb.DragDelta += SelectedRegionThumb_DragDelta;
            _selectedRegionThumb.DragCompleted += SelectedRegionThumb_DragCompleted;

            _leftTopCornerGrid = (Grid)GetTemplateChild(LeftTopCornerGridTemplateName);
            _rightTopCornerGrid = (Grid)GetTemplateChild(RightTopCornerGridTemplateName);
            _rightBottomCornerGrid = (Grid)GetTemplateChild(RightBottomCornerGridTemplateName);
            _leftBottomCornerGrid = (Grid)GetTemplateChild(LeftBottomCornerGridTemplateName);

            _leftTopCornerThumb = (Thumb)GetTemplateChild(LeftTopCornerThumbTemplateName);
            _rightTopCornerThumb = (Thumb)GetTemplateChild(RightTopCornerThumbTemplateName);
            _rightBottomCornerThumb = (Thumb)GetTemplateChild(RightBottomCornerThumbTemplateName);
            _leftBottomCornerThumb = (Thumb)GetTemplateChild(LeftBottomCornerThumbTemplateName);

            _leftTopCornerThumb.DragDelta += LeftTopCornerThumb_DragDelta;
            _rightTopCornerThumb.DragDelta += RightTopCornerThumb_DragDelta;
            _rightBottomCornerThumb.DragDelta += RightBottomCornerThumb_DragDelta;
            _leftBottomCornerThumb.DragDelta += LeftBottomCornerThumb_DragDelta;

            UpdateMask();
            UpdateCornerPosition();
        }

        private void ImageCropper_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // TODO

            UpdateMask();
        }

        public ImageSource GetCroppedBitmap()
        {
            var left = (int)Canvas.GetLeft(_selectedRegionThumb);
            var top = (int)Canvas.GetTop(_selectedRegionThumb);
            var width = (int)_selectedRegionThumb.ActualWidth;
            var height = (int)_selectedRegionThumb.ActualHeight;

            RenderTargetBitmap b = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Default);
            b.Render(GetTemplateChild("PART_SourceImage") as Visual);

            var oo = new CroppedBitmap(b, new Int32Rect(left, top, width, height));

            System.IO.FileStream fs = new System.IO.FileStream("DOG.png", System.IO.FileMode.Create);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(oo));
            encoder.Save(fs);

            return oo;

            var image = (Bitmap)System.Drawing.Image.FromFile(@"D:\Projects\ImageCropper\WpfApp1\WpfApp2\bd_logo1_31bdc765.png");

            var bitmap = new Bitmap(width, height);
            for (int i = left; i < left + width && i < image.Width; i++)
            {
                for (int j = top; j < top + height && j < image.Height; j++)
                {
                    var c = image.GetPixel(i, j);
                    bitmap.SetPixel(i - left, j - top, c);
                }
            }

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);

            WriteableBitmap result = BitmapFactory.New(0, 0);

            return result.FromStream(stream);
        }

        private void LeftBottomCornerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var left = Canvas.GetLeft(_selectedRegionThumb);

            Canvas.SetLeft(_selectedRegionThumb, left + e.HorizontalChange);
            _selectedRegionThumb.Width -= e.HorizontalChange;
            _selectedRegionThumb.Height += e.VerticalChange;

            UpdateMask();
            UpdateCornerPosition();
        }

        private void LeftTopCornerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var left = Canvas.GetLeft(_selectedRegionThumb);
            var top = Canvas.GetTop(_selectedRegionThumb);

            Canvas.SetLeft(_selectedRegionThumb, left + e.HorizontalChange);
            Canvas.SetTop(_selectedRegionThumb, top + e.VerticalChange);
            _selectedRegionThumb.Width -= e.HorizontalChange;
            _selectedRegionThumb.Height -= e.VerticalChange;

            UpdateMask();
            UpdateCornerPosition();
        }

        private void RightBottomCornerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _selectedRegionThumb.Width += e.HorizontalChange;
            _selectedRegionThumb.Height += e.VerticalChange;

            UpdateMask();
            UpdateCornerPosition();
        }

        private void RightTopCornerThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var top = Canvas.GetTop(_selectedRegionThumb);

            Canvas.SetTop(_selectedRegionThumb, top + e.VerticalChange);
            _selectedRegionThumb.Width += e.HorizontalChange;
            _selectedRegionThumb.Height -= e.VerticalChange;

            UpdateMask();
            UpdateCornerPosition();
        }

        private void SelectedRegionThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            // TODO

            UpdateMask();
            UpdateCornerPosition();
        }

        private void SelectedRegionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var left = Canvas.GetLeft(_selectedRegionThumb);
            var top = Canvas.GetTop(_selectedRegionThumb);
            Canvas.SetLeft(_selectedRegionThumb, left + e.HorizontalChange);
            Canvas.SetTop(_selectedRegionThumb, top + e.VerticalChange);

            UpdateMask();
            UpdateCornerPosition();
        }

        private void UpdateCornerPosition()
        {
            if (_selectedRegionThumb != null)
            {
                var left = Canvas.GetLeft(_selectedRegionThumb);
                var top = Canvas.GetTop(_selectedRegionThumb);

                if (_leftTopCornerGrid != null)
                {
                    Canvas.SetLeft(_leftTopCornerGrid, left - _leftTopCornerGrid.ActualWidth / 2);
                    Canvas.SetTop(_leftTopCornerGrid, top - _leftTopCornerGrid.ActualHeight / 2);
                }
                if (_rightTopCornerGrid != null)
                {
                    Canvas.SetLeft(_rightTopCornerGrid, left + _selectedRegionThumb.ActualWidth - _rightTopCornerGrid.ActualWidth / 2);
                    Canvas.SetTop(_rightTopCornerGrid, top - _rightTopCornerGrid.ActualHeight / 2);
                }
                if (_rightBottomCornerGrid != null)
                {
                    Canvas.SetLeft(_rightBottomCornerGrid, left + _selectedRegionThumb.ActualWidth - _rightBottomCornerGrid.ActualWidth / 2);
                    Canvas.SetTop(_rightBottomCornerGrid, top + _selectedRegionThumb.ActualHeight - _rightBottomCornerGrid.ActualHeight / 2);
                }
                if (_leftBottomCornerGrid != null)
                {
                    Canvas.SetLeft(_leftBottomCornerGrid, left - _leftBottomCornerGrid.ActualWidth / 2);
                    Canvas.SetTop(_leftBottomCornerGrid, top + _selectedRegionThumb.ActualHeight - _leftBottomCornerGrid.ActualHeight / 2);
                }
            }
        }

        private void UpdateMask()
        {
            if (_canvas != null && _maskPath != null && _selectedRegionThumb != null)
            {
                var left = Canvas.GetLeft(_selectedRegionThumb);
                var top = Canvas.GetTop(_selectedRegionThumb);
                _maskPath.Data = new GeometryGroup()
                {
                    Children = new GeometryCollection()
                    {
                        new RectangleGeometry()
                        {
                            Rect = new Rect(0, 0, _canvas.ActualWidth, _canvas.ActualHeight)
                        },
                        new RectangleGeometry()
                        {
                            Rect = new Rect(left, top, _selectedRegionThumb.ActualWidth, _selectedRegionThumb.ActualHeight)
                        }
                    }
                };
            }
        }
    }
}