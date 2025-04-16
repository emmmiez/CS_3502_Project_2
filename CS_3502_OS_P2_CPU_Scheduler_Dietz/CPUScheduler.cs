using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace CS_3502_OS_P2_CPU_Scheduler_Dietz
{
    //define the process control block:
    class Process
    {
        public int ID { get; set; } //unique identifier for each process
        public int ArrivalTime { get; set; } //time when the process arrives 
        public int BurstTime { get; set; } // total CPU time required 
        public int RemainingTime { get; set; } //remaining time left to finish
        public int CompletionTime { get; set; } //time at which the process completes execution
        public int WaitingTime { get; set; } //time the process spent waiting in the ready queue
        public int TurnaroundTime { get; set; } //total time from arrival to completion
        public double responseRatio { get; set; } //used for algorithm 2 of 2         
    }

    class CPUScheduler
    {


        //calculate average waiting time (AWT)
        static double CalculateAWT(List<Process> processes) 
        {
            double totalWaitingTime = 0;
            //for each process, add waiting time to total waiting time
            foreach (Process process in processes) 
            {
                totalWaitingTime += process.WaitingTime;
            }
            return totalWaitingTime / processes.Count; //calculate and return the average waiting time
        }


        //calculate average turnaround time (ATT)
        static double CalculateATT(List<Process> processes)
        {
            double totalTurnaroundTime = 0;
            //for each process, add turnaround time to total turnaround time
            foreach (Process process in processes)
            {
                totalTurnaroundTime += process.TurnaroundTime;
            }
            return totalTurnaroundTime / processes.Count; //calculate and return the total turnaround time
        }


        //calculate cpu utilization (%)
        static double CalculateCPUUtilization(List<Process> processes, int totalTime)
        {
            int totalCpuUtilization = 0;
            foreach (Process process in processes)
            {
                totalCpuUtilization += process.BurstTime;
            }
            return ((double)totalCpuUtilization / totalTime) * 100; //calculate and return cpu utilization
        }


        //calculate throughput (processes per second)
        static double CalculateThroughput(List<Process> processes, int totalTime)
        {
            return (double)processes.Count / totalTime; //calculate and return throughput
        }
        

        //***shortest remaining time first algorithm 1 of 2***
        void RunSRTF(List<Process> processes)
        {
            int time = 0;
            int completed = 0; //number of completed processes
            int n = processes.Count; //total number of processes
            Process currentProcess = null;  

            //run until all processes are complete
            while(completed < n)
            {
                //select process with the shortest remaining time 
                var readyQueue = processes
                    .Where(p => p.ArrivalTime <= time && p.RemainingTime > 0) 
                    .OrderBy(p => p.RemainingTime) //choose the process with least remaining time
                    .ThenBy(p => p.ArrivalTime) //if two processes have the same remaining time, the one that arrived earlier gets chosen 
                    .ToList(); //convert the data to a list

                //if we found any ready processes:
                if(readyQueue.Any())
                {
                    currentProcess = readyQueue.First(); //pick the process with shortest reamining time 
                    currentProcess.RemainingTime--; 

                    //if the process has finished:
                    if(currentProcess.RemainingTime == 0)
                    {
                        currentProcess.CompletionTime = time + 1; //record completion time
                        currentProcess.TurnaroundTime = currentProcess.CompletionTime - currentProcess.ArrivalTime; //TAT = CT - AT
                        currentProcess.WaitingTime = currentProcess.TurnaroundTime - currentProcess.BurstTime; //WT = TAT - BT
                        completed++; //increase count of completed processes
                    }
                }
                time++; //move time forward 1 unit
            }
            //time to print the results
            Console.WriteLine("\nProcess\tAT\tBT\tCT\tTAT\tWT");

            //print the data for each process in table format
            foreach(var p in processes.OrderBy(p => p.ID))
            {
                Console.WriteLine($"P{p.ID}\t{p.ArrivalTime}\t{p.BurstTime}\t{p.CompletionTime}\t{p.TurnaroundTime}\t{p.WaitingTime}");
            }

            //calculate and print metrics to screen
            double averageWT = CalculateAWT(processes);
            double averageTAT = CalculateATT(processes);
            double cpuUtilization = CalculateCPUUtilization(processes, time);
            double throughput = CalculateThroughput(processes, time);

            Console.WriteLine($"\nAverage Waiting Time: {averageWT:F2}");
            Console.WriteLine($"Average Turnaround Time: {averageTAT:F2}");
            Console.WriteLine($"CPU Utilization: {cpuUtilization:F2}%");
            Console.WriteLine($"Throughput: {throughput:F2} processes/unit time\n");
        }


        //***Highest Response Ratio Next algorithm 2 of 2***
        public void RunHRRN(List<Process> processes)
        {
            int time = 0; 
            int completed = 0;
            int n = processes.Count;
            List<Process> queue = new List<Process>();

            Console.WriteLine("Order of execution: ");

            //continue until all processes are completed 
            while(completed < n)
            {
                //get all processes that have arrived
                queue = processes.Where(p => p.ArrivalTime <= time && p.RemainingTime > 0).ToList();

                //if no process is ready, move time forward
                if (queue.Count == 0)
                {
                    time++;
                    continue;
                }

                //calculate response ratios
                foreach(var p in queue)
                {
                    int waitingTime = time - p.ArrivalTime;
                    p.responseRatio = (waitingTime + p.BurstTime) / (double)p.BurstTime;
                }

                //select the process with the highest response ratio
                var nextProcess = queue.OrderByDescending(p => p.responseRatio).First();

                time += nextProcess.BurstTime;
                nextProcess.CompletionTime = time;
                nextProcess.TurnaroundTime = nextProcess.CompletionTime - nextProcess.ArrivalTime;
                nextProcess.WaitingTime = nextProcess.TurnaroundTime - nextProcess.BurstTime;
                nextProcess.RemainingTime = 0;
                completed++;

                Console.WriteLine($"P{nextProcess.ID} | End: {nextProcess.CompletionTime}, Waiting: {nextProcess.WaitingTime}");
            }
            //time to print the results
            Console.WriteLine("\nProcess\tAT\tBT\tCT\tTAT\tWT");

            //print the data for each process in table format
            foreach (var p in processes.OrderBy(p => p.ID))
            {
                Console.WriteLine($"P{p.ID}\t{p.ArrivalTime}\t{p.BurstTime}\t{p.CompletionTime}\t{p.TurnaroundTime}\t{p.WaitingTime}");
            }

            //calculate and print metrics to screen
            double averageWT = CalculateAWT(processes);
            double averageTAT = CalculateATT(processes);
            double cpuUtilization = CalculateCPUUtilization(processes, time);
            double throughput = CalculateThroughput(processes, time);

            Console.WriteLine($"\nAverage Waiting Time: {averageWT:F2}");
            Console.WriteLine($"Average Turnaround Time: {averageTAT:F2}");
            Console.WriteLine($"CPU Utilization: {cpuUtilization:F2}%");
            Console.WriteLine($"Throughput: {throughput:F2} processes/unit time\n");
        }
        

        //method for large scale testing using random data
        static List<Process> LargeScaleTest()
        {
            Random random = new Random(); 
            List<Process> processes = new List<Process>(); //create a list of processes

            int numbProcesses = random.Next(10, 51);

            for(int i = 0; i < numbProcesses; i++)
            {
                int arrival = random.Next(0, 51);
                int burst = random.Next(1, 21);

                //generate random data for each process and add it to the list
                processes.Add(new Process
                {
                    ID = i + 1,
                    ArrivalTime = arrival,
                    BurstTime = burst,
                    RemainingTime = burst,
                });
            }
            return processes;
        }


        //method for edge case testing (test 1- all processes arrive at time 0 with identical burst times)
        static List<Process> EdgeCaseTest1(int numbProcesses)
        {
            List<Process> processes = new List<Process>(); //create a list of processes

            for(int i = 0; i < numbProcesses; i++)
            {
                //create each process with the same burst time (5) and arrival time 0
                processes.Add(new Process
                {
                    ID = i + 1,
                    ArrivalTime = 0,
                    BurstTime = 5,
                    RemainingTime = 5
                });
            }
            return processes;
        }


        //method for edge case testing (test 2- mix of very long and short burst times)
        static List<Process> EdgeCaseTest2(int numbProcesses)
        {
            List<Process> processes = new List<Process>(); //create a list of processes 
            Random random = new Random(); //create a random number generator 

            for(int i = 0; i < numbProcesses; i++)
            {
                processes.Add(new Process
                {
                    ID = i + 1,
                    ArrivalTime = random.Next(0, 10),
                    BurstTime = (i % 2 == 0) ? 1 : 20, //mix of short and long burst times
                    RemainingTime = (i % 2 == 0) ? 1 : 20
                });
            }
            return processes;
        }


        //main method 
        static void Main(string[] args)
        {
            int choice;

            while (true) //ensure the user selects choices 1 or 2
            {
                //give the user a choice on which algorithm to use and test
                Console.WriteLine("Choose an algorithm for testing: ");
                Console.WriteLine("1. Shortest Remaining Time First (SRTF) ");
                Console.WriteLine("2. Highest Response Ratio Next (HRRN) ");

                if (int.TryParse(Console.ReadLine(), out choice) && (choice == 1 || choice == 2))
                {
                    break; //if the user enters 1 or 2, break out of the loop
                }
                else
                {
                    Console.WriteLine("Invalid choice! Please enter 1 or 2. \n");
                }
            }

            Console.Write("Enter number of processes: ");
            int numbProcesses = int.Parse(Console.ReadLine());

            List<Process> processes = new List<Process>(); //create a list for each process to be stored 

            Console.WriteLine("Enter process details: ");
            for (int i = 0; i < numbProcesses; i++)
            {
                Console.Write($"Enter arrival time for process {i + 1}: ");
                int arrivalTime = int.Parse(Console.ReadLine());

                Console.Write($"Enter burst time for process {i + 1}: ");
                int burstTime = int.Parse(Console.ReadLine());

                //create the process using the given data and add it to the list
                processes.Add(new Process
                {
                    ID = i + 1,
                    ArrivalTime = arrivalTime,
                    BurstTime = burstTime,
                    RemainingTime = burstTime,
                    CompletionTime = 0,
                    TurnaroundTime = 0,
                    WaitingTime = 0,
                });
            }

            CPUScheduler scheduler = new CPUScheduler(); //create an instance of the CPUScheduler class 

            if (choice == 1)
            {
                Console.WriteLine("\nRunning Shortest Remaining Time First (SRTF) Scheduling Algorithm... \n");
                scheduler.RunSRTF(processes); //call the SRTF method 
            }
            else if (choice == 2)
            {
                Console.WriteLine("\nRunning Highest Response Ratio Next (HRRN) Scheduling Algorithm... \n");
                scheduler.RunHRRN(processes); //call the MLFQ method
            }


            //run the large scale testing method based on user selection
            Console.WriteLine("\nRunning Large Scale Test... ");
            List<Process> largeScaleProcesses = LargeScaleTest();
            if (choice == 1)
            {
                scheduler.RunSRTF(largeScaleProcesses); //run SRTF on large scale data
            }
            else
            {
                scheduler.RunHRRN(largeScaleProcesses); //run MLFQ on large scale data
            }


            //run the edge case test methods based on user selection
            Console.WriteLine("\nRunning Edge Case Test 1... ");
            List<Process> edgeCaseTest1 = EdgeCaseTest1(numbProcesses);
            if(choice == 1)
            {
                scheduler.RunSRTF(edgeCaseTest1); //run SRTF on edge case test 1 
            }
            else
            {
                scheduler.RunHRRN(edgeCaseTest1); //run MLFQ on edge case test 1
            }

            //edge case test 2
            Console.WriteLine("\nRunning Edge Case Test 2... ");
            List<Process> edgeCaseTest2 = EdgeCaseTest2(numbProcesses);
            if(choice == 1)
            {
                scheduler.RunSRTF(edgeCaseTest2); //run SRTF on edge case test 2 
            }
            else
            {
                scheduler.RunHRRN(edgeCaseTest2); //run MLFQ on edge case test 2
            }
        }
    }
}