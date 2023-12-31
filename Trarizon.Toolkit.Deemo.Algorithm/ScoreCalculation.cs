using CommunityToolkit.Diagnostics;

namespace Trarizon.Toolkit.Deemo.Algorithm;
public static class ScoreCalculation
{
	private const float CharmingBoundary = 50f;
	private const float HitBoundary = 120;

	// Result is in [0, 1]
	public static float TotalScore(int notesCount, ReadOnlySpan<float> offsets) => CombineScores(JudgeScore(notesCount, offsets), ComboScore(notesCount, offsets));
	public static float TotalScore(ReadOnlySpan<float> offsets) => TotalScore(offsets.Length, offsets);
	public static float TotalScore(int notesCount, int charmingCount) => CombineScores(JudgeScore(notesCount, charmingCount), ComboScore(notesCount, charmingCount));

	// Result is in [0, 1]
	public static float JudgeScore(int notesCount, ReadOnlySpan<float> offsets)
	{
		Guard.IsGreaterThanOrEqualTo(notesCount, 0);
		Guard.IsGreaterThanOrEqualTo(notesCount, offsets.Length);

		return offsets.Length == 0 ? 0 : ActualJudgeScore(offsets) / notesCount;
	}
	public static float JudgeScore(ReadOnlySpan<float> offsets) => JudgeScore(offsets.Length, offsets);
	public static float JudgeScore(int notesCount, int charmingCount)
	{
		Guard.IsLessThanOrEqualTo(charmingCount, notesCount);
		return charmingCount / notesCount;
	}

	// Result is in [0, 1]
	public static float ComboScore(int notesCount, ReadOnlySpan<float> offsets)
	{
		Guard.IsGreaterThanOrEqualTo(notesCount, 0);
		Guard.IsGreaterThanOrEqualTo(notesCount, offsets.Length);

		return offsets.Length == 0 ? 0 : ActualComboScore(offsets) / ActualFullComboScore(notesCount);
	}
	public static float ComboScore(ReadOnlySpan<float> offsets) => ComboScore(offsets.Length, offsets);
	public static float ComboScore(int notesCount, int hitCount)
	{
		Guard.IsLessThanOrEqualTo(hitCount, notesCount);
		return ActualFullComboScore(hitCount) / ActualFullComboScore(notesCount);
	}

	private static float ActualJudgeScore(ReadOnlySpan<float> offsets)
	{
		float score = 0f;

		for (int i = 0; i < offsets.Length; i++) {
			if (offsets[i].IsCharming())
				score += 1f;
			else if (offsets[i].IsHit())
				score += (HitBoundary - offsets[i]) / (HitBoundary - CharmingBoundary) * 0.5f + 0.5f;
		}
		return score;
	}
	private static float ActualComboScore(ReadOnlySpan<float> offsets)
	{
		float score = 0f;

		int i = 0;
		while (i < offsets.Length) {
			if (offsets[i++].IsHit())
				break;
		}

		float prev = 0;
		for (; i < offsets.Length; i++) {
			if (offsets[i].IsHit()) {
				prev += 1;
				score += prev;
			}
			else
				prev *= 0.7f;
		}

		return score;
	}
	private static float ActualFullComboScore(int noteCount) => ((noteCount - 1) * noteCount) >> 1;
	private static float CombineScores(float judgeScore, float comboScore) => judgeScore * 0.8f + comboScore * 0.2f;

	private static bool IsCharming(this float offset) => offset <= CharmingBoundary;
	private static bool IsHit(this float offset) => offset <= HitBoundary;

	public static ScoreResult Calc(ReadOnlySpan<float> offsets)
	{
		ScoreResult result = new() { NoteCount = offsets.Length };
		float sCombo = -1f; // first hit score is 0;

		int nCombo = 0;

		for (int i = 0; i < offsets.Length; i++) {
			switch (offsets[i]) {
				case <= CharmingBoundary:
					result.JudgeScore += 1f;

					result.ComboScore += sCombo += 1f;
					nCombo++;
					result.CharmingCount++;
					break;
				case <= HitBoundary:
					result.JudgeScore += (HitBoundary - offsets[i]) / (HitBoundary - CharmingBoundary) * 0.5f + 0.5f;
					result.ComboScore += sCombo += 1f;

					nCombo++;
					break;
				default:
					// Didn't hit leading notes
					if (result.MaxCombo == 0 && nCombo == 0)
						break;

					if (result.MaxCombo < nCombo) 
						result.MaxCombo = nCombo;
					nCombo = 0;
					sCombo *= 0.7f;
					break;
			}
		}
		result.JudgeScore /= result.NoteCount;
		result.ComboScore /= ((result.NoteCount - 1) * result.NoteCount) >> 1;
		return result;
	}
}

public record struct ScoreResult
{
	public int CharmingCount;
	public int MaxCombo;
	public int NoteCount;
	public float JudgeScore;
	public float ComboScore;

	public readonly float Score => JudgeScore * 0.8f + ComboScore * 0.2f;
}
