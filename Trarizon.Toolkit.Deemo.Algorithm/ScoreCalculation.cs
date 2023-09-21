namespace Trarizon.Toolkit.Deemo.Algorithm;
public static class ScoreCalculation
{
	private const float CharmingLimit = 50f;
	private const float HitLimit = 120;

	// Max Result is in [0, 1]
	public static float TotalScore(ReadOnlySpan<float> offsets) => JudgeScore(offsets) * 0.8f + ComboScore(offsets) * 0.2f;

	// Max Result is in [0, 1]
	public static float JudgeScore(ReadOnlySpan<float> offsets) => offsets.Length == 0 ? 0 :
		ActualJudgeScore(offsets) / offsets.Length;

	// Max Result is in [0, 1]
	public static float ComboScore(ReadOnlySpan<float> offsets) => offsets.Length == 0 ? 0 :
		ActualComboScore(offsets) / ((offsets.Length - 1) * offsets.Length / 2);

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

	private static bool IsCharming(this float score) => score <= CharmingLimit;

	private static bool IsHit(this float score) => score <= HitLimit;
}
