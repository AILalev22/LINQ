using System;
using System.Linq;

int[] numbers = { 8, 3, 12, 5, 2, 9, 4, 7 };

Student[] students =
{
    new Student(1, "Ana", 17, "Burgas"),
    new Student(2, "Boris", 18, "Sofia"),
    new Student(3, "Clara", 20, "Burgas"),
    new Student(4, "Deyan", 19, "Varna"),
    new Student(5, "Elena", 18, "Sofia"),
    new Student(6, "Filip", 21, "Burgas")
};

Course[] courses =
{
    new Course(101, "C# Basics"),
    new Course(102, "Web Development"),
    new Course(103, "Databases"),
    new Course(104, "Algorithms")
};

Enrollment[] enrollments =
{
    new Enrollment(1, 1, 101, 5),
    new Enrollment(2, 2, 101, 6),
    new Enrollment(3, 3, 102, 6),
    new Enrollment(4, 4, 102, 4),
    new Enrollment(5, 5, 103, 5),
    new Enrollment(6, 6, 103, 6),
    new Enrollment(7, 2, 103, 4),
    new Enrollment(8, 3, 101, 3),
    new Enrollment(9, 6, 102, 5)
};

Console.WriteLine("01. Square every number");
var squares = numbers.Select(n => n * n);
foreach (var value in squares)
    Console.WriteLine(value);

Console.WriteLine("\n02. Project student details");
var studentDetails = students.Select(s => new { s.Name, s.City });
foreach (var student in studentDetails)
    Console.WriteLine($"{student.Name} / {student.City}");

Console.WriteLine("\n03. Keep the even numbers");
var evenNumbers = numbers.Where(n => n % 2 == 0);
foreach (var value in evenNumbers)
    Console.WriteLine(value);

Console.WriteLine("\n04. Burgas students aged at least 18");
var burgasAdults = students.Where(s => s.City == "Burgas" && s.Age >= 18);
foreach (var student in burgasAdults)
    Console.WriteLine($"{student.Id}: {student.Name}");

Console.WriteLine("\n05. Names of adult students");
var adultNames = students.Where(s => s.Age >= 18).Select(s => s.Name);
foreach (var name in adultNames)
    Console.WriteLine(name);

Console.WriteLine("\n06. Sort numbers ascending");
var ascendingNumbers = numbers.OrderBy(n => n);
foreach (var value in ascendingNumbers)
    Console.WriteLine(value);

Console.WriteLine("\n07. Adult names from youngest to oldest");
var adultsByAge = students
    .Where(s => s.Age >= 18)
    .OrderBy(s => s.Age)
    .Select(s => s.Name);
foreach (var name in adultsByAge)
    Console.WriteLine(name);

Console.WriteLine("\n08. Sort numbers descending using OrderBy");
var descendingNumbers = numbers.OrderBy(n => -n);
foreach (var value in descendingNumbers)
    Console.WriteLine(value);

Console.WriteLine("\n09. The first Sofia student");
var firstSofiaStudent = students.First(s => s.City == "Sofia");
Console.WriteLine($"{firstSofiaStudent.Name} (Id {firstSofiaStudent.Id})");
// Boris is first in source order among students from Sofia; Elena appears later.

Console.WriteLine("\n10. First when no student matches");
try
{
    var plovdivStudent = students.First(s => s.City == "Plovdiv");
    Console.WriteLine(plovdivStudent);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"InvalidOperationException: {ex.Message}");
}
// First(predicate) throws InvalidOperationException when no element matches.

Console.WriteLine("\n11. Exactly one student with Id 4");
var studentWithId4 = students.Single(s => s.Id == 4);
Console.WriteLine($"{studentWithId4.Name} (Id {studentWithId4.Id})");
// Single is appropriate when the predicate is expected to match exactly one record.

Console.WriteLine("\n12. Single with zero or several matches");
try
{
    var age18Student = students.Single(s => s.Age == 18);
    Console.WriteLine(age18Student);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Age == 18: InvalidOperationException: {ex.Message}");
}
// Two students are age 18, so Single throws.
try
{
    var missingStudent = students.Single(s => s.Id == 99);
    Console.WriteLine(missingStudent);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Id == 99: InvalidOperationException: {ex.Message}");
}
// No student has Id 99, so Single also throws, this time because there are zero matches.

