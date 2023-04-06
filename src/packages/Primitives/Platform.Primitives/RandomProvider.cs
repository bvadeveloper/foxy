using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Platform.Primitives
{
    internal static class RandomProvider
    {
        [ThreadStatic]
        static Random random;

        [ThreadStatic]
        static XorShift64 xorShift;

        // this random is async-unsafe, be careful to use.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Random GetRandom()
        {
            if (random == null)
            {
                random = CreateRandom();
            }
            return random;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Random CreateRandom()
        {
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[sizeof(int)];
            rng.GetBytes(buffer);
            var seed = BitConverter.ToInt32(buffer, 0);
            
            return new Random(seed);
        }

        // this random is async-unsafe, be careful to use.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static XorShift64 GetXorShift64()
        {
            if (xorShift == null)
            {
                xorShift = CreateXorShift64();
            }
            return xorShift;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static XorShift64 CreateXorShift64()
        {
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[sizeof(UInt64)];
            rng.GetBytes(buffer);
            var seed = BitConverter.ToUInt64(buffer, 0);
            
            return new XorShift64(seed);
        }
    }

    internal class XorShift64
    {
        private UInt64 _x = 88172645463325252UL;

        public XorShift64(UInt64 seed)
        {
            if (seed != 0)
            {
                _x = seed;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UInt64 Next()
        {
            _x = _x ^ (_x << 7);
            return _x = _x ^ (_x >> 9);
        }
    }
}