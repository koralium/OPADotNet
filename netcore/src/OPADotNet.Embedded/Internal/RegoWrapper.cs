/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using OPADotNet.Ast;
using OPADotNet.Ast.Models;
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

        [DllImport("regosdk", EntryPoint = "CompilePolicy", CharSet = CharSet.Ansi)]
        public static extern int CompilePolicy(string fileName, string rawText);

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

        [DllImport("regosdk", EntryPoint = "PrepareEvaluation", CharSet = CharSet.Ansi)]
        public static extern int PrepareEvaluation(int compilerId, int storeId, string query);

        [DllImport("regosdk", EntryPoint = "RemoveEvalQuery")]
        public static extern void RemoveEvalQuery(int evalQueryId);

        [DllImport("regosdk", EntryPoint = "PreparedEval", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PreparedEval(int partialQueryId, string input);

        [DllImport("regosdk", EntryPoint = "NewStore")]
        public static extern int NewStore();

        [DllImport("regosdk", EntryPoint = "RemoveStore")]
        public static extern void RemoveStore(int storeId);

        [DllImport("regosdk", EntryPoint = "WriteToStore", CallingConvention = CallingConvention.Cdecl)]
        public static extern int WriteToStore(int storeId, int transactionId, string path, string input);

        [DllImport("regosdk", EntryPoint = "ReadFromStore", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadFromStore(int storeId, int transactionId, string path);

        public delegate void TransactionCallbackDelegate(int transactionId, string error);

        [DllImport("regosdk", EntryPoint = "NewTransaction", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NewTransaction(int storeId, int write);

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

        private delegate string FunctionCallbackDelegate(string json);

        [DllImport("regosdk", EntryPoint = "RegisterFunction1", CharSet = CharSet.Ansi)]
        private static extern void RegisterFunction1Internal(string name, FunctionCallbackDelegate callback);

        public delegate AstTerm Function1Resolver(AstTerm astTerm);

        public static void RegisterFunction1(string name, Function1Resolver resolver)
        {
            RegisterFunction1Internal(name, (json) =>
            {
                var term = PartialJsonConverter.ReadTerm(json);
                return PartialJsonConverter.SerializeTerm(resolver(term));
            });
        }

        public delegate string StoreResolver(string path);

        [DllImport("regosdk", EntryPoint = "RegisterRemoteStore", CharSet = CharSet.Ansi)]
        public static extern void RegisterRemoteStore(int storeId, string path, StoreResolver storeResolver);
    }
}
