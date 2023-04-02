using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace BigNums
{
    class Program
    {
        static void Main(string[] args)
        {
            var A = new Client_A();
            var B = new Client_B();
            A.GenerateA();
            A.GenerateP();
            A.GenerateG();
            A.GenerateX();
            A.SendOpenData(B);

            B.GenerateB();
            B.GenerateY();
            B.SendGeneratedY(A);

            A.GenerateAKey();


            BigInteger bKey = B.GenerateBKey();
            BigInteger aKey = A.GenerateAKey();
            if (aKey == bKey)
            {
                Console.WriteLine("Ключи совпали!");
            }
            else
                Console.WriteLine("Ключи различны!");
            Console.WriteLine("Ключ А: " + aKey);
            Console.WriteLine("Ключ B: " + bKey);
        }

        public class Client_A
        {
            private BigInteger a;
            public BigInteger p;
            public BigInteger g;
            public BigInteger X;
            public BigInteger Y;

            public BigInteger GenerateAKey()
            {
                return BigMath.Pow(Y, a, p);
            }

            public void GenerateX()
            {
                X = BigMath.Pow(g, a, p);
            }

            public void GenerateA()
            {
                a = BigMath.GetBigPrime(100);
            }
            public void GenerateP()
            {
                p = BigMath.GetBigPrime(300);
            }
            public void GenerateG()
            {
                while (true)
                {
                    g = BigMath.GetBigPrime(10);
                    if (BigMath.Pow(g, p-1, p) == 1)
                        break;
                }
            }

            public void SendOpenData(Client_B B)
            {
                B.X = this.X;
                B.g = this.g;
                B.p = this.p;
            }
        }

        public class Client_B
        {
            private BigInteger b;
            public BigInteger X;
            public BigInteger Y;
            public BigInteger g;
            public BigInteger p;

            public void GenerateB()
            {
                b = BigMath.GetBigPrime(100);
            }

            public void GenerateY()
            {
                Y = BigMath.Pow(g, b, p);
            }
            public void SendGeneratedY(Client_A A)
            {
                A.Y = this.Y;
            }

            public BigInteger GenerateBKey()
            {
                return BigMath.Pow(X, b, p);
            }

        }

        public class Sender
        {
            public BigInteger n;
            public BigInteger fi;
            public BigInteger e;
            public BigInteger d;
            public void SetNFi()
            {
                BigInteger p = BigMath.GetBigPrime(1024);
                BigInteger q = BigMath.GetBigPrime(1024);
                this.n = p * q;
                this.fi = (p - 1)*(q - 1);
                Console.WriteLine(p);
                Console.WriteLine("НОВАЯ СТРОКА");
                //Console.WriteLine(BigMath.FermaTest(q));
                GenerateE();
                Console.WriteLine("Вот ниже");
                this.d = BigMath.modInverse(e, fi);
                var message = 13;
                BigInteger c = BigMath.Pow(message, e, n);
                BigInteger m = BigMath.Pow(c, d, n);
                Console.WriteLine(c);
                Console.WriteLine(m);
            }

            public void GenerateE()
            {
                Random r = new Random();
                int decimals = r.Next(1, 2048);
                while ((e = BigMath.GetBigPrime(decimals)) > fi)
                {
                    e = BigMath.GetBigPrime(decimals);
                }
                while (BigMath.NOD(e, fi) != 1)
                {
                    e++;
                }
                Console.WriteLine(e);
            }
        }

        public static class BigMath
        {
            public static BigInteger modInverse(BigInteger a, BigInteger m)
            {
                BigInteger m0 = m;
                BigInteger y = 0, x = 1;
                if (m == 1 || NOD(a, m) != 1)
                    return 0;
                BigInteger q;
                BigInteger t;
                while (a > 1)
                {
                    // q является частным
                    q = a / m;
                    t = m;
                    // m осталось, процесс
                    // То же, что и алгоритм Евклида
                    m = a % m;
                    a = t;
                    t = y;
                    // Обновляем x и y
                    y = x - q * y;
                    x = t;
                }
                // Сделать х положительным
                if (x < 0)
                    x += m0;
                return x;
            }

            public static BigInteger modInverse2(BigInteger e, BigInteger phi)
            {
                BigInteger phi0 = phi;
                BigInteger d1, d2, q, r, d = 0;
                d1 = 1;
                d2 = 0;
                if (e == 0)
                {
                    return d;
                }
                while (phi > 1)
                {
                    q = e / phi;
                    r = e % phi;

                    d = d1 - q * d2;

                    d1 = d2;
                    d2 = d;

                    e = phi;
                    phi = r;
                }
                if (d < 0)
                {
                    d += phi0;
                }
                return d;
            }

            public static BigInteger NOD(BigInteger a, BigInteger b)
            {
                while (a != 0 && b != 0)
                {
                    if (a > b)
                    {
                        a = a % b;
                    }
                    else
                    {
                        b = b % a;
                    }
                }
                return a + b;
            }

            public static BigInteger GetBigE(int dec)
            {
                Random r = new Random();
                BigInteger a = 0;
                a += Pow(2, dec);
                for (int i = dec - 1; i >= 0; i--)
                {
                    if (r.Next(0, 2) == 1)
                        a += Pow(2, i);
                }
                return a;
            }

            public static BigInteger GetBigPrime(int dec)
            {
                Random r = new Random();
                BigInteger a = 0;
                BigInteger max = Pow(10, dec);
                for (int i = 0; Pow(2, i) < max; i++)
                {
                    if (r.Next(0, 2) == 1)
                    {
                        a += Pow(2, i);
                    } 
                }
                a += max;
                while (!FermaTest(a, 10))
                {
                    a++;
                }
                return a;
            }

            public static BigInteger Pow(BigInteger a, BigInteger n, BigInteger module)
            {
                BigInteger res = 1;
                while (n > 0)
                {
                    if (n % 2 == 0)// если число четное, то 
                    {
                        n /= 2;
                        a *= a;
                        a %= module;
                    }
                    else
                    {
                        n--;
                        res *= a;
                        res %= module;
                    }
                }
                return res % module;
            }

            public static BigInteger Pow(BigInteger a, BigInteger n)
            {
                BigInteger res = 1;
                while (n > 0)
                {
                    if (n % 2 == 0)
                    {
                        n /= 2;
                        a *= a;
                    }
                    else
                    {
                        n--;
                        res *= a;
                    }
                }
                return res;
            }

            public static bool FermaTest(BigInteger a, int tries = 5)
            {
                var sieve = EratosfensSieve(100);
                foreach (var i in sieve)
                {
                    if (NOD(i, a) != 1)
                        return false;
                }
                //Random rand = new Random(DateTime.Now.Second * DateTime.Now.Minute + DateTime.Now.Millisecond);
                Random rand = new Random();
                for (int i = 0; i < tries; i++)
                {
                    BigInteger x = (rand.Next() % (a - 4)) + 2;
                    if (NOD(a, x) != 1)
                        return false;
                    if (Pow(x, a - 1, a) != 1)
                        return false;
                }
                return true;
            }

            public static List<int> EratosfensSieve(int count)
            {
                var sieve = new List<int>();
                //заполнение списка числами от 2 до n-1
                for (var i = 2; i < count; i++)
                {
                    sieve.Add(i);
                }

                for (var i = 0; i < sieve.Count; i++)
                {
                    for (var j = 2; j < count; j++)
                    {
                        sieve.Remove(sieve[i] * j);
                    }
                }
                return sieve;
            }
        }
    }
}