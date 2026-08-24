#if UNITY_EDITOR
using System.Diagnostics;
using UnityEngine;

namespace ConfigFramework.Editor
{
    /// <summary>
    /// Luban 导出封装（Editor）：调 Luban.ClientServer 生成 C# 代码 + protobuf 数据。
    /// </summary>
    public static class ConfigExportTool
    {
        /// <summary>执行 Luban 导出。</summary>
        /// <param name="lubanDll">Luban.ClientServer.dll 路径</param>
        /// <param name="rootDef">表结构定义根目录（schema）</param>
        /// <param name="excelDir">Excel 源表目录</param>
        /// <param name="codeDir">生成代码输出目录</param>
        /// <param name="dataDir">生成数据输出目录</param>
        public static void Export(string lubanDll, string rootDef, string excelDir, string codeDir, string dataDir)
        {
            string args = $"-j cfg -- -d \"{rootDef}\" --input_data_dir \"{excelDir}\" " +
                          $"--output_code_dir \"{codeDir}\" --output_data_dir \"{dataDir}\" " +
                          $"--gen_types code_protobuf3,data_protobuf_bin -s client";

            var psi = new ProcessStartInfo("dotnet", $"\"{lubanDll}\" {args}")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using (Process p = Process.Start(psi))
            {
                if (p == null)
                {
                    UnityEngine.Debug.LogError("[ConfigExport] 无法启动 Luban 导出进程（请确认已安装 .NET 运行时）");
                    return;
                }
                string log = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
                p.WaitForExit();

                if (p.ExitCode != 0)
                    UnityEngine.Debug.LogError("[ConfigExport] Luban 导出失败:\n" + log);
                else
                {
                    UnityEngine.Debug.Log("[ConfigExport] Luban 导出成功");
                    UnityEditor.AssetDatabase.Refresh();
                }
            }
        }
    }
}
#endif
