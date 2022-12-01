using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace EstimatePI
{
    internal static class Program
    {
        static readonly Random random = new Random();
        private static long startTime;
        private static void Main(string[] args)
        {
            const string separator = "----------------------------------------------";
            const double defaultSamples = 1000000;
            const double defaultReport = 50000;
            
            var samples = args.Length >= 1 && double.TryParse(args[0], out var parsedSamples) && parsedSamples >= 1
                ? parsedSamples
                : defaultSamples;
            var reportEvery = args.Length == 2 && double.TryParse(args[1], out var parsedReport) && parsedReport >= 0
                ? parsedReport
                : defaultReport;
            
            Console.Clear();
            Console.WriteLine($"\nPI-Estimation tool with Monte Carlo Algorithm\n" +
                              $"Using this settings: {samples} samples, {reportEvery} report Interval\n" + separator + 
                              $"\nFirst calculating an accuracy estimate of our samples ({samples}). This may take some time.");
            
            
            var estimatedRandomness = ProveRandomness(samples);
            Console.WriteLine($"\nThe accuracy with {samples} samples in randomness is {estimatedRandomness} for this run" +
                              separator +
                              $"\nStarting Estimation on {DateTime.Now}");
            startTime = Environment.TickCount64;
            var estimatedPi = GetPi(sampleSize: samples, reportEvery);
            
            var elapsedTime = Environment.TickCount64 - startTime;
            Console.WriteLine($"\n" + separator +
                              $"\nFinal Estimation of Pi: {estimatedPi} - total duration: {TimeSpan.FromMilliseconds(elapsedTime)}");
        }

        private static double GetPi(double sampleSize, double reportEvery = 1000)
        {
            var results = new List<double>();
            
            const int radius = 100000;
            var outerSquareArea = Math.Pow(radius, 2);

            var pointOutOfCircleCounter = 0;
            for (var i = 1; i <= sampleSize; i++)
            {
                var pointX = random.Next(0, radius+1);
                var pointY = random.Next(0, radius+1);

                var innerRectDiag = Math.Sqrt(Math.Pow(pointX, 2) + Math.Pow(pointY, 2));
                if (innerRectDiag > radius)
                    pointOutOfCircleCounter++;
                
                var outOfCirclePercentage = (double)pointOutOfCircleCounter/i * 100;
                var innerCircleArea = (outOfCirclePercentage == 0) ? outerSquareArea : (outerSquareArea - (outerSquareArea / 100 * outOfCirclePercentage)) * 4;
                
                var pi = innerCircleArea / Math.Pow(radius, 2);
                results.Add(pi);

                if (i % reportEvery != 0) continue;

                var elapsedTime = TimeSpan.FromMilliseconds(Environment.TickCount64 - startTime);
                Console.WriteLine($"The average pi-estimation is {results.Average()}, operation running for {elapsedTime:hh\\:mm\\:ss\\:ffff}(HH:MM:SS:mm). Sample {i} out of {sampleSize}.");
            }
            
            return results.Average();
        }

        private static double ProveRandomness(double samples)
        {
            var zeroCounter = 0;
            for (var i = 0; i < samples; i++)
            {
                zeroCounter = (random.Next(0, 2) % 2 == 0) ? zeroCounter+1 : zeroCounter;
            }

            return (double)zeroCounter * 100 / samples;
        }
    }
}