using System.Globalization;
namespace Q4_GradingSystem;

    //a) Student class
    public class Student
    {
        public int Id { get; }
        public string FullName { get; }
        public int Score { get; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70 && Score <= 79) return "B";
            if (Score >= 60 && Score <= 69) return "C";
            if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }

        public override string ToString()
        {
            return $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
        }
    }

    //b) & c) Custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    //d) Processor class
    public class StudentResultProcessor
    {
        // Read students from a CSV-like .txt file: "Id,Full Name,Score"
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using var reader = new StreamReader(inputFilePath);
            string? line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                // Skip empty/whitespace-only lines
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');
                if (parts.Length < 3)
                {
                    throw new MissingFieldException(
                        $"Line {lineNumber}: Expected 3 fields (Id, FullName, Score) but found {parts.Length}."
                    );
                }

                string idRaw = parts[0].Trim();
                string nameRaw = parts[1].Trim();
                string scoreRaw = parts[2].Trim();

                if (string.IsNullOrWhiteSpace(idRaw) ||
                    string.IsNullOrWhiteSpace(nameRaw) ||
                    string.IsNullOrWhiteSpace(scoreRaw))
                {
                    throw new MissingFieldException($"Line {lineNumber}: One or more required fields are empty.");
                }

                if (!int.TryParse(idRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Student ID '{idRaw}' is not a valid integer.");
                }

                if (!int.TryParse(scoreRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int score))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{scoreRaw}' is not a valid integer.");
                }

                // Optional check: keep scores in 0–100 range
                if (score < 0 || score > 100)
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{score}' must be between 0 and 100.");
                }

                students.Add(new Student(id, nameRaw, score));
            }

        return students;
    }

    // Write formatted report
    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath, false);
        writer.WriteLine("=== Student Grade Report ===");
        writer.WriteLine($"Generated: {DateTime.Now}");
        writer.WriteLine();
        foreach (var s in students)
        {
            writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
        }
    }
}

//e) Main application flow
public static class Program
{
    public static void Main(string[] args)
    {
        // You can pass custom file paths via args[0] and args[1].
        // Defaults for quick testing:
        string inputPath = args.Length > 0 ? args[0] : "students_input.txt";
        string outputPath = args.Length > 1 ? args[1] : "students_report.txt";

        try
        {
            var processor = new StudentResultProcessor();

            // If no input file exists, create a tiny demo file to make testing easy
            if (!File.Exists(inputPath))
            {
                File.WriteAllLines(inputPath, new[]
                {
                    "101,Alice Smith,84",
                    "102,Bob Johnson,73",
                    "103,Carol Lee,59",
                    "104,David Kim,42"
                });
                Console.WriteLine($"Sample input created at: {Path.GetFullPath(inputPath)}");
            }

            var students = processor.ReadStudentsFromFile(inputPath);
            processor.WriteReportToFile(students, outputPath);

            Console.WriteLine("Report generated successfully.");
            Console.WriteLine($"Input : {Path.GetFullPath(inputPath)}");
            Console.WriteLine($"Output: {Path.GetFullPath(outputPath)}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File not found: {ex.FileName}\n{ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Invalid score format: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing field: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("An unexpected error occurred.");
            Console.WriteLine(ex.Message);
        }
    }
}