using System.Windows;
using System.Windows.Interop;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace WinNotes.Helpers
{
    internal class WindowSnapHelper
    {
        public struct WindowRectangle
        {
            public double X;
            public double Y;
            public double Width;
            public double Height;
        }

        #region SnapToScreenEdge
        public static readonly DependencyProperty SnapToScreenEdgeProperty = DependencyProperty.RegisterAttached("SnapToScreenEdge", typeof(bool), typeof(WindowSnapHelper), new PropertyMetadata(false, OnSnapToScreenEdgeChanged));

        public static void SetSnapToScreenEdge(DependencyObject element, bool value) => element.SetValue(SnapToScreenEdgeProperty, value);

        public static bool GetSnapToScreenEdge(DependencyObject element) => (bool)element.GetValue(SnapToScreenEdgeProperty);

        private static void OnSnapToScreenEdgeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                if ((bool)e.NewValue)
                {
                    window.SizeChanged += Window_SizeChanged;
                    window.MouseMove += Window_MouseMove;
                }
                else
                {
                    window.SizeChanged -= Window_SizeChanged;
                    window.MouseMove -= Window_MouseMove;
                }
            }
        }

        private static void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SnapToScreenEdge(sender as Window);
        }

        private static void Window_MouseMove(object sender, MouseEventArgs e)
        {
            SnapToScreenEdge(sender as Window);
        }

        private static WindowRectangle prevRectangle = new WindowRectangle { X = .0, Y = .0, Width = .0, Height = .0 };

        private static void SnapToScreenEdge(Window sender)
        {
            // 获取当前窗口的位置和大小
            double windowLeft = sender.Left;
            double windowTop = sender.Top;
            double windowWidth = sender.ActualWidth;
            double windowHeight = sender.ActualHeight;

            // 判断较上一次是否有更新
            if (windowLeft != prevRectangle.X || windowTop != prevRectangle.Y || windowWidth != prevRectangle.Width || windowHeight != prevRectangle.Height)
            {
                // 获取当前窗口所在的工作区大小（不包括任务栏）
                IntPtr hwnd = new WindowInteropHelper(sender).Handle;
                var workArea = System.Windows.Forms.Screen.FromHandle(hwnd).WorkingArea;

                // 计算边缘吸附的阈值范围
                double snapLeft = workArea.Left + GetSnapDistance(sender);
                double snapTop = workArea.Top + GetSnapDistance(sender);
                double snapRight = workArea.Right - windowWidth - GetSnapDistance(sender);
                double snapBottom = workArea.Bottom - windowHeight - GetSnapDistance(sender);

                // 判断窗口是否需要进行边缘吸附
                bool shouldSnap = false;

                if (windowLeft < snapLeft)
                {
                    windowLeft = snapLeft;
                    shouldSnap = true;
                }
                else if (windowLeft > snapRight)
                {
                    windowLeft = snapRight;
                    shouldSnap = true;
                }

                if (windowTop < snapTop)
                {
                    windowTop = snapTop;
                    shouldSnap = true;
                }
                else if (windowTop > snapBottom)
                {
                    windowTop = snapBottom;
                    shouldSnap = true;
                }

                // 如果需要进行边缘吸附，则更新窗口的位置
                if (shouldSnap)
                {
                    sender.Left = windowLeft;
                    sender.Top = windowTop;
                }

                prevRectangle = new WindowRectangle { X = windowLeft, Y = windowTop, Width = windowWidth, Height = windowHeight };
            }
        }
        #endregion

        #region SnapDistance
        public static readonly DependencyProperty SnapDistanceProperty = DependencyProperty.RegisterAttached("SnapDistance", typeof(int), typeof(WindowSnapHelper), new PropertyMetadata(20));

        public static void SetSnapDistance(DependencyObject element, int value) => element.SetValue(SnapDistanceProperty, value);

        public static int GetSnapDistance(DependencyObject element) => (int)element.GetValue(SnapDistanceProperty);
        #endregion
    }
}
