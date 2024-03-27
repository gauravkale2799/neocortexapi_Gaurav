﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using NeoCortexApi;
using System.Linq;

namespace NeoCortexApiSample
{
    class Program
    {
        private static int next;

        /// <summary>
        /// This sample shows a typical experiment code for SP and TM
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // SE Project: Approve Prediction of Multisequence Learning 

            // Starts experiment that demonstrates how to learn spatial patterns.

            //to creating synthetic dataset
            //string path = HelpMethod.SaveDataset(HelpMethod.CreateDataset());
            //Console.WriteLine($"Dataset saved: {path}");

            //to read dataset
            string BasePath = AppDomain.CurrentDomain.BaseDirectory;
            string datasetPath = Path.Combine(BasePath, "dataset", "dataset_03.json");
            Console.WriteLine($"Reading Dataset: {datasetPath}");
            List<Sequence> sequences = HelpMethod.ReadDataset(datasetPath);

            //to read test dataset
            string testsetPath = Path.Combine(BasePath, "dataset", "test_01.json");
            Console.WriteLine($"Reading Testset: {testsetPath}");
            List<Sequence> sequencesTest = HelpMethod.ReadDataset(testsetPath);


            //run learing  part only
            //RunSimpleMultiSequenceLearningExperiment(sequences);

            //run learning + prediction and generates report for results
            List<Report> reports = RunMultiSequenceLearningExperiment(sequences, sequencesTest);

            WriteReport(sequences, reports);

            Console.WriteLine("Done...");

        }



        /*private static List<Report> RunMultiSequenceLearningExperiment(List<Sequence> sequences, List<Sequence> sequencesTest)
        {
            throw new NotImplementedException();
        }*/

        // <summary>
        /// write and formats data in report object to a file
        /// </summary>
        /// <param name="sequences">input sequence</param>
        /// <param name="reports">object of report</param>
        private static void WriteReport(List<Sequence> sequences, List<Report> reports)
        {
            string BasePath = AppDomain.CurrentDomain.BaseDirectory;
            string reportFolder = Path.Combine(BasePath, "report");
            if (!Directory.Exists(reportFolder))
                Directory.CreateDirectory(reportFolder);
            string reportPath = Path.Combine(reportFolder, $"report_{DateTime.Now.Ticks}.txt");
            if (!File.Exists(reportPath))
            {
                using (StreamWriter sw = File.CreateText(reportPath))
                {
                    sw.WriteLine("------------------------------");
                    foreach (Sequence sequence in sequences)
                    {
                        sw.WriteLine($"Sequence: {sequence.name} -> {string.Join("-", sequence.data)}");
                    }
                    sw.WriteLine("------------------------------");
                    foreach (Report report in reports)
                    {
                        sw.WriteLine($"Using test sequence: {report.SequenceName} -> {string.Join("-", report.SequenceData)}");
                        foreach (string log in report.PredictionLog)
                        {
                            sw.WriteLine($"\t{log}");
                        }
                        sw.WriteLine($"\tAccuracy: {report.Accuracy}%");
                        sw.WriteLine("------------------------------");
                    }
                }
            }
        }
        private static void RunSimpleMultiSequenceLearningExperiment(List<Sequence> sequences)
        {
            // Prototype for building the prediction engine.
            //List<Report> reports = new List<Report>();
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);
        }


        /// <summary>
        /// This example demonstrates how to learn two sequences and how to use the prediction mechanism.
        /// First, two sequences are learned.
        /// Second, three short sequences with three elements each are created und used for prediction. The predictor used by experiment privides to the HTM every element of every predicting sequence.
        /// The predictor tries to predict the next element.
        /// </summary>



        private static List<Report> RunMultiSequenceLearningExperiment(List<Sequence> sequences, List<Sequence> sequencesTest)
        {

            //
            // Prototype for building the prediction engine.
            List<Report> reports = new List<Report>();
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            foreach (Sequence item in sequencesTest)
            {
                Report report = new Report();
                report.SequenceName = (string)item.name;
                Debug.WriteLine($"Using test sequence: {item.name}");
                Console.WriteLine("------------------------------");
                Console.WriteLine($"Using test sequence: {item.name}");
                predictor.Reset();
                report.SequenceData = item.data;
                var accuracy = PredictNextElement(predictor, item.data, report);
                reports.Add(report);
                Console.WriteLine($"Accuracy for {item.name} sequence: {accuracy}%");
            }

            return reports;

            //
            // These list are used to see how the prediction works.
            // Predictor is traversing the list element by element. 
            // By providing more elements to the prediction, the predictor delivers more precise result.
            /*var list1 = new double[] { 1.0, 2.0, 3.0, 4.0, 2.0, 5.0 };
            var list2 = new double[] { 2.0, 3.0, 4.0 };
            var list3 = new double[] { 8.0, 1.0, 2.0 };

            predictor.Reset();
            PredictNextElement(predictor, list1);

            predictor.Reset();
            PredictNextElement(predictor, list2);

            predictor.Reset();
            PredictNextElement(predictor, list3);*/
        }

        private static double PredictNextElement(Predictor predictor, int[] list, Report report)
        {

            int matchCount = 0;
            int predictions = 0;
            double accuracy = 0.0;
            List<string> logs = new List<string>();
            Console.WriteLine("------------------------------");


            int prev = -1;
            bool first = true;

            foreach (var item in list)
            {
                //var res = predictor.Predict(item);
                if (first)
                {
                    first = false;
                }
                else
                {
                    Console.WriteLine($"Input: {prev}");
                    var res = predictor.Predict(prev);
                    string log = "";

                    if (res.Count > 0)
                    {
                        foreach (var pred in res)
                        {
                            Debug.WriteLine($"Predicted Input: {pred.PredictedInput} - Similarity: {pred.Similarity}");
                        }

                        var sequence = res.First().PredictedInput.Split('_');
                        var prediction = res.First().PredictedInput.Split('-');
                        Console.WriteLine($"Predicted Sequence: {sequence.First()} - Predicted next element: {prediction.Last()}");
                        log = $"Input: {prev}, Predicted Sequence: {sequence.First()}, Predicted next element: {prediction.Last()}";
                        //compare current element with prediction of previous element
                        if (next == Int32.Parse(prediction.Last()))
                        {
                            matchCount++;
                        }
                    }
                    else
                    {

                        Console.WriteLine("Nothing predicted :(");
                        log = $"Input: {prev}, Nothing predicted";



                    }
                    logs.Add(log);
                    predictions++;
                }

                //save previous element to compare with upcoming element
                prev = next;

            }
            report.PredictionLog = logs;

            /*
             * Accuracy is calculated as number of matching predictions made 
             * divided by total number of prediction made for an element in subsequence
             * 
             * accuracy = number of matching predictions/total number of prediction * 100
             */
            accuracy = (double)matchCount / predictions * 100;
            report.Accuracy = accuracy;

            Console.WriteLine("------------------------------");
            return accuracy;
        }


    }
}

