using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Tabloulet.Helpers
{
    public partial class RFID : Node
    {
        public static async Task<Guid> GetUIDAsync(Guid idScenario = default)
        {
            using var process = new Process();
            process.StartInfo.FileName = "bash";
            process.StartInfo.Arguments = "-c \"nfc-list | grep UID | cut -d':' -f2 | tr -d ' '\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;

            process.Start();
            string hexInput = await process.StandardOutput.ReadToEndAsync();
            hexInput = hexInput.Trim();

            hexInput = new string(
                hexInput.Where(c => "0123456789abcdefABCDEF".Contains(c)).ToArray()
            );

            if (hexInput.Length > 32)
                hexInput = hexInput[32..];
            else
                hexInput = hexInput.PadRight(32, '0');

            string formattedGuid = string.Format(
                "{0}-{1}-{2}-{3}-{4}",
                hexInput[..8],
                hexInput.Substring(8, 4),
                hexInput.Substring(12, 4),
                hexInput.Substring(16, 4),
                hexInput.Substring(20, 12)
            );

            Guid output = new(formattedGuid);
            await process.WaitForExitAsync();

            return XORGuids(output, idScenario);
        }

        private static Guid XORGuids(Guid guid1, Guid guid2)
        {
            byte[] bytes1 = guid1.ToByteArray();
            byte[] bytes2 = guid2.ToByteArray();
            byte[] result = new byte[bytes1.Length];

            for (int i = 0; i < bytes1.Length; i++)
            {
                result[i] = (byte)(bytes1[i] ^ bytes2[i]);
            }

            return new Guid(result);
        }
    }
}
