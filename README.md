﻿# Native.Minhook

A modern, lightweight, and high-performance C# wrapper for the [MinHook](https://github.com/TsudaKageyu/minhook) library. 

It is designed specifically for **.NET Native AOT** applications, providing seamless static linking and a clean, object-oriented API for native function interception.

## Features
- **Native AOT Ready**: Fully compatible with .NET 8/9 Native AOT compilation.
- **Zero External Dependencies**: Uses static linking. You don't need to ship `minhook.dll` or `minhook.so` alongside your application.
- **Automated x86 Fix**: Includes a built-in compatibility patch (`fix_ultof3.lib`) to resolve MSVC v143 linker issues on 32-bit Windows.
- **Clean API**: Forget about raw `void*` pointers and manual memory management. Manage your hooks easily with the `Hook` class.
- **Cross-Architecture**: Supports both `win-x86` and `win-x64` out of the box.

## Installation

```bash
dotnet add package Native.Minhook
```

*(Note: Your project must have `<PublishAot>true</PublishAot>` enabled to use this package).*

## Publishing
Native AOT requires you to publish the project for a specific architecture. You cannot use dotnet run or dotnet build for testing the statically linked native code. Use one of the following commands:

```bash
# For 64-bit applications
dotnet publish -r win-x64 -c Release

# For 32-bit applications
dotnet publish -r win-x86 -c Release
```

## Quick Start

Here is a basic example of how to intercept a function using `Native.Minhook`:

```csharp
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Native.Minhook;

public static unsafe class Program
{
    private static Hook _hook;

    public static void Main()
    {
        // 1. Initialize MinHook
        if (MinHook.Initialize() != MH_STATUS.MH_OK) return;

        // 2. Create a hook. 
        // You can use an offset (new Hook(0x1234)), 
        // an address (new Hook((nint)0x7FF...)), 
        // or an exported API (new Hook("user32.dll", "MessageBoxA"))
        _hook = new Hook("user32.dll", "MessageBoxA");

        // 3. Apply the detour
        nint detourAddr = (nint)(delegate* unmanaged[Stdcall]<nint, nint, nint, uint, int>)&DetourMessageBox;
        
        if (_hook.Create(detourAddr) == MH_STATUS.MH_OK)
        {
            _hook.Enable();
            Console.WriteLine("Hook successfully installed!");
        }
    }

    // 4. Your detour function
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static int DetourMessageBox(nint hWnd, nint lpText, nint lpCaption, uint uType)
    {
        // Do something before calling the original
        
        // Call the original function
        var original = (delegate* unmanaged[Stdcall]<nint, nint, nint, uint, int>)_hook.OriginalAddress;
        return original(hWnd, lpText, lpCaption, uType);
    }
}
```