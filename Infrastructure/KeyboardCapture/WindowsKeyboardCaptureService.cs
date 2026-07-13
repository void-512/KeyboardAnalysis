using KeyboardAnalysis.Application.Interfaces;
using KeyboardAnalysis.Domain.Models;

namespace KeyboardAnalysis.Infrastructure.KeyboardCapture;

/// <summary>Captures completed key presses using the Windows low-level keyboard hook.</summary>
public sealed class WindowsKeyboardCaptureService : IKeyboardCaptureService
{
    private const int WhKeyboardLl = 13;
    private const int WmKeyDown = 0x0100;
    private const int WmKeyUp = 0x0101;
    private const int WmSysKeyDown = 0x0104;
    private const int WmSysKeyUp = 0x0105;
    private const uint WmQuit = 0x0012;
    private const uint LlkhfInjected = 0x10;

    private readonly Dictionary<uint, PressDetails> _pressedKeys = new();
    private readonly object _stateLock = new();
    private readonly HookProcedure _hookProcedure;
    private IntPtr _hookHandle;
    private Thread? _hookThread;
    private uint _hookThreadId;
    private TaskCompletionSource? _startupCompletion;

    public WindowsKeyboardCaptureService() => _hookProcedure = HookCallback;

    public event EventHandler<KeyEvent>? KeyEventCaptured;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        lock (_stateLock)
        {
            if (_hookThread is not null)
            {
                return Task.CompletedTask;
            }

            _startupCompletion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _hookThread = new Thread(RunMessageLoop) { IsBackground = true, Name = "Keyboard capture hook" };
            _hookThread.Start();
            return _startupCompletion.Task.WaitAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        Thread? hookThread;
        uint threadId;
        lock (_stateLock)
        {
            hookThread = _hookThread;
            threadId = _hookThreadId;
        }

        if (hookThread is null)
        {
            return;
        }

        NativeMethods.PostThreadMessage(threadId, WmQuit, UIntPtr.Zero, IntPtr.Zero);
        await Task.Run(hookThread.Join, cancellationToken);
    }

    private void RunMessageLoop()
    {
        _hookThreadId = NativeMethods.GetCurrentThreadId();
        _hookHandle = NativeMethods.SetWindowsHookEx(WhKeyboardLl, _hookProcedure, IntPtr.Zero, 0);

        if (_hookHandle == IntPtr.Zero)
        {
            _startupCompletion?.TrySetException(new InvalidOperationException("Unable to register the Windows keyboard hook."));
            ResetHookState();
            return;
        }

        _startupCompletion?.TrySetResult();
        try
        {
            while (NativeMethods.GetMessage(out _, IntPtr.Zero, 0, 0) > 0) { }
        }
        finally
        {
            NativeMethods.UnhookWindowsHookEx(_hookHandle);
            _hookHandle = IntPtr.Zero;
            _pressedKeys.Clear();
            ResetHookState();
        }
    }

    private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0)
        {
            var keyData = Marshal.PtrToStructure<KbdLlHookStruct>(lParam);
            if ((keyData.Flags & LlkhfInjected) == 0)
            {
                var message = wParam.ToInt32();
                if (message is WmKeyDown or WmSysKeyDown)
                {
                    _pressedKeys.TryAdd(keyData.VirtualKeyCode,
                        new PressDetails(DateTimeOffset.Now, Stopwatch.GetTimestamp(), GetActiveApplication()));
                }
                else if ((message is WmKeyUp or WmSysKeyUp) && _pressedKeys.Remove(keyData.VirtualKeyCode, out var press))
                {
                    var duration = Stopwatch.GetElapsedTime(press.StartTimestamp);
                    KeyEventCaptured?.Invoke(this, new KeyEvent(press.Timestamp, (int)keyData.VirtualKeyCode,
                        duration, press.ActiveApplication));
                }
            }
        }

        return NativeMethods.CallNextHookEx(_hookHandle, code, wParam, lParam);
    }

    private static string GetActiveApplication()
    {
        var windowHandle = NativeMethods.GetForegroundWindow();
        if (windowHandle == IntPtr.Zero)
        {
            return "Unknown";
        }

        NativeMethods.GetWindowThreadProcessId(windowHandle, out var processId);
        try
        {
            using var process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }
        catch (ArgumentException)
        {
            return "Unknown";
        }
        catch (InvalidOperationException)
        {
            return "Unknown";
        }
    }

    private void ResetHookState()
    {
        lock (_stateLock)
        {
            _hookThread = null;
            _hookThreadId = 0;
        }
    }

    private sealed record PressDetails(DateTimeOffset Timestamp, long StartTimestamp, string ActiveApplication);

    private delegate IntPtr HookProcedure(int code, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KbdLlHookStruct
    {
        public uint VirtualKeyCode;
        public uint ScanCode;
        public uint Flags;
        public uint Time;
        public UIntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Message
    {
        public IntPtr WindowHandle;
        public uint Value;
        public UIntPtr WParam;
        public IntPtr LParam;
        public uint Time;
        public int PointX;
        public int PointY;
        public uint Private;
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int hookId, HookProcedure callback, IntPtr module, uint threadId);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hookHandle, int code, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern int GetMessage(out Message message, IntPtr windowHandle, uint minimum, uint maximum);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostThreadMessage(uint threadId, uint message, UIntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr windowHandle, out uint processId);
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
    }
}
