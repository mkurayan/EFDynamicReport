#EF Dynamic Report
This library allows to build dynamic reports based on Entity Framework mapping. Dynamic report system extracts information about DB structure (columns and tables names) from EF mapping and then uses this information in order to build SQL query which will return data for report. Such approach helps to separate your report mapping and DB structure.

DynamicReport.Demo project contains simple example which shows how you can configure report system in order to build dynamic reports. Here is a main steps:


###1. Setup your Entity Framework mappings.
In internet exists many articles about Entity Framework.
Here we will show only example from *DynamicReport.Demo* project.


    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        public string FirstName { get; set; }

        //some code removed for brevity...
    }

    public class SchoolDbContext : DbContext
    {
         public DbSet<Student> Students { get; set; }

        //some code removed for brevity...
    }
![dbstructure](https://cloud.githubusercontent.com/assets/9530261/9613719/d7c1e4b8-50f5-11e5-9b6d-08c43feef4c7.png)
###2. Configure Report Template
For each report we should declare what columns will be available in it. In our case for this purpose we use report template class  **StudentsReport** which contains information about the all columns which will be available in report.

    public class StudentsReportTemplate : ReportTemplate
    {
        ///some code removed for brevity...

        public override IEnumerable<IReportColumn> ReportColumns
        {
            get
            {
               return new List<IReportColumn>
                    {
                        StudentTable.Column("First Name", x => x.FirstName),
                        StudentTable.Column("Last Name", x => x.LastName),
                        StudentTable.Column("Phone", x => x.Phone),
                        StudentTable.Column("Home Adress", x => x.HomeAdress),
                        new AverageScore(queryExtractor, StudentTable).Column("Average Score"),
                        new MinimumScore(queryExtractor, StudentTable).Column("Minimum Score"),
                        new MaximumScore(queryExtractor, StudentTable).Column("Maximum Score"),
                        new Age(StudentTable).Column("Age"),
                        new Subjects(queryExtractor, StudentTable).Column("Subjects")
                    };
            }
        }

        public override IReportDataSource ReportDataSource
        {
            get { return StudentTable.GetReportDataSource(); }
        }
    }


Each statement is declaring one of the possible report columns. 
For example *StudentTable.Column("First Name", x => x.FirstName)* is declaring that report can contain students names.
If you will  build report with only one "First Name" column, the result SQL query will be look like this: 

    SELECT 
        s.FirstName 
    FROM 
        [dbo].[Students] AS s

###3. Create report and get report data from it
After we configured Report Template we can create report model from it.

    StudentsReportTemplate template = new StudentsReportTemplate(...);

    //Report Template create IReportModel object which do not depend from concrete report template.
    IReportModel reportModel = template.CreateReport();

In order to get report data we specify what columns and filters we want to see in report (all columns should exists in report, see step #2 for more details) 
    
    var filters = new IReportFilter[0];    
    var columns = new IReportColumn[]
    {
        new ReportColumn { Title = "First Name"}
    }   
    
    var data =  reportModel.Get(columns, filters)

For example report about the students averages scores will look like this one:
![dynamicreport](https://cloud.githubusercontent.com/assets/9530261/9613718/d7c19a4e-50f5-11e5-9d7c-2b6b068660ae.png)

Result SQL query will look like this:

    SELECT 
        ( SELECT AVG(e.Score)  FROM ExamenResults as e WHERE e.StudentId = s.StudentId) AS AverageScore,
        s.FirstName AS FirstName 
        s.LastName AS LastName
    FROM 
        FROM [dbo].[Students] AS s
