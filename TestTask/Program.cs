using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    class Program
    {
        static void Main(string[] args)
        {
            RecordContext rc = new RecordContext();
            switch (args[0])
            {
                case "1":
                    {
                        rc.Database.CreateIfNotExists();
                        if (rc.Database.Exists())
                            Console.WriteLine("Table created.");
                        else
                            Console.WriteLine("There was some errors"); 
                        break;
                    }
                case "2":
                    {
                        byte forGender = 2;
                        if (args[3] == "F")
                            forGender = 0;
                        if (args[3] == "M")
                            forGender = 1;
                        rc.records.Add(new Record { fsln = args[1], dateOfBirth = DateTime.ParseExact(args[2],"dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture), gender = forGender });
                        rc.SaveChanges();
                        Console.WriteLine("Done.");
                        break;
                    }
                case "3":
                    {
                        int age;
                        var workaround = (from r in rc.records select r).ToList<Record>();
                        List<Record> distinctAsRequested = workaround.GroupBy(r => new { r.fsln, r.dateOfBirth }).Select(group => group.First()).ToList();
                        foreach (Record r in distinctAsRequested)
                        {
                            
                            if (r.dateOfBirth.Month < DateTime.Today.Month)
                                age = DateTime.Today.Year - r.dateOfBirth.Year;
                            if (r.dateOfBirth.Month == DateTime.Today.Month)
                            {
                                if (r.dateOfBirth.Day <= DateTime.Today.Day)
                                    age = DateTime.Today.Year - r.dateOfBirth.Year;
                                else
                                    age = DateTime.Today.Year - r.dateOfBirth.Year - 1;
                            }
                            else
                                age = DateTime.Today.Year - r.dateOfBirth.Year - 1;
                            Console.WriteLine("FSLN = " + r.fsln + ", Date of birth =  " + r.dateOfBirth + ", Age = " + age);
                        }
                        break;
                    }
                case "4":
                    {

                        int i = 0;
                        for (i = 0;i<100;i++)
                        {
                            rc.records.Add(new Record { fsln = randomizeNameForFirstHundred(), dateOfBirth = Convert.ToDateTime(DateTime.Today), gender = 1});
                            rc.SaveChanges();
                        }
                        for (i=i; i<1000000; i++)
                        {
                            rc.records.Add(new Record { fsln = randomizeName(), dateOfBirth = Convert.ToDateTime(DateTime.Today), gender = Convert.ToByte(i%2) });
                            rc.SaveChanges();
                        }
                        break;
                    }
                case "5":
                    {
                        Stopwatch timer = new Stopwatch();
                        timer.Start();
                        //ObjectContext context = (new RecordContext() as IObjectContextAdapter).ObjectContext;
                        //ObjectQuery<Record> uniques = context.CreateQuery<Record>("SELECT * FROM Records WHERE gender = 1 AND fsln EQUAL F*");
                        var uniques = (from r in rc.records where r.gender.Equals(1) && r.fsln.StartsWith("F") select r);
                        timer.Stop();
                        uniques.ToList<Record>();
                        TimeSpan ts = timer.Elapsed;
                        String elapsed = String.Format("{0:00}:{1:00}:{2:00}:{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                        foreach (Record r in uniques)
                        {
                            Console.WriteLine(r.fsln + " " + r.dateOfBirth.ToString() );
                        }
                        Console.WriteLine("Elapsed time " + elapsed);
                        break;
                    }
                case "6": break;
            }
            Console.ReadKey();


            String randomizeName()
            {
                String lowerCaseDictionary = "abcdefghijklmnopqrstuvwxyz";
                String upperCaseDictionary = lowerCaseDictionary.ToUpper();
                Random r = new Random();
                String name = "";
                for (int i = 0; i < 3; i++)
                {
                    name += upperCaseDictionary[r.Next(upperCaseDictionary.Length )];
                    for (int j = 0; i < r.Next(20); i++)
                        name += lowerCaseDictionary[r.Next(lowerCaseDictionary.Length)];

                }
                return name;
            }

            String randomizeNameForFirstHundred()
            {
                String lowerCaseDictionary = "abcdefghijklmnopqrstuvwxyz";
                String upperCaseDictionary = lowerCaseDictionary.ToUpper();
                Random r = new Random();
                String name = "F";
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < r.Next(20); j++)
                        name += lowerCaseDictionary[r.Next(lowerCaseDictionary.Length)];
                    name += upperCaseDictionary[r.Next(upperCaseDictionary.Length)];

                }
                for (int j = 0; j < r.Next(20); j++)
                    name += lowerCaseDictionary[r.Next(lowerCaseDictionary.Length)];
                return name;
            }
        }
    }
}
