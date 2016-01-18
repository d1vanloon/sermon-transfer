using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sermon_Transfer.Properties;

namespace Sermon_Transfer
{
    class Program
    {
        //Decide what to do
        static void Main(string[] args)
        {
            //Print header
            Console.WriteLine("Sermon Transfer - Copyright (c) 2012-2013 Intelligent Design Software\n");

            //Handle arguments
            if (args.Length > 0)
            {
                //If in settings mode, edit settings
                if (args[0] == "settings")
                {
                    editSettings();
                }
                //If in transfer mode, transfer files
                if (args[0] == "transfer")
                {
                    transferFiles();
                }
            }
            else
            {
                //If no mode specified, transfer files
                transferFiles();
            }
        }

        //Transfer all files in the source directory to a dated folder in the target directory
        private static void transferFiles()
        {
            //Open settings and get current date
            Settings settings = new Settings();
            DateTime currentDate = DateTime.Now;

            //Declare variables and load settings
            string fileName;
            string destFile;
            string sourcePath = settings.source;
            string targetPath = settings.target;

            //Set sub-directory based on current date
            string subDirectory = currentDate.ToString("MM-dd-yy");

            //Get the full path of the target, with the sub-directory
            string targetSubDirectory = System.IO.Path.Combine(targetPath, subDirectory);

            Console.WriteLine("Transferring files...");

            //If the source is valid
            if (System.IO.Directory.Exists(sourcePath))
            {
                //If the target path has been specified
                //if (targetPath != "")
                if(System.IO.Directory.Exists(targetPath))
                {
                    //If the date sub-directory does not exist
                    if (!System.IO.Directory.Exists(targetSubDirectory))
                    {
                        //Create target sub-directory
                        Console.WriteLine("Creating directory...");
                        System.IO.Directory.CreateDirectory(targetSubDirectory);
                        Console.WriteLine("Directory created.");
                    }

                    //Get all of the files in the source directory
                    string[] files = System.IO.Directory.GetFiles(sourcePath);

                    //If there are files to copy
                    if (files.Length > 0)
                    {
                        //Copy each of the files and overwrite destination files if they already exist.
                        foreach (string s in files)
                        {
                            //Use static Path methods to extract only the file name from the path.
                            fileName = System.IO.Path.GetFileName(s);
                            destFile = System.IO.Path.Combine(targetSubDirectory, fileName);
                            try
                            {
                                Console.WriteLine("Transferring " + fileName + "...");
                                System.IO.File.Copy(s, destFile, true);
                                Console.WriteLine("Transferred " + fileName + ".");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Could not transfer " + fileName + ".");
                            }

                            try
                            {
                                Console.WriteLine("Deleting " + fileName + "...");
                                System.IO.File.Delete(s);
                                Console.WriteLine("Deleted " + fileName + ".");
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Could not delete " + fileName + ".");
                            }
                        }
                    }
                    else
                    {
                        //No files to copy
                        Console.WriteLine("No files found.");
                        Console.WriteLine("Press any key to exit.");
                        Console.ReadKey(true);
                        Environment.Exit(0);
                    }
                }
                else
                {
                    //Target path does not exist
                    Console.WriteLine("Cannot find target path.");
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey(true);
                    Environment.Exit(0);
                }
            }
            else
            {
                //Source directory does not exist
                Console.WriteLine("Cannot find source path.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
                Environment.Exit(0);
            }

            //Success
            Console.WriteLine("Files transferred.");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
            Environment.Exit(0);
        }

        //Modify the source and target directories
        private static void editSettings()
        {
            //Open settings
            Settings settings = new Settings();

            //Load settings
            string sourcePath;
            string targetPath;
            bool doneChanging = false;

            //Loop until user quits (go at least once)
            do
            {
                //Re-load settings
                sourcePath = settings.source;
                targetPath = settings.target;

                //Display current settings
                Console.WriteLine("Settings:");
                Console.WriteLine("\tSource directory: " + sourcePath);
                Console.WriteLine("\tTarget directory: " + targetPath);

                //Display menu
                Console.WriteLine("Change settings? ");
                Console.WriteLine("Y - Yes");
                Console.WriteLine("N - No");
                char command = Console.ReadKey(true).KeyChar;

                //If YES
                if ((command == 'y') | (command == 'Y'))
                {
                    //Display menu
                    Console.WriteLine("Change source directory or target directory?");
                    Console.WriteLine("1 - Source Directory");
                    Console.WriteLine("2 - Target Directory");

                    //Variable to hold menu choice
                    int option;

                    //Input might not be of the right format
                    try
                    {
                        //Read input
                        option = int.Parse(Console.ReadKey(true).KeyChar.ToString());
                    }
                    catch (Exception e)
                    {
                        //Invalid input, show error message and prompt again
                        Console.WriteLine("Invalid input. Please try again. Details:\n" + e.Message);

                        //Skip back to the start of the loop
                        continue;
                    }

                    //Handle the choice
                    switch (option)
                    {
                        case 1: //Change source directory
                            Console.WriteLine("Please enter the new source directory:");
                            string tempSourcePath = Console.ReadLine();
                            //If the new directory exists
                            if (System.IO.Directory.Exists(tempSourcePath))
                            {
                                //Change settings
                                settings.source = tempSourcePath;
                                Console.WriteLine("Setting saved.");
                            }
                            else
                            {
                                //Directory doesn't exist, try again
                                Console.WriteLine("Invalid path. Please try again.");
                            }
                            break;
                        case 2: //Change target directory
                            Console.WriteLine("Please enter the new target directory:");
                            string tempTargetPath = Console.ReadLine();
                            //If the new directory exists
                            if (System.IO.Directory.Exists(tempTargetPath))
                            {
                                //Change settings
                                settings.target = tempTargetPath;
                                Console.WriteLine("Setting saved.");
                            }
                            else
                            {
                                //Directory doesn't exist, try again
                                Console.WriteLine("Invalid path. Please try again.");
                            }
                            break;
                        default:
                            //User chose a number other than 1 or 2
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    //Chose NO to change settings?
                    Console.WriteLine("Done changing settings.");
                    //Set watch variable to true, causes loop exit
                    doneChanging = true;
                }
            } while (!doneChanging);

            //Save settings to disk
            settings.Save();
            //Quit application
            Environment.Exit(0);
        }
    }
}
