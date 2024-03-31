


ML 23/24-09 Approve Prediction of Multisequence Learning  

## Introduction Part

In this project, I have  introduced novel enhancements to the MultisequenceLearning algorithm. These enhancements involve automating the process of dataset retrieval from a specified path using the function HelpMethod.ReadDataset(datasetPath). Additionally, I  possess test data located in a separate file, which also needs to be read for subsequent evaluation of subsequences, formatted similarly to HelpMethod.ReadDataset(testsetPath). The function RunMultiSequenceLearningExperiment(sequences, sequencesTest) is employed to execute the learning experiment, utilizing multiple sequences provided in sequences along with test subsequences from sequencesTest. Following the completion of the learning phase, the accuracy of predicted elements is calculated for evaluation.


flow chart can be found [On the path](../Documentaion/Flow_Chart):

Above the flow of implementation of my project.

`Sequence` is the model of how I process and store the dataset. And can be seen below:

 csharp code
public class Sequence
{
        public String name { get; set; }
        public int[] data { get; set; }
}


eg:
- Dataset

json part
[
  {
    "name": "S1",
    "data": [ 0, 2, 5, 6, 7, 8, 10, 11, 13 ]
  },
  {
    "name": "S2",
    "data": [ 1, 2, 3, 4, 6, 11, 12, 13, 14 ]
  },
  {
    "name": "S3",
    "data": [ 1, 2, 3, 4, 7, 8, 10, 12, 14 ]
  }
]

- Test Dataset
[
  {
    "name": "T1",
    "data": [ 1, 2, 4 ]
  },
  {
    "name": "T2",
    "data": [ 2, 3, 4 ]
  },
  {
    "name": "T3",
    "data": [ 4, 5, 7 ]
  },
  {
    "name": "T4",
    "data": [ 5, 8, 9 ]
  }
]


My Project is implemented on methods are in `HelpMethod.cs` and file can be found [On the path](../HelpMethod.cs):

1. FetchHTMConfig()

Here I saved the HTMConfig which is used for Hierarchical Temporal Memory for the `Connections`

/// <summary>
/// HTM Config for creating Connections
/// <param name="inputBits">input bits</param>
/// <param name="numColumns">number of columns</param>
/// <returns>Object of HTMConfig</returns>
public static HtmConfig FetchHTMConfig(int inputBits, int numColumns)
{
    HtmConfig cfg = new HtmConfig(new int[] { inputBits }, new int[] { numColumns })
    {
        Random = new ThreadSafeRandom(42),

        CellsPerColumn = 25,
        GlobalInhibition = true,
        LocalAreaDensity = -1,
        NumActiveColumnsPerInhArea = 0.02 * numColumns,
        PotentialRadius = (int)(0.15 * inputBits),
        MaxBoost = 10.0,
        DutyCyclePeriod = 25,
        MinPctOverlapDutyCycles = 0.75,
        MaxSynapsesPerSegment = (int)(0.02 * numColumns),
        ActivationThreshold = 15,
        ConnectedPermanence = 0.5,e.
        PermanenceDecrement = 0.25,
        PermanenceIncrement = 0.15,
        PredictedSegmentDecrement = 0.1,
    };

    return cfg;
}

2. getEncoder()

I have used `ScalarEncoder` so that we all numeric value are encoding here.

I took that `inputBits` which work same as `HTMConfig`.

/// <summary>
/// Get the encoder with settings
/// <param name="inputBits">input bits</param>
/// <returns>Object of EncoderBase</returns>
public static EncoderBase GetEncoder(int inputBits)
{
        double max = 20;

        Dictionary<string, object> settings = new Dictionary<string, object>()
        {
        { "W", 15},
        { "N", inputBits},
        { "Radius", -1.0},
        { "MinVal", 0.0},
        { "Periodic", false},
        { "Name", "scalar"},
        { "ClipInput", false},
        { "MaxVal", max}
        };

        EncoderBase encoder = new ScalarEncoder(settings);

        return encoder;
}

Additional Info: "MaxValue" for encoder is set to "20" which can be change but then this value should be matched while creating synthetic dataset.

3. ReadDataset()

While reading the JSON file, I passed as full path and retuns the object of list of `Sequence`

/// <summary>
/// Reads dataset from the file
/// <param name="path">full path of the file</param>
/// <returns>Object of list of Sequence</returns>
public static List<Sequence> ReadDataset(string path)
{
        Console.WriteLine("Reading Sequence...");
        String lines = File.ReadAllText(path);
        //var sequence = JsonConvert.DeserializeObject(lines);
        List<Sequence> sequence = System.Text.Json.JsonSerializer.Deserialize<List<Sequence>>(lines);

        return sequence;
}

