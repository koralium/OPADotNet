using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OPADotNet.Embedded.Internal
{
    internal static class RegoWrapper
    {
        static RegoWrapper()
        {
        }

        [DllImport("regosdk", EntryPoint = "CompileModules", CharSet = CharSet.Ansi)]
        public static extern int CompileModules(string modules);

        [DllImport("regosdk", EntryPoint = "RemoveCompiler")]
        public static extern void RemoveCompiler(int compilerId);

        public delegate void PointerCallbackDelegate(int poiner, string error);

        [DllImport("regosdk", EntryPoint = "PreparePartial", CharSet = CharSet.Ansi)]
        public static extern int PreparePartial(int compilerId, int storeId, string query);

        [DllImport("regosdk", EntryPoint = "RemovePartialQuery")]
        public static extern void RemovePartialQuery(int partialQueryId);

        public delegate void EvaluateCallbackDelegate(string val);

        [DllImport("regosdk", EntryPoint = "PreparedPartial", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PreparedPartial(int partialQueryId, string input, string[] unknowns, int unknownsLength);

        [DllImport("regosdk", EntryPoint = "NewStore")]
        public static extern int NewStore();

        [DllImport("regosdk", EntryPoint = "RemoveStore")]
        public static extern void RemoveStore(int storeId);

        [DllImport("regosdk", EntryPoint = "WriteToStore", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WriteToStore(int storeId, int transactionId, string path, string input);

        [DllImport("regosdk", EntryPoint = "ReadFromStore", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadFromStore(int storeId, int transactionId, string path);

        public delegate void TransactionCallbackDelegate(int transactionId, string error);

        [DllImport("regosdk", EntryPoint = "NewTransaction", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NewTransaction(int storeId);

        [DllImport("regosdk", EntryPoint = "RemoveTransaction")]
        public static extern void RemoveTransaction(int transactionId);

        public delegate void ErrorCallbackDelegate(string error);

        [DllImport("regosdk", EntryPoint = "CommitTransaction")]
        public static extern int CommitTransaction(int storeId, int transactionId);

        [DllImport("regosdk", EntryPoint = "UpsertPolicy")]
        public static extern int UpsertPolicy(int storeId, int transactionId, string policyName, string module);

        [DllImport("regosdk", EntryPoint = "FreeString")]
        public static extern void FreeString(int index);

        [DllImport("regosdk", EntryPoint = "GetModules")]
        public static extern int GetModules(int compilerId);


        [DllImport("regosdk", EntryPoint = "GetString", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetStringInternal(int index);

        public static string GetString(int index)
        {
            var ptr = GetStringInternal(index);
            var content = Marshal.PtrToStringAnsi(ptr);
            FreeString(index);
            return content;
        }
    }
}
