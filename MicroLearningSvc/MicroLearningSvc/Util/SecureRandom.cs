using MicroLearningSvc.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MicroLearningSvc.Util
{
    class SecureRandom : ISecureRandom, IDisposable
    {
        readonly RNGCryptoServiceProvider _cryptoServiceProvider;

        public SecureRandom()
        {
            _cryptoServiceProvider = new RNGCryptoServiceProvider();
        }

        public int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
                throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = this.GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private uint GetRandomUInt()
        {
            var randomBytes = this.GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        public byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            _cryptoServiceProvider.GetBytes(buffer);
            return buffer;
        }

        public void Dispose()
        {
            _cryptoServiceProvider.SafeDispose();
        }
    }
}
