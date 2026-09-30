#define DB_INIT_1

#if DB_INIT_1
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Data
{
	public static class DbInitializer
	{
		public static void Initialize(ContosoUniversityContext context)
		{
			// Look for any students.
			if (context.Students.Any())
			{
				return;   // DB has been seeded
			}

			var students = new Student[]
			{
				new Student{FirstName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2019-09-01")},
				new Student{FirstName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2017-09-01")},
				new Student{FirstName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2018-09-01")},
				new Student{FirstName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2017-09-01")},
				new Student{FirstName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2017-09-01")},
				new Student{FirstName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2016-09-01")},
				new Student{FirstName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2018-09-01")},
				new Student{FirstName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2019-09-01")}
			};

			context.Students.AddRange(students);
			context.SaveChanges();

			var courses = new Course[]
			{
				new Course{CourseID=1050,Title="Chemistry",Credits=3},
				new Course{CourseID=4022,Title="Microeconomics",Credits=3},
				new Course{CourseID=4041,Title="Macroeconomics",Credits=3},
				new Course{CourseID=1045,Title="Calculus",Credits=4},
				new Course{CourseID=3141,Title="Trigonometry",Credits=4},
				new Course{CourseID=2021,Title="Composition",Credits=3},
				new Course{CourseID=2042,Title="Literature",Credits=4}
			};

			context.Courses.AddRange(courses);
			context.SaveChanges();

			var enrollments = new Enrollment[]
			{
				new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
				new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
				new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
				new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
				new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
				new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
				new Enrollment{StudentID=3,CourseID=1050},
				new Enrollment{StudentID=4,CourseID=1050},
				new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
				new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
				new Enrollment{StudentID=6,CourseID=1045},
				new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
			};

			context.Enrollments.AddRange(enrollments);
			context.SaveChanges();
		}
	}
} 
#endif

#if DB_INIT_2
using RazorPages.Data;
using RazorPages.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContosoUniversity.Data
{
	public static class DbInitializer
	{
		public static void Initialize(ContosoUniversityContext context)
		{
			// Look for any students.
			if (context.Students.Any())
			{
				return;   // DB has been seeded
			}

			var alexander = new Student
			{
				FirstName = "Carson",
				LastName = "Alexander",
				EnrollmentDate = DateTime.Parse("2016-09-01")
			};

			var alonso = new Student
			{
				FirstName = "Meredith",
				LastName = "Alonso",
				EnrollmentDate = DateTime.Parse("2018-09-01")
			};

			var anand = new Student
			{
				FirstName = "Arturo",
				LastName = "Anand",
				EnrollmentDate = DateTime.Parse("2019-09-01")
			};

			var barzdukas = new Student
			{
				FirstName = "Gytis",
				LastName = "Barzdukas",
				EnrollmentDate = DateTime.Parse("2018-09-01")
			};

			var li = new Student
			{
				FirstName = "Yan",
				LastName = "Li",
				EnrollmentDate = DateTime.Parse("2018-09-01")
			};

			var justice = new Student
			{
				FirstName = "Peggy",
				LastName = "Justice",
				EnrollmentDate = DateTime.Parse("2017-09-01")
			};

			var norman = new Student
			{
				FirstName = "Laura",
				LastName = "Norman",
				EnrollmentDate = DateTime.Parse("2019-09-01")
			};

			var olivetto = new Student
			{
				FirstName = "Nino",
				LastName = "Olivetto",
				EnrollmentDate = DateTime.Parse("2011-09-01")
			};

			var students = new Student[]
			{
				alexander,
				alonso,
				anand,
				barzdukas,
				li,
				justice,
				norman,
				olivetto
			};

			context.AddRange(students);

			var abercrombie = new Instructor
			{
				FirstName = "Kim",
				LastName = "Abercrombie",
				HireDate = DateTime.Parse("1995-03-11")
			};

			var fakhouri = new Instructor
			{
				FirstName = "Fadi",
				LastName = "Fakhouri",
				HireDate = DateTime.Parse("2002-07-06")
			};

			var harui = new Instructor
			{
				FirstName = "Roger",
				LastName = "Harui",
				HireDate = DateTime.Parse("1998-07-01")
			};

			var kapoor = new Instructor
			{
				FirstName = "Candace",
				LastName = "Kapoor",
				HireDate = DateTime.Parse("2001-01-15")
			};

			var zheng = new Instructor
			{
				FirstName = "Roger",
				LastName = "Zheng",
				HireDate = DateTime.Parse("2004-02-12")
			};

			var instructors = new Instructor[]
			{
				abercrombie,
				fakhouri,
				harui,
				kapoor,
				zheng
			};

			context.AddRange(instructors);

			var officeAssignments = new OfficeAssignment[]
			{
				new OfficeAssignment {
					Instructor = fakhouri,
					Location = "Smith 17" },
				new OfficeAssignment {
					Instructor = harui,
					Location = "Gowan 27" },
				new OfficeAssignment {
					Instructor = kapoor,
					Location = "Thompson 304" }
			};

			context.AddRange(officeAssignments);

			var english = new Department
			{
				Name = "English",
				Budget = 350000,
				StartDate = DateTime.Parse("2007-09-01"),
				Administrator = abercrombie
			};

			var mathematics = new Department
			{
				Name = "Mathematics",
				Budget = 100000,
				StartDate = DateTime.Parse("2007-09-01"),
				Administrator = fakhouri
			};

			var engineering = new Department
			{
				Name = "Engineering",
				Budget = 350000,
				StartDate = DateTime.Parse("2007-09-01"),
				Administrator = harui
			};

			var economics = new Department
			{
				Name = "Economics",
				Budget = 100000,
				StartDate = DateTime.Parse("2007-09-01"),
				Administrator = kapoor
			};

			var departments = new Department[]
			{
				english,
				mathematics,
				engineering,
				economics
			};

			context.AddRange(departments);

			var chemistry = new Course
			{
				CourseID = 1050,
				Title = "Chemistry",
				Credits = 3,
				Department = engineering,
				Instructors = new List<Instructor> { kapoor, harui }
			};

			var microeconomics = new Course
			{
				CourseID = 4022,
				Title = "Microeconomics",
				Credits = 3,
				Department = economics,
				Instructors = new List<Instructor> { zheng }
			};

			var macroeconmics = new Course
			{
				CourseID = 4041,
				Title = "Macroeconomics",
				Credits = 3,
				Department = economics,
				Instructors = new List<Instructor> { zheng }
			};

			var calculus = new Course
			{
				CourseID = 1045,
				Title = "Calculus",
				Credits = 4,
				Department = mathematics,
				Instructors = new List<Instructor> { fakhouri }
			};

			var trigonometry = new Course
			{
				CourseID = 3141,
				Title = "Trigonometry",
				Credits = 4,
				Department = mathematics,
				Instructors = new List<Instructor> { harui }
			};

			var composition = new Course
			{
				CourseID = 2021,
				Title = "Composition",
				Credits = 3,
				Department = english,
				Instructors = new List<Instructor> { abercrombie }
			};

			var literature = new Course
			{
				CourseID = 2042,
				Title = "Literature",
				Credits = 4,
				Department = english,
				Instructors = new List<Instructor> { abercrombie }
			};

			var courses = new Course[]
			{
				chemistry,
				microeconomics,
				macroeconmics,
				calculus,
				trigonometry,
				composition,
				literature
			};

			context.AddRange(courses);

			var enrollments = new Enrollment[]
			{
				new Enrollment {
					Student = alexander,
					Course = chemistry,
					Grade = Grade.A
				},
				new Enrollment {
					Student = alexander,
					Course = microeconomics,
					Grade = Grade.C
				},
				new Enrollment {
					Student = alexander,
					Course = macroeconmics,
					Grade = Grade.B
				},
				new Enrollment {
					Student = alonso,
					Course = calculus,
					Grade = Grade.B
				},
				new Enrollment {
					Student = alonso,
					Course = trigonometry,
					Grade = Grade.B
				},
				new Enrollment {
					Student = alonso,
					Course = composition,
					Grade = Grade.B
				},
				new Enrollment {
					Student = anand,
					Course = chemistry
				},
				new Enrollment {
					Student = anand,
					Course = microeconomics,
					Grade = Grade.B
				},
				new Enrollment {
					Student = barzdukas,
					Course = chemistry,
					Grade = Grade.B
				},
				new Enrollment {
					Student = li,
					Course = composition,
					Grade = Grade.B
				},
				new Enrollment {
					Student = justice,
					Course = literature,
					Grade = Grade.B
				}
			};

			context.AddRange(enrollments);
			context.SaveChanges();
		}
	}
} 
#endif