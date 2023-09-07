using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;



class Program
{
    static CustomVisionPredictionClient Client;
    static async Task Main(string[] args)
    {
        string endpoint;
        string Projectkey;
        Guid projectId;
        string predictionKey;
        string modelname;
        
        try
        {

            // Get Configuration Settings
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("C:\\Users\\Kenne\\source\\repos\\Upgift2Vision\\Upgift2Vision\\Appsetting.json");
            IConfigurationRoot configuration = builder.Build();
            endpoint = configuration["Endpoint"];
            Projectkey = configuration["Key"];
            projectId = Guid.Parse(configuration["Id"]);
            predictionKey = (configuration["PredictionKey"]);
            modelname = configuration["ModelName"];


            // Authenticate a client for the prediction API
            Client = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            
            Console.WriteLine("Please choose if you want url or filepath");
            Console.WriteLine("1 url");
            Console.WriteLine("2 filepath");
            var choose = Console.ReadLine();
            switch (choose)
            {
                case "1":
                    Console.WriteLine("Input url");
                    string url = Console.ReadLine();
                    await GetUrl(url);
                    break;
                case "2":
                    Console.WriteLine("Input filepath");
                    string filepath = Console.ReadLine();
                    await GetFilepath(filepath);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }



        async Task GetUrl(string url)
        {

            var imageUrl = url;
            var imageurlcog = new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models.ImageUrl(imageUrl);
            imageurlcog.Validate();
            Console.WriteLine(url);
            if (imageUrl != null)
            {

                var result = await Client.ClassifyImageUrlAsync(projectId, modelname, imageurlcog);
                Console.Write(Environment.NewLine);
                foreach (var prediction in result.Predictions)
                {
                    Console.WriteLine($"Tag: {prediction.TagName} - Probability: {String.Format("Value: {0:P2}.", prediction.Probability)}");
                }
            }
        }
        async Task GetFilepath(string filepath)
        {
            String[] images = Directory.GetFiles(filepath);
            foreach (var image in images)
            {
                using (MemoryStream imageData = new MemoryStream(File.ReadAllBytes(image)))
                {
                    var resultImages = await Client.ClassifyImageAsync(projectId, modelname, imageData);
                    foreach (var prediction in resultImages.Predictions)
                    {
                        if (prediction.Probability > 0.5)
                        {
                            Console.WriteLine($"{prediction.TagName} ({prediction.Probability:P1})");
                        }
                    }
                }
            }
        }
    }
}