4. CreateDataset()

I've implemented an enhancement to automate dataset creation, thereby eliminating the need for manual effort. This feature enables the generation of datasets based on specified parameters such as the numberOfSequence to be created, the size of each sequence, and optionally, the startVal and endVal to define the range of values within each sequence.

/// <summary>
/// Creates list of Sequence as per configuration
/// <returns>Object of list of Sequence</returns>

public static List<Sequence> CreateDataset()
{
        int numberOfSequence = 3;
        int size = 12;
        int startVal = 0;
        int endVal = 15;
        Console.WriteLine("Creating Sequence...");
        List<Sequence> sequence = HelpMethod.CreateSequences(numberOfSequence, size, startVal, endVal);

        return sequence;
}


5. SaveDataset()

The dataset is stored within the dataset directory located at the BasePath of the application's execution environment.


/// <summary>
/// Saves the dataset in 'dataset' folder in BasePath of application
/// <param name="sequences">Object of list of Sequence</param>
/// <returns>Full path of the dataset</returns>

public static string SaveDataset(List<Sequence> sequences)
{
        string BasePath = AppDomain.CurrentDomain.BaseDirectory;
        string reportFolder = Path.Combine(BasePath, "dataset");
        if (!Directory.Exists(reportFolder))
        Directory.CreateDirectory(reportFolder);
        string reportPath = Path.Combine(reportFolder, $"dataset_{DateTime.Now.Ticks}.json");

        Console.WriteLine("Saving dataset...");

        if (!File.Exists(reportPath))
        {
        using (StreamWriter sw = File.CreateText(reportPath))
        {
          sw.WriteLine(JsonConvert.SerializeObject(sequences));
        }
        }

        return reportPath;
}


6. Calculating accuracy in PredictNextElement() in `Program.cs`

int matchCount = 0;
int predictions = 0;
double accuracy = 0.0;

foreach (var item in list)
{
    Predict();
    //compare current element with prediction of previous element
    if(item == Int32.Parse(prediction.Last()))
    {
        matchCount++;
    }
    predictions++;
    accuracy = (double)matchCount / predictions * 100;
}



## How to run the project

### To create synthetic dataset

1. Open the [sln](../../../NeoCortexApi.sln) and select `MultiSequenceLearning` as startup project.

2. In `Program.cs` I have the `Main()`. 
   Uncomment below code for creating a synthetic dataset.

csharp
//to create synthetic dataset
string path = HelpMethod.SaveDataset(HelpMethod.CreateDataset());
Console.WriteLine($"Dataset saved: {path}");


3. Run to create dataset 

### To run the experiment

1. Open the [NeoCortexApi.sln](../../../NeoCortexApi.sln) and select `MultiSequenceLearning` as startup project.

2. In `Program.cs` I have the `Main()`. Change the name of `dataset` file saved from previous run  as seen below:


//to read dataset
string BasePath = AppDomain.CurrentDomain.BaseDirectory;
string datasetPath = Path.Combine(BasePath, "dataset", "dataset_03.json"); //edit name of dataset here
Console.WriteLine($"Reading Dataset: {datasetPath}");
List<Sequence> sequences = HelpMethod.ReadDataset(datasetPath);

and also *copy the [test data](../dataset/test_01.json) to the folder* (`{BASEPATH}\neocortexapi\source\SE Project 23-24\MultiSequenceLearning\bin\Debug\net6.0\dataset`).

## Results

I've conducted the experiment the maximum number of times feasible with various datasets. I have opted for small dataset sizes and a limited number of sequences to mitigate lengthy execution times.

Result can be found under the path, which contain screenshots and output of session and process completed.

![results](./Images/Output.png)

## Reference

Source:-

- Forked from [ddobric/neocortexapi](https://github.com/ddobric/neocortexapi)

- https://www.numenta.com/resources/research-publications/

- https://www.numenta.com/blog/2019/10/24/machine-learning-guide-to-htm/

- https://3rdman.de/2020/06/htm-core-and-csharp/

- https://www.frontiersin.org/articles/10.3389/fncir.2017.00081/full

- https://medium.com/hierarchical-learning/hierarchical-temporal-memory-overview-d411c9e4f90e

- https://journals.plos.org/ploscompbiol/article?id=10.1371/journal.pcbi.1011801

- https://aircconline.com/csit/csit1006.pdf