Console.WriteLine("\n13. First versus Single");
TryPrediction("numbers.First(n => n > 7)", () => numbers.First(n => n > 7).ToString());
TryPrediction("numbers.Single(n => n > 7)", () => numbers.Single(n => n > 7).ToString());
TryPrediction("numbers.Single(n => n > 10)", () => numbers.Single(n => n > 10).ToString());
TryPrediction("numbers.First(n => n < 0)", () => numbers.First(n => n < 0).ToString());

Console.WriteLine("\n14. Group students by city");
var studentsByCity = students.GroupBy(s => s.City);
foreach (var group in studentsByCity)
{
    Console.WriteLine($"{group.Key}:");
    foreach (var student in group)
        Console.WriteLine($"  {student.Name}");
}

Console.WriteLine("\n15. Group minors and adults");
var studentsByAgeCategory = students.GroupBy(s => s.Age >= 18);
foreach (var group in studentsByAgeCategory)
{
    string label = group.Key ? "Adults" : "Minors";
    Console.WriteLine($"{label}:");
    foreach (var student in group)
        Console.WriteLine($"  {student.Name}");
}

Console.WriteLine("\n16. High grades grouped by course");
var highGradesByCourse = enrollments
    .Where(e => e.Grade >= 5)
    .GroupBy(e => e.CourseId);
foreach (var group in highGradesByCourse)
{
    Console.WriteLine($"{group.Key}:");
    foreach (var enrollment in group)
        Console.WriteLine($"  Enrollment {enrollment.Id}");
}

Console.WriteLine("\n17. Attach student names to grades");
var gradesWithStudentNames = enrollments.Join(
    students,
    e => e.StudentId,
    s => s.Id,
    (e, s) => new
    {
        EnrollmentId = e.Id,
        StudentName = s.Name,
        e.Grade
    });
foreach (var item in gradesWithStudentNames)
    Console.WriteLine($"{item.EnrollmentId}/{item.StudentName}/{item.Grade}");

Console.WriteLine("\n18. Courses and their high grades");
var coursesWithHighGrades = courses
    .Join(
        enrollments,
        c => c.Id,
        e => e.CourseId,
        (c, e) => new { CourseTitle = c.Title, e.StudentId, e.Grade })
    .Where(item => item.Grade >= 5);
foreach (var item in coursesWithHighGrades)
    Console.WriteLine($"{item.CourseTitle}: {item.StudentId}/{item.Grade}");
// Algorithms is absent because it has no matching enrollments.

Console.WriteLine("\n19. Sorted report from three arrays");
var sortedReport = enrollments
    .Join(
        students,
        e => e.StudentId,
        s => s.Id,
        (e, s) => new { Enrollment = e, StudentName = s.Name })
    .Join(
        courses,
        item => item.Enrollment.CourseId,
        c => c.Id,
        (item, c) => new
        {
            EnrollmentId = item.Enrollment.Id,
            item.StudentName,
            CourseTitle = c.Title,
            Grade = item.Enrollment.Grade
        })
    .Where(item => item.Grade >= 5)
    .OrderBy(item => item.EnrollmentId)
    .Select(item => new
    {
        item.StudentName,
        item.CourseTitle,
        item.Grade
    });
foreach (var item in sortedReport)
    Console.WriteLine($"{item.StudentName}/{item.CourseTitle}/{item.Grade}");

Console.WriteLine("\n20. Highest grade in each course");
var highestGradePerCourse = enrollments
    .Join(
        students,
        e => e.StudentId,
        s => s.Id,
        (e, s) => new { Enrollment = e, StudentName = s.Name })
    .Join(
        courses,
        item => item.Enrollment.CourseId,
        c => c.Id,
        (item, c) => new
        {
            CourseId = c.Id,
            CourseTitle = c.Title,
            EnrollmentId = item.Enrollment.Id,
            StudentName = item.StudentName,
            Grade = item.Enrollment.Grade
        })
    .GroupBy(item => new { item.CourseId, item.CourseTitle })
    .OrderBy(group => group.Key.CourseId)
    .Select(group => group.OrderBy(item => -item.Grade).First());

foreach (var item in highestGradePerCourse)
    Console.WriteLine($"{item.CourseTitle} / {item.StudentName} / {item.Grade}");
// Algorithms is omitted because the joins only include courses with enrollments.
// OrderBy is stable, so equal top grades keep the first matching enrollment's source order.

static void TryPrediction(string expression, Func<string> evaluate)
{
    try
    {
        Console.WriteLine($"{expression} => {evaluate()}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"{expression} => InvalidOperationException: {ex.Message}");
    }
}

public record Student(int Id, string Name, int Age, string City);
public record Course(int Id, string Title);
public record Enrollment(int Id, int StudentId, int CourseId, int Grade);
