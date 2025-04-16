# CS_3502_Project_2
CS 3502 Project 2- CPU Scheduling 

## Overview
This project simulates two complex CPU Scheduling Algorithms in order to evaluate their performance under different workloads: Shortest Remaining Time First (SRTF) and Highest Response Ratio Next (HRRN). The program was developed so that the two algorithms could be analyzed and measured in terms of efficiency, fairness, and responsiveness in process scheduling. 

## Algorithms Implemented
**SRTF (Shortest Remaining Time First)**: A preemptive scheduling algorithm that prioritizes the process with the shortest remaining burst time.
**HRRN (Highest Response Ratio Next)**: A non-preemptive scheduling algorithm that calculates the response ratio for each process to fairly prioritize longer-waiting or larger processes.

## Features
- Handles process scheduling using both algorithms
- Outputs detailed metrics in table format:
     - Completion Time (CT)
     - Turnaround Time (TAT)
     - Waiting Time (WT)
     - Average TAT and WT
     - CPU Utilization
     - Throughput
- Automatically runs three different test cases;
    - **Small Dataset Test** (3-5 processes, manually defined process data)
    - **Large-Scale Test** (10-50 processes, randomly generated process data)
    - **Edge Case Tests**
        - Edge Case 1: All processes arrive at time 0 with identical burst times
        - Edge Case 2: Randomly generated arrival and burst times with extreme variations

## How to Run
1. Clone this repository or download the source code file.
2. Open the project in Visual Studio or your preferred C# IDE.
3. Build and run the project.

   Alternatively, from the command line:
   ```bash
   dotnet build
   dotnet run
