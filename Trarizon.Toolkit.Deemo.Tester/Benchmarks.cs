using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Tester;
[MemoryDiagnoser]
public class Benchmarks
{
	public static void Run() => BenchmarkRunner.Run<Benchmarks>();

	private Chart chart = Chart.FromJson(File.ReadAllText(@"D:\Project Charts\Deenote\Music\&Deemo\5.Elysian Volitation.hard.txt"));


	[Benchmark, Arguments(false), Arguments(true)]
	public Chart List(bool fixRange)
	{
		const float DefaultSize = 0.2f;

		List<(float Time, PianoSound Sound)> sounds
			= chart.Notes.SelectMany(n => n.Sounds.Select(s => (n.Time, s))).ToList();

		if (!sounds.Any())
			return new Chart(chart, false);

		int minp, maxp;
		if (fixRange)
			(minp, maxp) = (PianoSound.PitchMin88, PianoSound.PitchMax88);
		else {
			(minp, maxp) = (int.MaxValue, int.MinValue);
			foreach (var (_, sound) in sounds) {
				if (sound.Pitch < minp)
					minp = sound.Pitch;
				if (sound.Pitch > maxp)
					maxp = sound.Pitch;
			}
		}

		Chart atarashii = new(chart, false);
		atarashii.Notes.AddRange(sounds.Select(s => new Note(
				position: (s.Sound.Pitch - (minp + maxp) / 2) / ((maxp - minp) / 4f),
				size: DefaultSize,
				time: s.Time + s.Sound.Delay,
				sounds: new List<PianoSound>(1) { new PianoSound(s.Sound) },
				duration: s.Sound.Duration)));
		return atarashii;
	}

	[Benchmark, Arguments(false), Arguments(true)]
	public Chart Array(bool fixRange)
	{
		const float DefaultSize = 0.2f;

		(float Time, PianoSound Sound)[] sounds
			= chart.Notes.SelectMany(n => n.Sounds.Select(s => (n.Time, s))).ToArray();

		if (!sounds.Any())
			return new Chart(chart, false);

		int minp, maxp;
		if (fixRange)
			(minp, maxp) = (PianoSound.PitchMin88, PianoSound.PitchMax88);
		else {
			(minp, maxp) = (int.MaxValue, int.MinValue);
			foreach (var (_, sound) in sounds) {
				if (sound.Pitch < minp)
					minp = sound.Pitch;
				if (sound.Pitch > maxp)
					maxp = sound.Pitch;
			}
		}

		Chart atarashii = new(chart, false);
		atarashii.Notes.AddRange(sounds.Select(s => new Note(
				position: (s.Sound.Pitch - (minp + maxp) / 2) / ((maxp - minp) / 4f),
				size: DefaultSize,
				time: s.Time + s.Sound.Delay,
				sounds: new List<PianoSound>(1) { new PianoSound(s.Sound) },
				duration: s.Sound.Duration)));
		return atarashii;
	}

	[Benchmark, Arguments(false), Arguments(true)]
	public Chart Enumerable(bool fixRange)
	{
		const float DefaultSize = 0.2f;

		IEnumerable<(float Time, PianoSound Sound)> sounds
			= chart.Notes.SelectMany(n => n.Sounds.Select(s => (n.Time, s)));

		if (!sounds.Any())
			return new Chart(chart, false);

		int minp, maxp;
		if (fixRange)
			(minp, maxp) = (PianoSound.PitchMin88, PianoSound.PitchMax88);
		else {
			(minp, maxp) = (int.MaxValue, int.MinValue);
			foreach (var (_, sound) in sounds) {
				if (sound.Pitch < minp)
					minp = sound.Pitch;
				if (sound.Pitch > maxp)
					maxp = sound.Pitch;
			}
		}

		Chart atarashii = new(chart, false);
		atarashii.Notes.AddRange(sounds.Select(s => new Note(
				position: (s.Sound.Pitch - (minp + maxp) / 2) / ((maxp - minp) / 4f),
				size: DefaultSize,
				time: s.Time + s.Sound.Delay,
				sounds: new List<PianoSound>(1) { new PianoSound(s.Sound) },
				duration: s.Sound.Duration)));
		return atarashii;
	}
}
