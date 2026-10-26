// Copyright (c) 2026 WinsentShame
// SPDX-License-Identifier: MIT
// See LICENSE.txt for full license information.

using System.Runtime.InteropServices;

namespace Native.Minhook;

public unsafe class Hook
{
    public nint TargetAddress { get; }
    public nint OriginalAddress { get; private set; }
    public bool IsCreated { get; private set; }
    public bool IsEnabled { get; private set; }

    public Hook(nint target)
    {
        TargetAddress = target;
    }

    public Hook(int offset) : this(GetModuleHandle(null) + offset) { }

    public Hook(string moduleName, int offset)
    {
        nint hModule = GetModuleHandle(moduleName);
        if (hModule == 0) throw new DllNotFoundException($"Модуль '{moduleName}' не загружен.");
        TargetAddress = hModule + offset;
    }

    public Hook(string moduleName, string exportName)
    {
        nint hModule = GetModuleHandle(moduleName);
        if (hModule == 0)
        {
            hModule = LoadLibrary(moduleName);
            if (hModule == 0) throw new DllNotFoundException($"Не удалось загрузить '{moduleName}'.");
        }

        nint address = GetProcAddress(hModule, exportName);
        if (address == 0) throw new EntryPointNotFoundException($"Функция '{exportName}' не найдена.");

        TargetAddress = address;
    }

    public MH_STATUS Create(nint detourAddress)
    {
        if (IsCreated) return MH_STATUS.MH_ERROR_ALREADY_CREATED;

        var status = MinHook.CreateHook(TargetAddress, detourAddress, out nint original);
        if (status == MH_STATUS.MH_OK)
        {
            OriginalAddress = original;
            IsCreated = true;
        }
        return status;
    }

    public MH_STATUS Enable()
    {
        if (!IsCreated) return MH_STATUS.MH_ERROR_NOT_CREATED;
        var status = MinHook.EnableHook(TargetAddress);
        if (status == MH_STATUS.MH_OK) IsEnabled = true;
        return status;
    }

    public MH_STATUS Disable()
    {
        if (!IsCreated) return MH_STATUS.MH_ERROR_NOT_CREATED;
        var status = MinHook.DisableHook(TargetAddress);
        if (status == MH_STATUS.MH_OK) IsEnabled = false;
        return status;
    }

    public MH_STATUS Remove()
    {
        if (!IsCreated) return MH_STATUS.MH_ERROR_NOT_CREATED;
        var status = MinHook.RemoveHook(TargetAddress);
        if (status == MH_STATUS.MH_OK)
        {
            IsCreated = false;
            IsEnabled = false;
            OriginalAddress = 0;
        }
        return status;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint GetModuleHandle(string? lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern nint GetProcAddress(nint hModule, string lpProcName);
}