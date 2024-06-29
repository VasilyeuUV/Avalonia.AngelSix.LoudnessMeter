namespace Avalonia.AngelSix.LoudnessMeter.Services;

/// <summary>
/// Information about one audio fragment
/// </summary>
public record AudioChunkData(
    double ShortTermLufs,
    double IntegratedLufs,
    double Loudness,
    double LoudnessRange,
    double RealTimeDynamics,
    double AverageRealTimeDynamics,
    double MomentaryMaxLufs,
    double ShortTermMaxLufs,
    double TruePeakMax
    );
