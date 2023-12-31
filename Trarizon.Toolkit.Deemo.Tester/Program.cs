// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Runtime.InteropServices;
using Trarizon.Toolkit.Deemo.Algorithm;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Toolkit.Deemo.Tester;

if (Chart.TryParseFromJson(File.ReadAllText(@"D:\Project Charts\Deemo's Note Memory\music\Deemo\No.5-2.N.M.S.T. collection #2\5.Fur War, Pur War.hard.txt"), out var chart)) {
	Console.WriteLine("Loaded");

	var notes = chart.Notes.Where(n => n.IsVisible).Select(n => n.IsSlide ? 0f : 200f).ToList();
	var score = ScoreCalculation.Calc(CollectionsMarshal.AsSpan(notes));
	Console.WriteLine(score);

	var visibleNotes = chart.Notes.Where(n => n.IsVisible).ToArray();

	var slides = visibleNotes.Aggregate((0, 0, 0, 0), ((int Index, int Count, int TailIndex, int MaxCount) aggr, Note n) =>
	{
		if (n.IsSlide) {
			aggr.Count++;
		}
		else {
			if (aggr.MaxCount < aggr.Count) {
				aggr.MaxCount = int.Max(aggr.MaxCount, aggr.Count);
				aggr.TailIndex = aggr.Index;
			}
			aggr.Count = 0;
		}
		aggr.Index++;
		return aggr;
	}, aggr => (aggr.Item3 - aggr.Item4)..aggr.Item3);

	Console.WriteLine((visibleNotes.AsSpan(slides).ToArray().All(n => n.IsSlide)));

	Console.WriteLine(visibleNotes.AsSpan(slides).Length);


	var paint = ChartPainter.Paint(visibleNotes.AsSpan(slides).ToArray());
	paint.Snapshot().Encode().SaveTo(File.OpenWrite(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Img.png")));
}


// Benchmarks.Run();
//if (Chart.TryParseFromJson(File.ReadAllText(@"D:\Project Charts\Deenote\Music\&Deemo\5.Elysian Volitation.hard.txt"), out var chart)) {
//	Console.WriteLine("Loaded");
//	File.WriteAllText(@"D:\Project Charts\Deenote\Music\&Deemo\5.Elysian Volitation.hard.json", chart.ToJson(Trarizon.Toolkit.Deemo.ChartVersion.DeemoV2));
//	//Console.WriteLine(Chart.TryParseFromJson(chart.ToJson(),out _));
//}
//else Console.WriteLine("failed");
