using System.Text.Json.Serialization;

namespace auth3.Records;
public record DailyPuzzle(
    [property:JsonPropertyName("puzzle")]Puzzle Puzzle
);

public record Puzzle(
    [property:JsonPropertyName("solution")]List<string>Solution,
    [property:JsonPropertyName("fen")]string Fen

);