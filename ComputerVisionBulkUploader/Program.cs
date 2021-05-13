using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using System;
using System.Collections.Generic;
using System.IO;

namespace ComputerVisionBulkUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            //Load env vars
            var root                                                    = Directory.GetCurrentDirectory();
            var dotenv                                                  = Path.Combine(root, "settings.env");
            Env.Load(dotenv);

            try
            {
                string strPath                                          = args[0];
                string strTag                                           = args[1].ToLower().Trim();

                Console.WriteLine("Load Files: " + strPath);
                string[] astrFiles                                      = Directory.GetFiles(strPath);
                Console.WriteLine("Found " + astrFiles.Length.ToString() + " (s)");

                Console.WriteLine("Tag: " + strTag);
                Console.WriteLine("------------------");
                Console.WriteLine("Is this correct? (y/n)");

                string strConfirm = Console.ReadLine().ToLower().Trim();
                if (strConfirm == "n")
                    throw new Exception("Incorrect Settings");

                Console.WriteLine("Load Training Project");
                CustomVisionTrainingClient cvtClient = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(Environment.GetEnvironmentVariable("TRAINNING_KEY")))
                {
                    Endpoint                                            = Environment.GetEnvironmentVariable("ENDPOINT")
                };

                Console.WriteLine("Load Project");
                var cvProject                                           = cvtClient.GetProject(Guid.Parse(Environment.GetEnvironmentVariable("PROJECT_ID")));

                Console.WriteLine("Tags");
                var vTags                                               = cvtClient.GetTags(cvProject.Id);

                List<Guid> lstGuid                                      = new List<Guid>();

                foreach (var vTag in vTags)
                {
                    Console.Write(vTag.Name + " ");

                    if (vTag.Name.ToLower().Trim() == strTag)
                        lstGuid.Add(vTag.Id);
                }
                Console.WriteLine("");
                Console.WriteLine("Do the uploads");
                
                foreach (string strFile in astrFiles)
                {
                    Console.WriteLine("Working on: " + strFile);
                    cvtClient.CreateImagesFromData(cvProject.Id, File.OpenRead(strFile), lstGuid);
                    Console.WriteLine("Completed: " + strFile);
                }

                Console.WriteLine("Done!!!");
            }
            catch(Exception exError)
            {
                Console.WriteLine(exError.Message);
            }



        }
    }
}
