using CommunityToolkit.Diagnostics;

namespace Trarizon.Toolkit.Deemo.Algorithm;
public static class ScoreCalculation
{
	private const float CharmingLimit = 50f;
	private const float HitLimit = 120;

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
				score += (HitLimit - offsets[i]) / (HitLimit - CharmingLimit) * 0.5f + 0.5f;
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

	private static bool IsCharming(this float offset) => offset <= CharmingLimit;
	private static bool IsHit(this float offset) => offset <= HitLimit;
}
