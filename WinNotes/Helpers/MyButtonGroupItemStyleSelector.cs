using HandyControl.Controls;
using HandyControl.Tools;
using System.Windows;
using System.Windows.Controls.Primitives;
using Application = System.Windows.Application;

namespace WinNotes.Helpers
{
    internal class MyButtonGroupItemStyleSelector : ButtonGroupItemStyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (container is ButtonGroup buttonGroup && item is ToggleButton tb)
            {
                var count = buttonGroup.Items.OfType<System.Windows.Controls.Primitives.ButtonBase>().Count(b => b.IsVisible);
                var index = buttonGroup.Items.IndexOf(tb);

                // 返回你的自定义 Style，根据索引和方向
                if (count == 1)
                    return (Style)Application.Current.Resources["MyToggleButtonGroupItemSingle"];

                if (buttonGroup.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    if (index == 0) return (Style)Application.Current.Resources["MyToggleButtonGroupItemHorizontalFirst"];
                    if (index == count - 1) return (Style)Application.Current.Resources["MyToggleButtonGroupItemHorizontalLast"];
                }
                else
                {
                    if (index == 0) return (Style)Application.Current.Resources["MyToggleButtonGroupItemVerticalFirst"];
                    if (index == count - 1) return (Style)Application.Current.Resources["MyToggleButtonGroupItemVerticalLast"];
                }

                return (Style)Application.Current.Resources["MyToggleButtonGroupItemBaseStyle"];
            }

            // 其他类型按钮仍然用原始逻辑
            return base.SelectStyle(item, container);
        }
    }
}
