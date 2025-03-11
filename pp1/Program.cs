


using System.CommandLine.Invocation;
using System;
using System.CommandLine;
using System.Globalization;
using System.IO;
using System.Linq;


//יצירת השורש של הפקודות
var rootComand = new RootCommand("Root comand for file Bundler CLI");

//יצירת פקודה חדשה שהיא תעשה : תארוז קבצי קוד לקובץ טקסט
var bundleComand = new Command("bundle", "Bundle code files to a single file");
//מוסיף לשורש הפקודות את הפקודה שיצרנו שורה מעל  
rootComand.AddCommand(bundleComand);

//יצירת אפשרות שמכילה מידע על הקובץ כגון ניתוב אליו גודלו וכו...
var bundleOption = new Option<FileInfo>("--output", "file path and name");
//מוסיף לפקודה  את האפשרות שיצרנו שורה מעל
bundleComand.AddOption(bundleOption);

// אפשרות  של שפות
var languageOption = new Option<String>("--language", "give the language the patiant ask")
{
    //אפשרות שהיא חובה
    IsRequired = true
};
//הוספת אפשרות השפות לפקודת הבנדל
bundleComand.AddOption(languageOption);


// אפשרות להוספת הערות עם נתיב הקובץ המקורי
var noteOption = new Option<bool>("--note", "Include source file path as a comment");
// הוספת האפשרות לפקודת bundle
bundleComand.AddOption(noteOption);

//אופציה לסינון לפי שם הקובץ או סיומת הקובץ-סוג הקובץ
var sortOption = new Option<string>("--sort", () => "name", "Sort files by 'name' or 'type'");
bundleComand.AddOption(sortOption);


// אפשרות להסרת שורות ריקות
var removeOption = new Option<bool>("--remove", "Remove empty lines from the files");
bundleComand.AddOption(removeOption);


//הוספת אפשרות להכניס את שם מי שקיבץ את הקסבצים
var nameOptin = new Option<string>("--name", "enter the name of how made the documents");
bundleComand.AddOption(nameOptin);


bundleComand.SetHandler((FileInfo output, string language, bool note, string sort, bool remove, string name) =>
{
    try
    {


        string[] files;
        if (language.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            // קבלת כל הקבצים בתיקייה הנוכחית
            files = Directory.GetFiles(Directory.GetCurrentDirectory());
        }
        else
        {
            // המרת רשימת השפות למערך של סיומות
            var extensions = language.Split(',').Select(ext => ext.Trim()).ToArray();

            // סינון הקבצים לפי הסיומות
            files = Directory.GetFiles(Directory.GetCurrentDirectory())
                .Where(file => extensions.Any(ext => file.EndsWith($".{ext}", StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }

        // מיון הקבצים לפי הערך שנבחר ב--sort
        files = sort.ToLower() switch
        {
            "type" => files.OrderBy(file => Path.GetExtension(file)).ToArray(),
            _ => files.OrderBy(file => Path.GetFileName(file)).ToArray() // ברירת מחדל - מיון לפי שם בלבד
        };




        // פתיחת קובץ הפלט לכתיבה (יוצר את הקובץ אם הוא לא קיים)
        using (var writer = new StreamWriter(output.FullName))
        {
            //מוסיף את השם של מי שייצר
            writer.WriteLine($"made the document: {name}");
            writer.WriteLine();
            foreach (var file in files)
            {
                if (note)
                {
                    writer.WriteLine($"// Source file: {Path.GetFullPath(file)}");
                    ////writer.WriteLine(file);
                }
                writer.WriteLine();


                foreach (var line in File.ReadLines(file))
                {
                    
                    // דילוג על שורות ריקות רק אם המשתמש ביקש להסירן
                    if (!remove && !string.IsNullOrWhiteSpace(line))
                    {
                        writer.WriteLine(line);
                       // Console.WriteLine("okkkk");
                    }
                    //if (!remove)
                    //{
                    //    writer.WriteLine("you input no");
                    //}
                }
                writer.WriteLine(); // מוסיף שורה ריקה בין תכני הקבצים השונים

            }
        }

        Console.WriteLine("file was create");
    }

    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine("file fath is not valide");
    }


}, bundleOption, languageOption, noteOption, sortOption, removeOption, nameOptin);











//יצירת פקודה חדשה 
var create_rsp_Comand = new Command("create-rsp", "quick on");
//מוסיף לשורש הפקודות את הפקודה שיצרנו שורה מעל  
rootComand.AddCommand(create_rsp_Comand);

//מימוש הפקודה
create_rsp_Comand.SetHandler(() =>
{
    //בקשה מהמשתמש להכניס שם לקובץ שיווצר
    Console.WriteLine("enter name of the exec file");
    string output = Console.ReadLine();

        
    //יצירת קובץ בשם שנתן לו המשתמש
    FileInfo f = new FileInfo(output);
    Console.WriteLine("enter name of the new file");
    string namefile=Console.ReadLine();


    //בקשה מהמשתמש להכניס את השפה הרצויה
    Console.WriteLine("enter language or all");
    string language = Console.ReadLine();


    //בקשה מהמשתמש להכניס האם הוא רוצה שיהיה ניתוב או לא
    bool note;
    while (true)
    {
        Console.Write("enter the path? (true/false): ");
        if (bool.TryParse(Console.ReadLine(), out note))
        {
            break; // אם ההמרה הצליחה, יוצאים מהלולאה
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
        }
    }


    //בקשה מהמשתמש להכניס לפי מה הוא רוצה שזה יהיה ממוין
    Console.Write("Sort files by 'name' or 'type': enter ");
    string sort = Console.ReadLine();


    //המשתמש צריך להכניס האם הוא רוצה שיימחקו שורות רייקות
    bool remove;
    while (true)
    {
        Console.Write("Remove empty lines from the files? (true/false): ");
        if (bool.TryParse(Console.ReadLine(), out remove))
        {
            break; // אם ההמרה הצליחה, יוצאים מהלולאה
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter 'true' or 'false'.");
        }
    }

    //הוראה למשתמש להכניס את שם שלו
    Console.Write("Enter your name (the creator of the document): ");
    string name = Console.ReadLine();




    //בניית מה שיהיה כתוב בתוך התגובה
    var commandText = $"--output {namefile} --language {language} " +
                     $"{(note ? "--note" : "")} --sort {sort} " +
                     $"{(remove ? "--remove" : "")} --name {name}";

    // כתיבה לקובץ באמצעות File.WriteAllText
    File.WriteAllText(f.FullName, commandText);
    Console.WriteLine("the new file created successfully");

});



rootComand.InvokeAsync(args).Wait();
























