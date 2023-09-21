// See https://aka.ms/new-console-template for more information
using Trarizon.Toolkit.Deemo.Algorithm;

Console.WriteLine("Hello, World!");

var offsets = new float[1500];

//offsets[0] = 200;
offsets[1] = 200;

Console.WriteLine(ScoreCalculation.ComboScore(offsets));

//offsets[0] = 0;
offsets[1] = 0;
//offsets[^1] = 200;
offsets[^2] = 200;

Console.WriteLine(ScoreCalculation.ComboScore(offsets));