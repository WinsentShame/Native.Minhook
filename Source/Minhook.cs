// Copyright (c) 2026 WinsentShame
// SPDX-License-Identifier: MIT
// See LICENSE.txt for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Native.Minhook;

public enum MH_STATUS
{
    MH_OK = 0,
    MH_ERROR_ALREADY_INITIALIZED,
    MH_ERROR_NOT_INITIALIZED,
    MH_ERROR_ALREADY_CREATED,
    MH_ERROR_NOT_CREATED,
    MH_ERROR_ENABLED,
    MH_ERROR_DISABLED,
    MH_ERROR_NOT_EXECUTABLE,
    MH_ERROR_UNSUPPORTED_FUNCTION,
    MH_ERROR_MEMORY_ALLOC,
    MH_ERROR_MEMORY_PROTECT,
    MH_ERROR_MODULE_NOT_FOUND,
    MH_ERROR_FUNCTION_NOT_FOUND
}

public static unsafe partial class MinHook
{
    private const string Lib = "Minhook";

    [LibraryImport(Lib, EntryPoint = "MH_Initialize")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial MH_STATUS Initialize();

    [LibraryImport(Lib, EntryPoint = "MH_Uninitialize")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial MH_STATUS Uninitialize();

    [LibraryImport(Lib, EntryPoint = "MH_CreateHook")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial MH_STATUS CreateHook(nint target, nint detour, out nint original);

    [LibraryImport(Lib, EntryPoint = "MH_EnableHook")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial MH_STATUS EnableHook(nint target);

    [LibraryImport(Lib, EntryPoint = "MH_DisableHook")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial MH_STATUS DisableHook(nint target);

    [LibraryImport(Lib, EntryPoint = "MH_RemoveHook")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial MH_STATUS RemoveHook(nint target);
}