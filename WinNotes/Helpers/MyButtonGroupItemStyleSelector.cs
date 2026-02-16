using HandyControl.Controls;
using HandyControl.Tools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WinNotes.Helpers
{
    internal class MyButtonGroupItemStyleSelector : ButtonGroupItemStyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (container is ButtonGroup buttonGroup && item is ToggleButton tb)
            {
                var count = buttonGroup.Items.OfType<ButtonBase>().Count(b => b.IsVisible);
                var index = buttonGroup.Items.IndexOf(tb);

                // 返回你的自定义 Style，根据索引和方向
                if (count == 1)
                    return (Style)Application.Current.Resources["MyToggleButtonGroupItemBaseStyle"];

                if (buttonGroup.Orientation == Orientation.Horizontal)
                {
                    if (index == 0) return (Style)Application.Current.Resources["MyToggleButtonGroupItemBaseStyle"];
                    if (index == count - 1) return (Style)Application.Current.Resources["MyToggleButtonGroupItemBaseStyle"];
                }
                else
                {
                    if (index == 0) return (Style)Application.Current.Resources["MyToggleButtonGroupItemBaseStyle"];
                    if (index == count - 1) return (Style)Application.Current.Resources["MyToggleButtonGroupItemBaseStyle"];
                }

                return (Style)Application.Current.Resources["MyToggleButtonGroupItemBaseStyle"];
            }

            // 其他类型按钮仍然用原始逻辑
            return base.SelectStyle(item, container);

        }
    }
}
