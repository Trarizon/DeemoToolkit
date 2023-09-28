using CommunityToolkit.Diagnostics;
using Trarizon.Toolkit.Deemo.Algorithm.Utilities;
using Trarizon.Toolkit.Deemo.ChartModels;
using Trarizon.Library.Extensions;
using Trarizon.Library.Collections.Extensions.Queries;

namespace Trarizon.Toolkit.Deemo.Algorithm;
public static class ChartConverter
{
	public static Chart RearrangeByPitch(this Chart chart, bool fixRange = false)
	{
		const float DefaultSize = 0.2f;

		// List<> has a better performance here
		List<(float Time, PianoSound Sound)> sounds
			= chart.Notes.SelectMany(n => n.Sounds, (n, s) => (n.Time, s)).ToList();

		if (sounds.Count == 0)
			return new Chart(chart, false);

		var (minp, maxp) = fixRange
			? (PianoSound.PitchMin88, PianoSound.PitchMax88)
			: sounds.MinMax(s => s.Sound.Pitch);

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

		Random rnd = randomSeed is null ? Random.Shared : new Random(randomSeed.Value);
		Queue<Note> potentialOverlapNotes = new();

		var notes = chart.Notes.Where(n => n.IsVisible).SkipFirstAndKeep(out var prevNote);
		if (prevNote == null) // No note
			return;

		foreach (var note in notes) {
			prevNote = RandomInternal(prevNote, note);
			TryRandomNoteSize(prevNote);
		}

		{ // Last note
			if (prevNote.IsSlide) RandomNotePosition(prevNote);
			else RandomClick(prevNote);
			TryRandomNoteSize(prevNote);
		}
		return;

		// return: the note which is not handled
		Note RandomInternal(Note note, Note nextNote)
		{
			// If slide, just random[-2, 2]
			if (note.IsSlide) {
				RandomNotePosition(note);
			}
			// If double-hit, and this is click but next is slide,
			// process slide first to avoid overlapping
			else if (nextNote.IsSlide && nextNote.Time == note.Time) {
				RandomNotePosition(nextNote);
				potentialOverlapNotes.Enqueue(nextNote);
				return note;
			}
			// Processing click
			else {
				RandomClick(note);
			}

			potentialOverlapNotes.Enqueue(note);
			return nextNote;
		}

		void RandomNotePosition(Note note) => note.Position = rnd.NextSingle(-2, 2);

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
			RandomNotePosition(note);
		}

		void TryRandomNoteSize(Note note)
		{
			if (randomSize)
				note.Size = rnd.NextSingle(0.6f, 2f);
		}
	}

	public static void ToAllClick(this Chart chart)
	{
		foreach (var note in chart.Notes) {
			if (note.IsSlide) {
				note.IsSlide = false;
			}
			if (note.IsSwipe) {
				note.IsSwipe = false;
			}
			if (note.Duration > 0f) {
				note.Duration = 0f;
			}
		}
	}
}
