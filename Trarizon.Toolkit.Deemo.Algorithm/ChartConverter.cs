using CommunityToolkit.Diagnostics;
using Trarizon.Toolkit.Deemo.Algorithm.Utilities;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Algorithm;
public static class ChartConverter
{
	public static Chart RearrangeByPitch(this Chart chart, bool fixRange = false)
	{
		const float DefaultSize = 0.2f;

		// List<> has a better performance here
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

	public static void RandomNotes(this Chart chart,
		float multiTapTimeThreshold = 60f / 24f / 6f,
		float multiTapPositionThreshold = 1f,
		bool randomSize = false,
		int? randomSeed = null)
	{
		if (multiTapTimeThreshold < 0f) multiTapTimeThreshold = 0f;
		multiTapPositionThreshold = Math.Clamp(multiTapPositionThreshold, 0f, 2f);

		Random rnd = randomSeed is null ? new Random() : new Random(randomSeed.Value);
		Queue<Note> potentialOverlapNotes = new();
		Note prevNote = chart.Notes[0];

		foreach (var note in chart.Notes.Skip(1)) {
			if (!note.IsVisible)
				continue;

			prevNote = RandomInternal(prevNote, note);
			RandomNoteSize(prevNote);
		}

		if (prevNote.IsSlide) Random(prevNote);
		else RandomClick(prevNote);
		RandomNoteSize(prevNote);

		Note RandomInternal(Note note, Note nextNote)
		{
			// If slide, just random[-2, 2]
			if (note.IsSlide) {
				Random(note);
			}
			// If this is click but next is slide, process slide first to avoid overlapping
			else if (nextNote.IsSlide && nextNote.Time == note.Time) {
				Random(nextNote);
				potentialOverlapNotes.Enqueue(nextNote);
				return note;
			}
			// Processing click
			else {
				RandomClick(note);
			}

			potentialOverlapNotes.Enqueue(note);
			return nextNote!;
		}

		void Random(Note note) => note.Position = rnd.NextSingle() * (2 + 2) - 2;

		void RandomClick(Note note)
		{
			while (potentialOverlapNotes.TryPeek(out var prevNote)) {
				if (MathF.Abs(note.Time - prevNote.Time) < multiTapTimeThreshold) {
					Scope scope = new(-2, 2);
					foreach (var n in potentialOverlapNotes)
						scope.Exclude(n.Position - multiTapPositionThreshold, n.Position + multiTapPositionThreshold);

					if (scope.IsBlank)
						ThrowHelper.ThrowInvalidOperationException($"Cannot place note because of parameter {nameof(multiTapTimeThreshold)} or {nameof(multiTapPositionThreshold)} to large.");
					else
						note.Position = rnd.NextSingleInArea(scope);
					return;
				}
				else {
					potentialOverlapNotes.Dequeue();
				}
			}
			Random(note);
		}

		void RandomNoteSize(Note note)
		{
			if (randomSize)
				note.Size = rnd.NextSingle() * (2f - 0.6f) + 0.6f;
		}
	}

	public static void ToAllClick(this Chart chart)
	{
		foreach (var note in chart.Notes) {
			if (note.IsSlide) {
				note.IsSlide = false;
			}
		}
	}
}
