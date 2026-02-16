using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WinNotes.Helpers
{
    internal class ImageAutoResizeBehavior : Behavior<RichTextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnTextChanged;
            AssociatedObject.SizeChanged += OnSizeChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= OnTextChanged;
            AssociatedObject.SizeChanged -= OnSizeChanged;
            base.OnDetaching();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateImages();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateImages();
        }

        private void UpdateImages()
        {
            if (AssociatedObject.Document == null)
                return;

            double maxWidth = AssociatedObject.ViewportWidth - 10;
            if (maxWidth <= 0) return;

            foreach (Block block in AssociatedObject.Document.Blocks)
            {
                if (block is BlockUIContainer blockContainer &&
                    blockContainer.Child is Image img)
                {
                    ApplySize(img, maxWidth);
                }

                if (block is Paragraph paragraph)
                {
                    foreach (Inline inline in paragraph.Inlines)
                    {
                        if (inline is InlineUIContainer inlineContainer &&
                            inlineContainer.Child is Image img2)
                        {
                            ApplySize(img2, maxWidth);
                        }
                    }
                }
            }
        }

        private void ApplySize(Image img, double maxWidth)
        {
            if (img.MaxWidth != maxWidth)
            {
                img.Stretch = Stretch.Uniform;
                img.MaxWidth = maxWidth;
            }
        }
    }
}
