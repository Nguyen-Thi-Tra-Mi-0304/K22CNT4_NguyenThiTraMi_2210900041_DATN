using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WebThiTracNghiemOnline.Models;
namespace WebThiTracNghiemOnline.Context
{
    public class WebsiteTracNghiemDBContext : DbContext
    {
        public WebsiteTracNghiemDBContext() : base("DATATHITRACNGHIEMONLINEConnectionString") { }
        public DbSet<Industry> INDUSTRY { get; set; }
        public DbSet<AccountUser> ACCOUNTUSER { get; set; }
        public DbSet<Course> COURSE { get; set; }
        public DbSet<Semester> SEMESTER { get; set; }
        public DbSet<AccountStudents> ACCOUNTSTUDENTS { get; set; }
        public DbSet<Subject> SUBJECT { get; set; }
        public DbSet<Chapters> CHAPTER { get; set; }
        public DbSet<SubjectByTeacher> SUBJECTBYTEACHER { get; set; }
        public DbSet<Class> CLASS { get; set; }
        public DbSet<StudentOnClass> STUDENTONCLASS { get; set; }
        public DbSet<Question> QUESTION { get; set; }
        public DbSet<Answers> ANSWERS { get; set; }
        public DbSet<Exam> EXAM { get; set; }
        public DbSet<QuestionsToExam> QUESTIONTOEXAM { get; set; }
        public DbSet<TempAnswers> TEMPANSWERS { get; set; }
        public DbSet<ExamRecords> EXAMRECORDS { get; set; }
        public DbSet<Privilege> PRIVILEGE { get; set; }
    }
} 