using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 视频号视频下载工具
{
    /* Bob Jenkins's cryptographic random number generators, ISAAC and ISAAC64.

  Copyright (C) 1999-2024 Free Software Foundation, Inc.
  Copyright (C) 1997, 1998, 1999 Colin Plumb.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <https://www.gnu.org/licenses/>.

  Written by Colin Plumb and Paul Eggert.  */

    /*
     * --------------------------------------------------------------------
     * We need a source of random numbers for some data.
     * Cryptographically secure is desirable, but it's not life-or-death
     * so I can be a little bit experimental in the choice of RNGs here.
     *
     * This generator is based somewhat on RC4, but has analysis
     * <https://burtleburtle.net/bob/rand/isaacafa.html>
     * pointing to it actually being better.  I like it because it's nice
     * and fast, and because the author did good work analyzing it.
     * --------------------------------------------------------------------
     */
    using System;

    namespace RandomNumberGenerators
    {
        public class IsaacState
        {

  
    const int ISAAC_BITS_LOG = 6;
 
 


            // State variables for the random number generator.  The M member
            // should be seeded with nonce data before calling isaac_seed.  The
            // other members are private.
            //struct IsaacState
            //{
                
            //    public UIntPtr[] m; // Main state array
            //    public UIntPtr a, b, c; // Extra variables

            //    public IsaacState()
            //    {
            //        m = new UIntPtr[ISAAC_WORDS];
            //    }
            //}

            //public static void IsaacSeed(ref IsaacState state)
            //{
            //    // Implementation of isaac_seed
            //}

            //public static void IsaacRefill(ref IsaacState state, UIntPtr[] buffer)
            //{
            //    // Implementation of isaac_refill
            //}



 
            const int ISAAC_BYTES = ISAAC_WORDS * (ISAAC_BITS / 8);


            private const int ISAAC_BITS = 32;
            private const int ISAAC_WORDS = 256;
            private const int ISAAC_WORDS_LOG = 8;
            private const int HALF = ISAAC_WORDS / 2;

            private uint[] m = new uint[ISAAC_WORDS];
            private uint a, b, c;

            private static uint Just(uint a)
            {
               
               // uint desiredBits = ((uint)1 << 1 << (ISAAC_BITS - 1)) - 1;
                uint desiredBits = ((uint)1 << 1 << 30) - 1;
                return a & desiredBits;
            }

            private static uint Ind(uint[] m, uint x)
            {
                if (sizeof(uint) * 8 == ISAAC_BITS)
                {
                    return m[x & ((ISAAC_WORDS - 1) * sizeof(uint))];
                }
                else
                {
                    return m[(x / (ISAAC_BITS / 8)) & (ISAAC_WORDS - 1)];
                }
            }

            private static void Mix(ref uint a, ref uint b, ref uint c, ref uint d, ref uint e, ref uint f, ref uint g, ref uint h)
            {
                a -= e; f ^= Just(h) >> 9; h += a;
                b -= f; g ^= a << 9; a += b;
                c -= g; h ^= Just(b) >> 23; b += c;
                d -= h; a ^= c << 15; c += d;
                e -= a; b ^= Just(d) >> 14; d += e;
                f -= b; c ^= e << 20; e += f;
                g -= c; d ^= Just(f) >> 17; f += g;
                h -= d; e ^= g << 14; g += h;
            }

            private static void IsaacStep(ref uint a, ref uint b, ref uint c, ref uint d, ref uint e, ref uint f, ref uint g, ref uint h, uint[] m, uint[] r, int i, int off, uint mix)
            {
                uint x, y;
                a = (a ^ mix) + m[off + i];
                x = m[i];
                m[i] = y = Ind(m, x) + a + b;
                r[i] = b = Just(Ind(m, y >> ISAAC_WORDS_LOG) + x);
            }

            private static void IsaacMix(uint[] m, uint[] r)
            {
                uint a, b, c, d, e, f, g, h;
                //a = b = c = d = e = f = g = h = ((ISAAC_BITS == 32) ? 0x9e3779b9 : 0x9e3779b97f4a7c13);
                a = b = c = d = e = f = g = h =  0x9e3779b9 ;
                for (int i = 0; i < 4; i++)
                {
                    Mix(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h);
                }

                for (int i = 0; i < ISAAC_WORDS; i += 8)
                {
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i, HALF, a << 13);
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i + 1, HALF, Just(a) >> 6);
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i + 2, HALF, a << 2);
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i + 3, HALF, Just(a) >> 16);
                }

                for (int i = 0; i < ISAAC_WORDS; i += 8)
                {
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i, -HALF, a << 13);
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i + 1, -HALF, Just(a) >> 6);
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i + 2, -HALF, a << 2);
                    IsaacStep(ref a, ref b, ref c, ref d, ref e, ref f, ref g, ref h, m, r, i + 3, -HALF, Just(a) >> 16);
                }
            }

            public void Seed()
            {
                //uint a = ISAAC_BITS == 32 ? 0x1367df5a : 0x647c4677a2884b7c;
                //uint b = ISAAC_BITS == 32 ? 0x95d90059 : 0xb9f8b322c73ac862;
                //uint c = ISAAC_BITS == 32 ? 0xc3163e4b : 0x8c0ea5053d4712a0;
                //uint d = ISAAC_BITS == 32 ? 0x0f421ad8 : 0xb29b2e824a595524;
                //uint e = ISAAC_BITS == 32 ? 0xd92a4a78 : 0x82f053db8355e0ce;
                //uint f = ISAAC_BITS == 32 ? 0xa51a3c49 : 0x48fe4a0fa5a09315;
                //uint g = ISAAC_BITS == 32 ? 0xc4efea1b : 0xae985bf2cbfc89ed;
                //uint h = ISAAC_BITS == 32 ? 0x30609119 : 0x98f5704f6c44c0ab;
                uint a =  0x1367df5a  ;
                uint b =  0x95d90059  ;
                uint c =  0xc3163e4b  ;
                uint d =  0x0f421ad8  ;
                uint e =  0xd92a4a78  ;
                uint f =  0xa51a3c49  ;
                uint g =  0xc4efea1b  ;
                uint h =  0x30609119  ;

                IsaacMix(m, new uint[ISAAC_WORDS]);
                IsaacMix(m, new uint[ISAAC_WORDS]);

                this.a = this.b = this.c = 0;
            }

            public void Refill(uint[] result)
            {
                uint a = this.a;
                uint b = this.b + (++this.c);
                uint[] m = this.m;
                uint[] r = result;

                IsaacMix(m, r);

                this.a = a;
                this.b = b;
            }
        }

        public class IsaacRandom
        {
            private const int ISAAC_WORDS = 256;
            private const int ISAAC_BYTES = ISAAC_WORDS * sizeof(uint);

            private IsaacState state = new IsaacState();
            private uint[] result = new uint[ISAAC_WORDS];
            private int index = ISAAC_WORDS;

            public IsaacRandom()
            {
                state.Seed();
            }

            public IsaacRandom(uint[] seed)
            {
                state.Seed();
                SetSeed(seed);
            }

            public void SetSeed(uint[] seed)
            {
                if (seed.Length % ISAAC_WORDS != 0)
                {
                    throw new ArgumentException("Seed length must be a multiple of 256");
                }

                for (int i = 0; i < seed.Length; i += ISAAC_WORDS)
                {
                    state.Refill(seed);
                }
            }

            private void Generate()
            {
                if (index >= ISAAC_WORDS)
                {
                    state.Refill(result);
                    index = 0;
                }
            }

            public uint NextUInt()
            {
                Generate();
                return result[index++];
            }

            public int NextInt()
            {
                return (int)NextUInt();
            }

            public int NextInt(int maxValue)
            {
                if (maxValue <= 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue", "maxValue must be positive");
                }

                return (int)(NextUInt() % (uint)maxValue);
            }

            public int NextInt(int minValue, int maxValue)
            {
                if (minValue >= maxValue)
                {
                    throw new ArgumentOutOfRangeException("minValue", "minValue must be less than maxValue");
                }

                return minValue + NextInt(maxValue - minValue);
            }

            public double NextDouble()
            {
                const double maxUInt32 = (double)uint.MaxValue;
                return NextUInt() / (maxUInt32 + 1);
            }

            public double NextDouble(double maxValue)
            {
                if (maxValue <= 0)
                {
                    throw new ArgumentOutOfRangeException("maxValue", "maxValue must be positive");
                }

                return NextDouble() * maxValue;
            }

            public double NextDouble(double minValue, double maxValue)
            {
                if (minValue >= maxValue)
                {
                    throw new ArgumentOutOfRangeException("minValue", "minValue must be less than maxValue");
                }

                return minValue + NextDouble(maxValue - minValue);
            }
        }
    }





}
