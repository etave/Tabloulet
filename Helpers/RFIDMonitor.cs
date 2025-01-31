using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using PCSC;
using PCSC.Exceptions;
using PCSC.Monitoring;
using PCSC.Utils;

namespace Tabloulet.Helpers
{
    public partial class RFIDMonitor : Node
    {
        private static ISCardContext _isCardContext;
        private ISCardMonitor _monitor;
        private Dictionary<string, Guid> _guids;

        public override void _Ready()
        {
            base._Ready();
            _isCardContext = ContextFactory.Instance.Establish(SCardScope.System);
            _guids = [];
            StartMonitoring();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public void StartMonitoring()
        {
            var readerNames = GetReaderNames();
            if (readerNames == null || readerNames.Length < 1)
            {
                return;
            }

            _monitor = MonitorFactory.Instance.Create(SCardScope.System);
            AttachToAllEvents(_monitor);
            _monitor.Start(readerNames);
        }

        private void AttachToAllEvents(ISCardMonitor monitor)
        {
            monitor.CardInserted += (sender, args) => OnCardInserted(args);
            monitor.CardRemoved += (sender, args) => OnCardRemoved(args);
            monitor.MonitorException += MonitorException;
        }

        private void OnCardInserted(CardStatusEventArgs args)
        {
            if (!_guids.ContainsKey(args.ReaderName))
            {
                _guids.Add(args.ReaderName, GetCardUID(args.ReaderName));
            }
        }

        private void OnCardRemoved(CardStatusEventArgs args)
        {
            _guids.Remove(args.ReaderName);
        }

        private void MonitorException(object sender, PCSCException ex)
        {
            GD.PrintErr(SCardHelper.StringifyError(ex.SCardError));
        }

        private static string[] GetReaderNames()
        {
            using var context = ContextFactory.Instance.Establish(SCardScope.System);
            return context.GetReaders();
        }

        private static Guid GetCardUID(string readerName)
        {
            var reader = _isCardContext.ConnectReader(
                readerName,
                SCardShareMode.Shared,
                SCardProtocol.Any
            );

            using (reader.Transaction(SCardReaderDisposition.Leave))
            {
                byte[] getUidCommand = { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
                byte[] receiveBuffer = new byte[256];
                int receiveLength = receiveBuffer.Length;

                var response = reader.Transmit(
                    SCardPCI.GetPci(reader.Protocol),
                    getUidCommand,
                    getUidCommand.Length,
                    receiveBuffer,
                    receiveLength
                );

                if (receiveLength < 2)
                {
                    return Guid.Empty;
                }

                string uid = BitConverter
                    .ToString(receiveBuffer[..(receiveLength - 2)])
                    .Replace("-", "");

                if (uid.Length < 32)
                {
                    uid = uid.PadRight(32, '0');
                }

                string formattedGuid = string.Format(
                    "{0}-{1}-{2}-{3}-{4}",
                    uid[..8],
                    uid.Substring(8, 4),
                    uid.Substring(12, 4),
                    uid.Substring(16, 4),
                    uid.Substring(20, 12)
                );

                return new Guid(formattedGuid);
            }
        }

        private Guid GetMonitoredGuid(Guid idScenario = default)
        {
            if (_guids.Count == 0)
            {
                return Guid.Empty;
            }

            List<Guid> guids = new(_guids.Values);
            guids.Sort();

            Guid uid = idScenario;
            foreach (var guid in guids)
            {
                uid = XORGuids(uid, guid);
            }

            return uid;
        }

        public async Task<Guid> GetStableMonitoredGuid(
            Guid idScenario = default,
            int checkInterval = 2500,
            int maxAttempts = 5
        )
        {
            Guid previousGuid = Guid.Empty;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                Guid currentGuid = GetMonitoredGuid(idScenario);

                if (currentGuid == previousGuid)
                {
                    return currentGuid;
                }

                previousGuid = currentGuid;
                attempts++;
                await Task.Delay(checkInterval);
            }

            return previousGuid;
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
