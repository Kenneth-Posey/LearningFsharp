CREATE DATABASE FSharpSample;
go

use FSharpSample

CREATE TABLE Student(
	StudentID int not null
		constraint PrimaryKey primary key,	
	Name nvarchar(50) null,
	Age int null,);
	
CREATE TABLE Course(
	CourseID int not null
		constraint PrimaryKey2 primary key,
	CourseName varchar(max) null);
	
go
	
CREATE TABLE CourseSelection(
	StudentID int not null
		constraint foreignKey references dbo.Student(StudentID),
	CourseID int null
		constraint foreignKey2 references dbo.Course(CourseID),
	ID int null);
	
go

INSERT Student VALUES (1, 'Lisa', 21)
INSERT Student VALUES (2, 'Brent', 22)
INSERT Student VALUES (3, 'Anita', 20)
INSERT Student VALUES (4, 'Ken', 22)
INSERT Student VALUES (5, 'Cathy', 22)
INSERT Student VALUES (6, 'Tom', 20)
INSERT Student VALUES (7, 'Zeoy', 21)
INSERT Student VALUES (8, 'Mark', 23)
INSERT Student VALUES (9, 'John', null) 
go

INSERT Course VALUES (1, 'Math') 
INSERT Course VALUES (2, 'Physics') 
INSERT Course VALUES (3, 'Biology') 
INSERT Course VALUES (4, 'English')
go

INSERT CourseSelection VALUES(1, 1, null)
INSERT CourseSelection VALUES(2, 1, null)
INSERT CourseSelection VALUES(3, 1, null)
INSERT CourseSelection VALUES(2, 2, null)
INSERT CourseSelection VALUES(2, 2, null)
INSERT CourseSelection VALUES(3, 3, null)
INSERT CourseSelection VALUES(3, 2, null)
go
	