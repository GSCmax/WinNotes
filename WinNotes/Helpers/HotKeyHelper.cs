using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace WinNotes.Helpers
{
    public sealed class HotKeyHelper : IDisposable
    {
        private readonly Window _window;

        private readonly int _id;

        private bool _isKeyRegistered;

        private Dispatcher _currentDispatcher;


        HotKeyHelper(ModifierKeys modifierKeys, Key key, Window window, Action<HotKeyHelper> onKeyAction)
        {
            Key = key;
            KeyModifier = modifierKeys;
            _id = GetHashCode();
            _window = window;
            _currentDispatcher = Dispatcher.CurrentDispatcher;
            RegisterHotKey();
            ComponentDispatcher.ThreadPreprocessMessage += ThreadPreprocessMessageMethod;

            if (onKeyAction != null)
                HotKeyPressed += onKeyAction;
        }

        ~HotKeyHelper()
        {
            Dispose();
        }

        public event Action<HotKeyHelper>? HotKeyPressed;

        public Key Key { get; private set; }

        public ModifierKeys KeyModifier { get; private set; }

        private int InteropKey => KeyInterop.VirtualKeyFromKey(Key);

        public void Dispose()
        {
            try
            {
                ComponentDispatcher.ThreadPreprocessMessage -= ThreadPreprocessMessageMethod;
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                UnregisterHotKey();
            }
        }

        private void OnHotKeyPressed()
        {
            _currentDispatcher.Invoke(
                delegate
                {
                    HotKeyPressed?.Invoke(this);
                });
        }

        private void RegisterHotKey()
        {
            if (Key == Key.None)
            {
                return;
            }

            if (_isKeyRegistered)
            {
                UnregisterHotKey();
            }

            _isKeyRegistered = HotKeyWinApi.RegisterHotKey(new WindowInteropHelper(_window).Handle, _id, KeyModifier, InteropKey);

            if (!_isKeyRegistered)
            {
                throw new ApplicationException("An unexpected Error occured! (Hotkey may already be in use)");
            }
        }

        private void ThreadPreprocessMessageMethod(ref MSG msg, ref bool handled)
        {
            if (handled)
            {
                return;
            }

            if (msg.message != HotKeyWinApi.WmHotKey || (int)(msg.wParam) != _id)
            {
                return;
            }

            OnHotKeyPressed();
            handled = true;
        }

        private void UnregisterHotKey()
        {
            _isKeyRegistered = !HotKeyWinApi.UnregisterHotKey(new WindowInteropHelper(_window).Handle, _id);
        }

        private static HotKeyHelper? hks;

        public static bool InitHotKey(string hkStr, Window w, Action<HotKeyHelper> onKeyAction)
        {
            ModifierKeys mks = ModifierKeys.None;
            Key k = Key.None;

            try
            {
                string[] HotKey_ModifierKeys = hkStr.Split(',')[0].Split('+');
                string HotKey_Key = hkStr.Split(',')[1];
                for (int i = 0; i < HotKey_ModifierKeys!.Length; i++)
                    mks |= (ModifierKeys)Enum.Parse(typeof(ModifierKeys), HotKey_ModifierKeys[i]);
                k = (Key)Enum.Parse(typeof(Key), HotKey_Key!);
            }
            catch
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.MessageBox.Show($"Hotkey ({hkStr}) is unrecognizable.\nPlease check the key names.\n\n无法识别热键({hkStr})。\n请检查键名。", "DeepSeekSpirit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                hks = new HotKeyHelper(mks, k, w, onKeyAction);
            }
            catch
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.MessageBox.Show($"Hotkey ({hkStr}) has already been registered.\nYou can not use hotkey to call the sprite currently.\n\n热键({hkStr})已被注册。\n您当前无法使用热键呼出此工具。", "DeepSeekSpirit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        public static void DisposeHotKey()
        {
            hks!.Dispose();
        }
    }

    public class HotKeyWinApi
    {
        public const int WmHotKey = 0x0312;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, ModifierKeys fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

}
