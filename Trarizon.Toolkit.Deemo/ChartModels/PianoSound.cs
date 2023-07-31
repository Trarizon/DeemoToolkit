﻿using Newtonsoft.Json;

namespace Trarizon.Toolkit.Deemo.ChartModels;
[JsonObject(MemberSerialization.OptIn)]
public sealed class PianoSound : IEquatable<PianoSound>
{
    public const int PitchMax88 = 108;
    public const int PitchMin88 = 21;

    [JsonProperty("w")]
    public float Delay { get; set; }

    [JsonProperty("d")]
    public float Duration { get; set; }

    [JsonProperty("p")]
    public int Pitch { get; set; }

    [JsonProperty("v")]
    public int Velocity { get; set; }

    [JsonConstructor]
    public PianoSound(float delay, float duration, int pitch, int velocity)
    {
        Delay = delay;
        Duration = duration;
        Pitch = pitch;
        Velocity = velocity;
    }

    public PianoSound(PianoSound other) :
        this(other.Delay, other.Duration, other.Pitch, other.Velocity)
    { }

    public bool Equals(PianoSound? other)
        => other != null
        && Delay == other.Delay
        && Duration == other.Duration
        && Pitch == other.Pitch
        && Velocity == other.Velocity;

    public override bool Equals(object? obj) => Equals(obj as PianoSound);

    public override int GetHashCode() => HashCode.Combine(Delay, Duration, Pitch, Velocity);
}