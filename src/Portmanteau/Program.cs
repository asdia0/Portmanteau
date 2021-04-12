namespace Portmanteau
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    class Program
    {
        static string uniquePath = "resources/unique_words.txt";
        static string allPath = "resources/all_words.txt";
        static string portPath = "resources/portmanteau.txt";

        static void Main(string[] args)
        {
            //FindUniqueWords();
            GetPortmanteau();
        }

        static string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        static void FindUniqueWords()
        {
            List<string> words = ReadFile(allPath).Split("\r\n").ToList();

            // Sort by decreasing length
            words.Sort((x, y) => x.Length.CompareTo(y.Length));
            words.Reverse();

            int index = 0;
            
            while (index < words.Count)
            {
                string entry = words[index];

                if (entry.Length == 2)
                {
                    index++;
                    continue;
                }

                foreach (string word in words.ToList())
                {
                    if (word.Length < entry.Length && word != entry && entry.Contains(word))
                    {
                        words.Remove(word);
                    }
                }

                index++;
            }

            File.Delete(uniquePath);
            File.WriteAllLines(uniquePath, words.ToArray());
        }

        static string CreatePormanteau(string a, string b)
        {
            // a--b
            string ab = string.Empty;

            for (int index = 0; index < Math.Min(a.Length, b.Length); index++)
            {
                if (a[(a.Length - index - 1)..a.Length] == b[0..(index + 1)])
                {
                    ab =b[0..(index + 1)];
                }
            }

            // b--a
            string ba = string.Empty;

            for (int index = 0; index < Math.Min(a.Length, b.Length); index++)
            {
                if (b[(b.Length - index - 1)..b.Length] == a[0..(index + 1)])
                {
                    ba = a[0..(index + 1)];
                }
            }

            // Return smallest superstring
            if (ab.Length >= ba.Length)
            {
                return a + b.Remove(0, ab.Length);
            }

            return b + a.Remove(0, ba.Length);
        }

        static int GetOverlapCount(string a, string b)
        {
            // a--b
            string ab = a + b;

            for (int index = 0; index < Math.Min(a.Length, b.Length); index++)
            {
                if (a[(a.Length - index - 1)..a.Length] == b[0..(index + 1)])
                {
                    ab = b[0..(index + 1)];
                }
            }

            // b--a
            string ba = b + a;

            for (int index = 0; index < Math.Min(a.Length, b.Length); index++)
            {
                if (b[(b.Length - index - 1)..b.Length] == a[0..(index + 1)])
                {
                    ba = a[0..(index + 1)];
                }
            }

            // Return smallest superstring
            if (ab.Length < ba.Length)
            {
                return ab.Length;
            }

            return ba.Length;
        }

        static void GetPortmanteau()
        {
            HashSet<string> words = ReadFile(uniquePath).Split("\r\n").ToHashSet();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (words.Count > 1)
            {
                File.Delete(portPath);
                File.WriteAllLines(portPath, words.ToArray());
                Console.WriteLine($"Entries remaining: {words.Count} ({stopwatch.Elapsed.Hours}h {stopwatch.Elapsed.Minutes}m {stopwatch.Elapsed.Seconds}s)");

                (string, string) mostOverlapped = (string.Empty, string.Empty);
                int overlapCount = 0;

                foreach (string a in words)
                {
                    if (a.Length > overlapCount)
                    {
                        foreach (string b in words)
                        {
                            if (b.Length > overlapCount && a != b)
                            {
                                int overlap = GetOverlapCount(a, b);
                                if (overlap > overlapCount)
                                {
                                    overlapCount = overlap;
                                    mostOverlapped.Item1 = a;
                                    mostOverlapped.Item2 = b;
                                }
                            }
                        }
                    }
                }

                words.Remove(mostOverlapped.Item1);
                words.Remove(mostOverlapped.Item2);
                words.Add(CreatePormanteau(mostOverlapped.Item1, mostOverlapped.Item2));
            }

            Console.WriteLine("Done");
            File.Delete(portPath);
            File.WriteAllLines(portPath, words.ToArray());
        }
    }
}
