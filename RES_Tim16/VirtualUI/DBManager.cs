﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualUI.Access;
using VirtualUI.Models;

namespace VirtualUI
{
    [ExcludeFromCodeCoverage]
    public class DBManager:IDBManager
    {
        static DBManager instance;

        public static DBManager Instance
        {
            get
            {
                if (instance == null)
                    return new DBManager();
                else
                    return instance;
            }
        }

        #region DeltaOperations
        public bool AddDelta(Delta delta)
        {
            using (var dbContext = new FileContext())
            {
                try
                {
                    if (delta.FileId == "")
                        return false;
                    dbContext.Deltas.Add(delta);
                    dbContext.SaveChanges();

                    Console.WriteLine(DateTime.Now + ": Delta Added to Database.");
                    return true;
                }
                catch
                {
                    Console.WriteLine(DateTime.Now + ": Delta wasn't added to Database.");
                    return false;
                }
            }
        }

        public bool DeltaExists(string id)
        {
            if(id==null)
            {
                throw new ArgumentNullException("Id can't be null");
            }
            using (var dbContext = new FileContext())
            {

                try
                {
                    Delta d = new Delta();
                    d = null;
                    d = dbContext.Deltas.Find(id);

                    if (d != null)
                    {
                        Console.WriteLine(DateTime.Now + ": Delta exists in Database.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(DateTime.Now + ": Delta doesn't exist in Database.");
                        return false;
                    }
                }
                catch
                {
                    Console.WriteLine(DateTime.Now + ": Something went wrong in DeltaExists method.");
                    return false;
                }
            }
        }

        public bool UpdateDelta(Delta d)
        {
            using (var dbContext = new FileContext())
            {

                var oldDelta = dbContext.Deltas.Find(d.FileId);
                if (oldDelta == null)
                {
                    Console.WriteLine("Can't update delta since it isn't in Database");
                    return false;
                }
                else
                {
                    oldDelta.FileId = d.FileId;
                    oldDelta.LineRange = d.LineRange;
                    oldDelta.Content = d.Content;

                    try
                    {
                        dbContext.SaveChanges();
                        Console.WriteLine(DateTime.Now + ": Delta Updated to Database.");
                        return true;
                    }
                    catch
                    {
                        Console.WriteLine(DateTime.Now + ": Something wrong with Delta updating to Database.");
                        return false;
                    }
                }
            }
        }

        #endregion

        #region FileOperations
        public bool AddFile(Files f)
        {
            if(f.Id==null || f.Name==null || f.Extension==null)
            {
                throw new ArgumentNullException("Values can't be null");
            }

            if (f.Id == "" || f.Name == "" || f.Extension == "")
            {
                return false;
            }
            using (var dbContext = new FileContext())
            {
                try
                {
                    dbContext.Files.Add(f);
                    dbContext.SaveChanges();
                    Console.WriteLine("New file has been added.");
                    return true;
                }
                catch
                {
                    Console.WriteLine("Somethingg is wrong the AddFile function for Database");
                    return false;
                    throw new ArgumentException("Out of range!");
                }
            }
        }

        public bool FileExists(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Arguments can't be null");
            }

            using (var dbContext = new FileContext())
            {

                try
                {
                    Files f = new Files();
                    f = null;
                    f = dbContext.Files.Find(id);

                    if (f != null)
                    {
                        Console.WriteLine("File has been found!");
                        return true;
                    }

                    else
                    {
                        Console.WriteLine("File doesn't exist in database");
                        return false;
                    }
                       
                }
                catch
                {
                    Console.WriteLine("Something wrong with the function FileExists");
                    return false;
                }
            }
        }

        #endregion

        #region FileContentOperations

        public bool AddFileContent(FileContent fcontent)
        {
            if(fcontent.FileId==null||fcontent.Content==null)
            {
                throw new ArgumentNullException ("Values can't be null");
            }

            if(fcontent.Content=="" || fcontent.Id=="")
            {
                return false;
            }

            using (var dbContext = new FileContext())
            {
                try
                {
                    fcontent.Id = Guid.NewGuid().ToString();
                    dbContext.FileContents.Add(fcontent);
                    dbContext.SaveChanges();
                    Console.WriteLine(DateTime.Now + ": FileContent Added to Database.");
                    return true;
                }
                catch
                {
                    Console.WriteLine(DateTime.Now + ": FileContent wasn't added to Database.");
                    return false;
                    throw new ArgumentException("Out of range!");
                    
                }
            }
        }

        public bool UpdateFileContent(FileContent fc)
        {
            if (fc.Id == null || fc.FileId == null || fc.Content == null)
            {
                throw new ArgumentNullException("Values can't be null");
            }

            if (fc.Content == "" || fc.Id == "")
            {
                return false;
            }

            using (var dbContext = new FileContext())
            {
                var oldFileContent = dbContext.FileContents.Find(fc.Id);

                if (oldFileContent == null)
                {
                    Console.WriteLine("Can't update filecontent since it isn't in Database");

                    return false;
                }
                else
                {

                    oldFileContent.Content = fc.Content;
                    oldFileContent.FileId = fc.FileId;
                    oldFileContent.Id = fc.Id;
                    try
                    {
                        dbContext.SaveChanges();
                        Console.WriteLine(DateTime.Now + ": FileContent Updated to Database.");
                        return true;
                    }
                    catch
                    {
                        Console.WriteLine(DateTime.Now + ": Something wrong with FileContent updating to Database.");
                        return false;
                        throw new ArgumentException("Out of range!");
                    }

                }
            }
        }

        public string GetFileContentId(string fileId)
        {
            if (fileId == null)
            {
                throw new ArgumentNullException("Arguments can't be null");
            }


            FileContent fc = null;
            using (var dbContext = new FileContext())
            {
                try
                {
                    fc = dbContext.FileContents.FirstOrDefault(x => x.FileId == fileId);

                    if (fc != null)
                    {
                        return fc.Id;
                    }

                    return null;
                }
                catch
                {

                     throw new ArgumentException("Something wrong with the function GetContent for Database");

                }
            }
        }


        public string GetContent(string id)
        {
            if(id==null)
            {
                throw new ArgumentNullException("Arguments can't be null");
            }
            using (var dbContext = new FileContext())
            {

                try
                {
                    FileContent fileContent = new FileContent();
                    fileContent = null;
                    fileContent = dbContext.FileContents.FirstOrDefault(u => u.FileId == id);

                    if (fileContent != null)
                        return fileContent.Content;

                    else
                        return null;
                }
                catch
                {
                    throw new ArgumentException("Something wrong with the function GetContent for Database");

                }
            }
        }

        #endregion
    }
}
